using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Package;
using CodeFlowLibrary.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.FileOps
{
    public class FilesManager
    {
        public static ICodeFlowPackage Flow { get; set; }
        private readonly List<string> _openFiles = new List<string>();
        private Dictionary<string, Type> AutoExportFiles { get; } = new Dictionary<string, Type>();
        private readonly List<TempFile> openFiles = new List<TempFile>();


        private void AddTempFile(string file)
        {
            if (FindTempFile(file) != null)
                openFiles.Add(new TempFile(Path.GetFileName(file), file, false));
        }
        private void AddTempFile(TempFile file)
        {
            if (FindTempFile(file.FullFilePath) != null)
                openFiles.Add(file);
        }

        private TempFile FindTempFile(string file)
        {
            return openFiles.Find(x => string.Compare(x.FullFilePath, file, true) == 0);
        }
        private void DeleteFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }
        public void Remove(string file)
        {
            TempFile tmp = FindTempFile(file);
            if (tmp == null)
                return;

            openFiles.Remove(tmp);

            DeleteFile(tmp.FullFilePath);
        }
        public bool IsTempFile(string file)
        {
            return FindTempFile(file) != null;
        }
        public void RemoveAll()
        {
            foreach (var file in openFiles)
                DeleteFile(file.FullFilePath);

            openFiles.Clear();
        }

        public void OpenTempFile(IManual man, Profile profile, bool autoExport)
        {
            var fileName = man.CodeId.ToString();
            var tmp = $"{Path.GetTempPath()}{man.CodeId}.{man.GetCodeExtension(profile)}";
            File.WriteAllText(tmp, man.ToString(), Encoding.UTF8);

            TempFile tempFile = new TempFile(tmp, fileName, autoExport);
            if (Flow.OpenFileAsync(tmp).Result)
                AddTempFile(tempFile);
        }
        public bool IsAutoExportManual(string path)
        {
            return FindTempFile(path)?.AutoExport ?? false;
        }
        public List<IManual> GetAutoExportIManual(string path)
        {
            List<IManual> man = null;
            if (IsAutoExportManual(path))
            {
                Helpers.DetectTextEncoding(path, out string code);
                string fileName = Path.GetFileName(path);
                try
                {
                    man = new VSCodeManualMatcher(code, fileName).Match();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return man;
        }
    }
}
