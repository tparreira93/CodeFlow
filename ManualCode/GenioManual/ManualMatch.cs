using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.ManualOperations
{
    public class ManualMatch
    {
        Type matchType;
        string vsCodeSnapshot;
        int matchPos;
        int matchLength;
        int codeStart;
        int codeLength;
        string fullFileName;

        public Type MatchType { get => matchType; set => matchType = value; }
        public string VsCodeSnapshot { get => vsCodeSnapshot; set => vsCodeSnapshot = value; }
        public int MatchPos { get => matchPos; set => matchPos = value; }
        public int MatchLength { get => matchLength; set => matchLength = value; }
        public string LocalFileName { get => FullFileName.Length > 0 ? Path.GetFileName(FullFileName) : ""; }
        public string FullFileName { get => fullFileName; set => fullFileName = value; }
        public int CodeStart { get => codeStart; set => codeStart = value; }
        public int CodeLength { get => codeLength; set => codeLength = value; }
    }
}
