using System;
using System.Collections.Generic;
using CodeFlow.GenioManual;
using CodeFlow.ManualOperations;

namespace CodeFlow.CodeControl.Analyzer
{
    public class ChangeAnalyzer
    {
        private ConflictList manualConflict;
        private ChangeList differences;

        public ChangeAnalyzer()
        {
            this.manualConflict = new ConflictList();
            this.differences = new ChangeList();
        }

        public ConflictList ManualConflict { get => manualConflict; }
        public ChangeList Differences { get => differences; }

        public void CheckBDDifferences(IManual toCheck, Profile profile)
        {
            CheckBDDifferences(new List<IManual>() { toCheck }, profile);
        }

        public void CheckBDDifferences(List<IManual> toCheck, Profile profile)
        {
            foreach (IManual m in toCheck)
            {
                IManual bd = Manual.GetManual(m.GetType(), m.CodeId, profile);

                //Compara com o que esta na BD
                if(bd == null)
                {
                    // Codigo foi apagado por isso criamos um vazio
                    CodeNotFound diff = new CodeNotFound(m);
                    Differences.AsList.Add(diff);
                }
                else
                {
                    IChange change = null;
                    if (String.IsNullOrWhiteSpace(m.Code))
                        change = new CodeEmpty(m, bd);

                    else if (!bd.Code.Equals(m.Code))
                        change = new CodeChange(m, bd);

                    else
                        continue;

                    IChange diff = Differences.AsList.Find(x => x.Mine.CodeId.Equals(m.CodeId));

                    if (diff != null)
                    {
                        Differences.AsList.Remove(diff);
                        ManualConflict.AsList.Add(new Conflict(diff.Mine.CodeId, new ChangeList(new List<IChange>() { change, diff })));
                    }

                    // Já inseriu um conflito
                    // Verifica
                    else if (ManualConflict.AsList.Find(x => x.Id.Equals(m.CodeId)) is Conflict conflict)
                        conflict.DifferenceList.AsList.Add(change);

                    else
                        Differences.AsList.Add(change);
                }
            }
        }
    }
}
