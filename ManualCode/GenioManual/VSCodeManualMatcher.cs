using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeFlow.ManualOperations;
using CodeFlow.Utils;

namespace CodeFlow.GenioManual
{
    public class VSCodeManualMatcher
    {
        string vsCodeSnapshot;
        string fileName;
        int? cursorPos;
        static Dictionary<Type, ManualMatchProvider> providers;
        public string VsCodeSnapshot { get => vsCodeSnapshot; set => vsCodeSnapshot = value; }
        public int? CursorPos { get => cursorPos; set => cursorPos = value; }
        public string FileName { get => fileName; set => fileName = value; }
        public static Dictionary<Type, ManualMatchProvider> MatchProvider {
            get
            {
                return providers ?? (providers = GetManualMatchProviders());
            } }
        public VSCodeManualMatcher(string vscode, int cursorPos, string fileName)
        {
            VsCodeSnapshot = vscode;
            CursorPos = cursorPos;
            FileName = fileName;
        }
        public VSCodeManualMatcher(string vscode, string fileName)
        {
            VsCodeSnapshot = vscode;
            FileName = fileName;
        }

        public List<IManual> Match()
        {
            ConcurrentBag<IManual> matches = new ConcurrentBag<IManual>();
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
                Parallel.ForEach(positions, (begin, state) =>
                {
                    string beginString = provider.MatchBeginnig;
                    string endString = provider.MatchEnd;
                    ConstructorInfo constructor = matchType.GetConstructor(new Type[] { });

                    int beginUpperLine = -1;
                    string upperLine = "";
                    var end = VsCodeSnapshot.IndexOf(endString, begin, StringComparison.Ordinal);
                    int idx = begin + beginString.Length;
                    int i = VsCodeSnapshot.IndexOf(Util.NewLine, idx, StringComparison.Ordinal);

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
                            LocalFileName = FileName,
                            CodeStart = i + Util.NewLine.Length
                        };

                        // Match line above begin tag
                        var endUpperLine = VsCodeSnapshot.LastIndexOf(Util.NewLine, begin, StringComparison.Ordinal);
                        if (endUpperLine > -1)
                        {
                            beginUpperLine = VsCodeSnapshot.LastIndexOf(Util.NewLine, endUpperLine, StringComparison.Ordinal) +
                                             Util.NewLine.Length;
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
                            code = VsCodeSnapshot.Substring(i + Util.NewLine.Length);
                        }
                        else
                        {
                            int length = end - match.CodeStart;
                            var c = VsCodeSnapshot.Substring(match.CodeStart, length);
                            int tmp = c.LastIndexOf(Util.NewLine, StringComparison.Ordinal);
                            length = tmp != -1 ? tmp : 0;
                            
                            if (length > 0)
                                code = c.Substring(0, length);

                            anotherB = VsCodeSnapshot.IndexOf(beginString, i, length, StringComparison.Ordinal);
                        }

                        match.CodeLength = code.Length;
                        if (anotherB == -1)
                        {
                            if (!(constructor?.Invoke(null) is IManual man))
                                state.Break();
                            else
                            {
                                man.Code = PackageOperations.Instance.ForceDOSLine ? Util.ConverToDOSLineEndings(code)
                                        : code;
                                man.CodeId = g;
                                man.Code = man.CodeTransformKeyValue();
                                man.LocalMatch = match;
                                if (man.MatchAndFix(upperLine))
                                    match.MatchPos = beginUpperLine;
                                matches.Add(man);
                            }
                        }
                    }
                });
            }
            return matches.ToList();
        }

        public List<int> AllIndexesOf(string str, string value)
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
        public static Dictionary<Type, ManualMatchProvider> GetManualMatchProviders()
        {
            return GetTypeAttribute(Assembly.GetExecutingAssembly(), typeof(ManualMatchProvider));
        }
        public static Dictionary<Type, ManualMatchProvider> GetTypeAttribute(Assembly assembly, Type attribute)
        {
            Dictionary<Type, ManualMatchProvider> providers = new Dictionary<Type, ManualMatchProvider>();

            foreach (Type type in assembly.GetTypes())
                if (type.GetCustomAttributes(attribute, true).FirstOrDefault() is ManualMatchProvider provider)
                    providers.Add(type, provider);

            return providers;
        }
    }
}
