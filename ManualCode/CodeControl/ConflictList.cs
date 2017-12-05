using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class ConflictList
    {
        List<Conflict> conflicts = new List<Conflict>();

        public ConflictList()
        {
        }

        public ConflictList(List<Conflict> conflicts)
        {
            this.AsList = conflicts ?? throw new ArgumentNullException(nameof(conflicts));
        }

        public List<Conflict> AsList { get => conflicts; set => conflicts = value; }
    }
}
