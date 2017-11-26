using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

namespace CodeFlow.SolutionOperations
{
    public class ProjectsAnalyzer : BackgroundWorker
    {
        private Dictionary<Guid, List<ManuaCode>> manualConflict;
        private List<IManual> ExportManual;
        private List<ManuaCode> verified;

        public ProjectsAnalyzer()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
            DoWork += Analyze;
        }

        public Dictionary<Guid, List<ManuaCode>> ManualConflict { get => manualConflict; set => manualConflict = value; }
        public List<IManual> ToExport { get => ExportManual; set => ExportManual = value; }

        private void Analyze(object sender, DoWorkEventArgs e)
        {
            if (!(e.Argument is List<GenioProjectProperties>))
                return;

            List<GenioProjectProperties>  projectsList = e.Argument as List<GenioProjectProperties>;
            verified = new List<ManuaCode>();
            ToExport = new List<IManual>();
            ManualConflict = new Dictionary<Guid, List<ManuaCode>>();

            int count = 0, i = 1;
            foreach (GenioProjectProperties project in projectsList)
                count += project.ProjectFiles.Count;

            try
            {
                foreach (GenioProjectProperties project in projectsList)
                {
                    foreach (GenioProjectItem item in project.ProjectFiles)
                    {
                        if(File.Exists(item.ItemPath) && Path.GetExtension(item.ItemPath).Length != 0
                            && ((PackageOperations.ExtensionFilters.Contains(Path.GetExtension(item.ItemPath.ToLower())) 
                                || PackageOperations.ExtensionFilters.Contains("*"))
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
            string code = File.ReadAllText(file, PackageOperations.GetFileEncoding());
            List<IManual> tmp = ManuaCode.GetManualCode(code);
            foreach (ManuaCode m in tmp)
            {
                // Já inseriu um conflito
                // Verifica
                if (ManualConflict.ContainsKey(m.CodeId))
                {
                    ManualConflict[m.CodeId].Add(m);
                    continue;
                }
                else
                {
                    // Vê se existe conflito 
                    // Verifica se já existe algum na lista a exportar
                    ManuaCode tmpM = verified.Find(x => x.CodeId.Equals(m.CodeId));
                    if (tmpM != null)
                    {
                        if (!ManualConflict.TryGetValue(m.CodeId, out List<ManuaCode> l))
                        {
                            l = new List<ManuaCode>();
                            ManualConflict.Add(m.CodeId, l);
                            l.Add(tmpM);
                        }

                        // Se existir adiciona
                        if (l != null)
                        {
                            l.Add(m);
                            continue;
                        }
                    }
                }

                //Compara com o que esta na BD
                ManuaCode bd = ManuaCode.GetManual(PackageOperations.GetActiveProfile(), m.CodeId);
                if (bd != null && !bd.Code.Equals(m.Code))
                    ToExport.Add(m);
                verified.Add(m);
            }

            List<Guid> toRemove = new List<Guid>();
            foreach (KeyValuePair<Guid, List<ManuaCode>> pair in ManualConflict)
            {
                bool keep = false;
                foreach (ManuaCode m in pair.Value)
                {
                    ManuaCode bd = ManuaCode.GetManual(PackageOperations.GetActiveProfile(), pair.Key);
                    if (bd != null && !bd.Code.Equals(m.Code))
                    {
                        keep = true;
                        break;
                    }
                }

                if (!keep)
                    toRemove.Add(pair.Key);
            }
            foreach (Guid g in toRemove)
                ManualConflict.Remove(g);
        }
    }
}
