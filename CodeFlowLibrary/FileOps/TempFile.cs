using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.FileOps
{
    public class TempFile
    {
        public TempFile(string fullFilePath, string fileName, bool autoExport)
        {
            AutoExport = autoExport;
            FullFilePath = fullFilePath;
            FileName = fileName;
        }

        public bool AutoExport { get; set; }
        public string FullFilePath { get; set; }
        public string FileName { get; set; }
    }
}
