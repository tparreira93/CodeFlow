using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeFlowLibrary.Util;
using CodeFlowLibrary.Settings;

namespace CodeFlowLibrary.GenioCode
{
    public class VSCodeManualMatcher
    {
        private string _vsCodeSnapshot;
        private string _fileName;
        private int? _cursorPos;
        private bool _concurrentMatching;
        private bool _isMatching;
        private IProducerConsumerCollection<IManual> _matches;
        private static Dictionary<Type, ManualMatchProvider> _providers;

        public bool ConcurrentMatching
        {
            get => _concurrentMatching;
            set => _concurrentMatching = value;
        }
        public bool IsMatching
        {
            get => _isMatching;
        }
        public string VsCodeSnapshot { get => _vsCodeSnapshot; set => _vsCodeSnapshot = value; }
        public int? CursorPos { get => _cursorPos; set => _cursorPos = value; }
        public string FileName { get => _fileName; set => _fileName = value; }
        public static Dictionary<Type, ManualMatchProvider> MatchProvider {
            get
            {
                return _providers ?? (_providers = GetManualMatchProviders());
            } }
        public VSCodeManualMatcher(string vscode, int cursorPos, string fileName)
        {
            VsCodeSnapshot = vscode;
            CursorPos = cursorPos;
            FileName = fileName;
            _matches = new ConcurrentBag<IManual>();
            ConcurrentMatching = true;
        }
        public VSCodeManualMatcher(string vscode, string fileName)
        {
            VsCodeSnapshot = vscode;
            FileName = fileName;
            _matches = new ConcurrentBag<IManual>();
            ConcurrentMatching = true;
        }

        public List<IManual> Match()
        {
            _isMatching = true;
            foreach (KeyValuePair<Type, ManualMatchProvider> manualMatchProvider in MatchProvider)
            {
                Type matchType = manualMatchProvider.Key;
                ManualMatchProvider provider = manualMatchProvider.Value;
                List<int> positions;

                if (CursorPos == null)
                    positions = AllIndexesOf(VsCodeSnapshot, provider.MatchBeginnig);
                else
                {
                    int tmp = VsCodeSnapshot.LastIndexOf(provider.MatchBeginnig, (int) CursorPos, (int) CursorPos + 1,
                        StringComparison.Ordinal);
                    positions = new List<int>();
                    if (tmp != -1)
                        positions.Add(tmp);
                }
                if(ConcurrentMatching)
                {
                    Parallel.ForEach(positions, (begin, state) =>
                    {
                        IManual manual = DoMatch(matchType, provider, begin);
                        if (manual == null)
                            state.Break();
                        else
                            while (!_matches.TryAdd(manual))
                            { }
                    });
                }
                else
                {
                    foreach (int begin in positions)
                    {
                        IManual manual = DoMatch(matchType, provider, begin);
                        if (manual == null)
                            break;
                        _matches.TryAdd(manual);
                    }
                }
            }
            _isMatching = false;
            return _matches.ToList();
        }

        private IManual DoMatch(Type matchType, ManualMatchProvider provider, int begin)
        {
            string beginString = provider.MatchBeginnig;
            string endString = provider.MatchEnd;
            ConstructorInfo constructor = matchType.GetConstructor(new Type[] { });

            int beginUpperLine = -1;
            string upperLine = "";
            var end = VsCodeSnapshot.IndexOf(endString, begin, StringComparison.Ordinal);
            int idx = begin + beginString.Length;
            int i = VsCodeSnapshot.IndexOf(Helpers.NewLine, idx, StringComparison.Ordinal);

            string guid = VsCodeSnapshot.Substring(idx, Math.Abs(i - idx));
            guid = guid.Substring(0, 36);
            if (Guid.TryParse(guid, out Guid g))
            {
                var match = new ManualMatch
                {
                    MatchPos = begin,
                    MatchLength = end,
                    MatchType = matchType,
                    VsCodeSnapshot = this.VsCodeSnapshot,
                    FullFileName = FileName,
                    CodeStart = i + Helpers.NewLine.Length
                };

                // Match line above begin tag
                var endUpperLine = VsCodeSnapshot.LastIndexOf(Helpers.NewLine, begin, StringComparison.Ordinal);
                if (endUpperLine > -1)
                {
                    beginUpperLine = VsCodeSnapshot.LastIndexOf(Helpers.NewLine, endUpperLine, StringComparison.Ordinal) +
                                     Helpers.NewLine.Length;
                    if (beginUpperLine != -1 && endUpperLine - beginUpperLine > 0)
                    {
                        upperLine = VsCodeSnapshot.Substring(beginUpperLine,
                            endUpperLine - beginUpperLine);
                    }
                }

                int anotherB;
                string code = String.Empty;
                if (end == -1)
                {
                    anotherB = VsCodeSnapshot.IndexOf(beginString, i, StringComparison.Ordinal);
                    code = VsCodeSnapshot.Substring(i + Helpers.NewLine.Length);
                }
                else
                {
                    int length = end - match.CodeStart;
                    var c = VsCodeSnapshot.Substring(match.CodeStart, length);
                    int tmp = c.LastIndexOf(Helpers.NewLine, StringComparison.Ordinal);
                    length = tmp != -1 ? tmp : 0;

                    if (length > 0)
                        code = c.Substring(0, length);

                    anotherB = VsCodeSnapshot.IndexOf(beginString, i, length, StringComparison.Ordinal);
                }

                match.CodeLength = code.Length;
                if (anotherB == -1)
                {
                    if (constructor?.Invoke(null) is IManual man)
                    {
                        man.Code = ManualCodeOptions.ForceDOSLine ? Helpers.ConverToDOSLineEndings(code)
                                : code;
                        man.CodeId = g;
                        man.Code = man.CodeTransformKeyValue();
                        man.LocalMatch = match;
                        if (man.MatchAndFix(upperLine))
                            match.MatchPos = beginUpperLine;
                        return man;
                    }
                }
            }
            return null;
        }

        /*
         * Registers a collection. It is usefull when we want that the data is added to a global collection and we want process it while it does matching.
         */
        public void Register(IProducerConsumerCollection<IManual> consumerCollection)
        {
            _matches = consumerCollection;
        }

        private List<int> AllIndexesOf(string str, string value)
        {
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.Ordinal);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private static Dictionary<Type, ManualMatchProvider> GetManualMatchProviders()
        {
            return GetTypeAttribute(Assembly.GetExecutingAssembly(), typeof(ManualMatchProvider));
        }

        private static Dictionary<Type, ManualMatchProvider> GetTypeAttribute(Assembly assembly, Type attribute)
        {
            var providers = new Dictionary<Type, ManualMatchProvider>();

            foreach (Type type in assembly.GetTypes())
                if (type.GetCustomAttributes(attribute, true).FirstOrDefault() is ManualMatchProvider provider)
                    providers.Add(type, provider);

            return providers;
        }
    }
}
