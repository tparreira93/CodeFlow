using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using CodeFlow.ManualOperations;
using System.Text;
using CodeFlow.CodeControl;

namespace CodeFlow.SolutionOperations
{
    public class ProjectsAnalyzer : BackgroundWorker
    {
        private DifferencesAnalyzer differences;

        public ProjectsAnalyzer()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            DoWork += Analyze;
            Differences = new DifferencesAnalyzer();
        }

        public DifferencesAnalyzer Differences { get => differences; set => differences = value; }

        private void Analyze(object sender, DoWorkEventArgs e)
        {
            if (!(e.Argument is List<GenioProjectProperties>))
                return;

            List<GenioProjectProperties>  projectsList = e.Argument as List<GenioProjectProperties>;

            int count = 0, i = 1;
            foreach (GenioProjectProperties project in projectsList)
                count += project.ProjectFiles.Count;

            try
            {
                foreach (GenioProjectProperties project in projectsList)
                {
                    foreach (GenioProjectItem item in project.ProjectFiles)
                    {
                        string extension = Path.GetExtension(item.ItemPath) ?? String.Empty;
                        if (File.Exists(item.ItemPath)
                            && ((PackageOperations.ExtensionFilters.Contains(extension.ToLower()) || PackageOperations.ExtensionFilters.Contains("*"))
                            && !PackageOperations.IgnoreFilesFilters.Contains(item.ItemName.ToLower())))
                            AnalyzeFile(item.ItemPath);

                        if (CancellationPending)
                            return;
                        ReportProgress((i * 100) / count);
                        i++;
                    }
                }
            }
            catch (Exception)
            { }
        }

        private void AnalyzeFile(string file)
        {
            Encoding enc = PackageOperations.DetectTextEncoding(file, out string text);
            string code = File.ReadAllText(file, enc);

            /*Encoding unicode = Encoding.Unicode;
            byte[] encBytes = enc.GetBytes(code);
            byte[] unicodeBytes = Encoding.Convert(enc, unicode, encBytes);
            string convertedCode = unicode.GetString(unicodeBytes);*/

            List<IManual> tmp = ManuaCode.GetManualCode(code, Path.GetFileName(file));
            Differences.CheckBDDifferences(tmp, PackageOperations.GetActiveProfile());
        }
    }
}
