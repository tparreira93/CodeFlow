using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class DifferencesAnalyzer
    {
        private ConflictList manualConflict;
        private DifferenceList differences;

        public DifferencesAnalyzer()
        {
            this.manualConflict = new ConflictList();
            this.differences = new DifferenceList();
        }

        public ConflictList ManualConflict { get => manualConflict; }
        public DifferenceList Differences { get => differences; }

        public void CheckBDDifferences(IManual toCheck, Profile profile)
        {
            CheckBDDifferences(new List<IManual>() { toCheck }, profile);
        }

        public void CheckBDDifferences(List<IManual> toCheck, Profile profile)
        {
            foreach (IManual m in toCheck)
            {
                var t = m.GetType();
                IManual bd = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { profile, m.CodeId }) as IManual;
                //Compara com o que esta na BD

                if (bd != null && !bd.Code.Equals(m.Code))
                {
                    Difference diff = Differences.AsList.Find(x => x.Local.CodeId.Equals(m.CodeId));

                    if (diff != null)
                    {
                        Differences.AsList.Remove(diff);
                        ManualConflict.AsList.Add(new Conflict(diff.Local.CodeId, 
                            new DifferenceList(new List<Difference>() { new Difference(m, bd), new Difference(diff.Local, bd) })));
                    }

                    // Já inseriu um conflito
                    // Verifica
                    else if (ManualConflict.AsList.Find(x => x.Id.Equals(m.CodeId)) is Conflict conflict)
                        conflict.DifferenceList.AsList.Add(new Difference(m, bd));

                    else
                        Differences.AsList.Add(new Difference(m, bd));
                }
            }
        }
    }
}
