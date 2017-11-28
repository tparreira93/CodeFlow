using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.ManualOperations
{
    public class DifferencesAnalyzer
    {
        private Dictionary<Guid, List<ManuaCode>> manualConflict;
        private List<IManual> differences;
        private List<ManuaCode> verified;

        public DifferencesAnalyzer()
        {
            this.manualConflict = new Dictionary<Guid, List<ManuaCode>>();
            this.differences = new List<IManual>();
            this.verified = new List<ManuaCode>();
        }

        public Dictionary<Guid, List<ManuaCode>> ManualConflict { get => manualConflict; }
        public List<IManual> Differences { get => differences; }

        public void CheckBDDifferences(IManual toCheck)
        {
            CheckBDDifferences(new List<IManual>() { toCheck });
        }

        public void CheckBDDifferences(List<IManual> toCheck)
        {
            foreach (ManuaCode m in toCheck)
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
                    Differences.Add(m);
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
