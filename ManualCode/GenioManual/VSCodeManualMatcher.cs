using CodeFlow.GenioManual;
using CodeFlow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.ManualOperations
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
            List<IManual> matches = new List<IManual>();
            object lockObject = new object();
            Parallel.ForEach(MatchProvider, (provider, state) =>
            {
                Type currentType = provider.Key;
                string begin_string = provider.Value.MatchBeginnig;
                string end_string = provider.Value.MatchEnd;
                ConstructorInfo constructor = currentType.GetConstructor(new Type[] { });

                int begin = -1, beginUpperLine = -1;
                int end = 0;
                int endUpperLine = -1;
                string upperLine = "";
                ManualMatch match = null;
                List<IManual> tmpMatches = new List<IManual>();
                int max = int.MaxValue;

                if (constructor == null)
                    state.Break();

                if (CursorPos != null)
                {
                    begin = VsCodeSnapshot.LastIndexOf(begin_string, (int)CursorPos, (int)CursorPos + 1);
                    max = 1;
                }
                else
                    begin = VsCodeSnapshot.IndexOf(begin_string);

                while (begin != -1 && tmpMatches.Count < max)
                {
                    end = VsCodeSnapshot.IndexOf(end_string, begin);
                    if (CursorPos == null || (begin <= CursorPos && end >= CursorPos))
                    {

                        int idx = begin + begin_string.Length;
                        int i = VsCodeSnapshot.IndexOf(Util.NewLine, idx);

                        string guid = VsCodeSnapshot.Substring(idx, Math.Abs(i - idx));
                        guid = guid.Substring(0, 36);
                        if (Guid.TryParse(guid, out Guid g))
                        {
                            match = new ManualMatch();
                            match.MatchPos = begin;
                            match.MatchLength = end;
                            match.MatchType = currentType;
                            match.VsCodeSnapshot = this.VsCodeSnapshot;
                            match.LocalFileName = FileName;
                            match.CodeStart = i + Util.NewLine.Length;

                            endUpperLine = VsCodeSnapshot.LastIndexOf(Util.NewLine, begin);
                            if (endUpperLine > -1)
                            {
                                beginUpperLine = VsCodeSnapshot.LastIndexOf(Util.NewLine, endUpperLine) + Util.NewLine.Length;
                                if (beginUpperLine != -1 && endUpperLine - beginUpperLine > 0)
                                {
                                    upperLine = VsCodeSnapshot.Substring(beginUpperLine, endUpperLine - beginUpperLine);
                                }
                            }

                            end = VsCodeSnapshot.IndexOf(end_string, match.CodeStart);
                            string c = "", code = "";
                            if (end == -1)
                            {
                                int anotherB = VsCodeSnapshot.IndexOf(begin_string, i);
                                if (anotherB != -1)
                                    break;

                                code = VsCodeSnapshot.Substring(i + Util.NewLine.Length);
                                end = VsCodeSnapshot.Length - 1;
                            }
                            else
                            {
                                int length = end - match.CodeStart;
                                c = VsCodeSnapshot.Substring(match.CodeStart, length);
                                int tmp = c.LastIndexOf(Util.NewLine);
                                length = tmp != -1 ? tmp : 0;

                                code = "";
                                if (length > 0)
                                    code = c.Substring(0, length);

                                int anotherB = VsCodeSnapshot.IndexOf(begin_string, i, length);
                                if (anotherB != -1)
                                    break;
                            }

                            match.CodeLength = code.Length;

                            IManual man = constructor.Invoke(null) as IManual;
                            if (man == null)
                                state.Break();

                            man.Code = PackageOperations.Instance.ForceDOSLine ? Util.ConverToDOSLineEndings(code) : code;
                            man.CodeId = g;
                            man.Code = man.CodeTransformKeyValue();
                            man.LocalMatch = match;
                            if (man.MatchAndFix(upperLine))
                                match.MatchPos = beginUpperLine;

                            tmpMatches.Add(man);
                        }

                        begin = VsCodeSnapshot.IndexOf(begin_string, end);
                    }
                    else
                        begin = -1;
                }

                lock (lockObject)
                {
                    matches.AddRange(tmpMatches);
                }
            });

            return matches;
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
