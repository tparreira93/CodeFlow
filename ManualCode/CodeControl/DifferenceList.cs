using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class DifferenceList
    {
        List<Difference> diffs = new List<Difference>();
        public List<Difference> AsList { get => diffs; set => diffs = value; }

        public DifferenceList(List<Difference> diffs)
        {
            AsList = diffs ?? throw new ArgumentNullException(nameof(diffs));
        }

        public DifferenceList()
        {
        }
    }
}
