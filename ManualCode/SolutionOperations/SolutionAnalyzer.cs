using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlow.SolutionOperations;
using EnvDTE;

namespace CodeFlow.SolutionOperations
{
    public class ProjectsAnalyzer
    {
        private List<GenioProjectProperties> projectsList;
        private Dictionary<Guid, List<ManuaCode>> manualConflict;
        private List<IManual> ExportManual;
        private List<ManuaCode> verified;

        public ProjectsAnalyzer(List<GenioProjectProperties> projectsToAnalyze)
        {
            projectsList = projectsToAnalyze;
            verified = new List<ManuaCode>();
            ToExport = new List<IManual>();
            ManualConflict = new Dictionary<Guid, List<ManuaCode>>();
        }

        public Dictionary<Guid, List<ManuaCode>> ManualConflict { get => manualConflict; set => manualConflict = value; }
        public List<IManual> ToExport { get => ExportManual; set => ExportManual = value; }

        public void Analyze()
        {
            try
            {
                foreach (GenioProjectProperties project in projectsList)
                {
                    foreach (GenioProjectItem item in project.ProjectFiles)
                    {
                        if(PackageOperations.ExtensionFilters.Contains(Path.GetExtension(item.ItemPath))
                            || !PackageOperations.IgnoreFilesFilters.Contains(item.ItemName))
                            AnalyzeFile(item.ItemPath);
                    }
                }
            }
            catch (Exception)
            { }
        }

        private void AnalyzeFile(string file)
        {
            string code = File.ReadAllText(file);
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
                ManuaCode bd = ManuaCode.GetManual(PackageOperations.ActiveProfile, m.CodeId);
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
                    ManuaCode bd = ManuaCode.GetManual(PackageOperations.ActiveProfile, pair.Key);
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
