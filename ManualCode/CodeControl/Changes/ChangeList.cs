using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class ChangeList
    {
        List<IChange> diffs = new List<IChange>();
        public List<IChange> AsList { get => diffs; set => diffs = value; }

        public ChangeList(List<IChange> diffs)
        {
            AsList = diffs ?? throw new ArgumentNullException(nameof(diffs));
        }

        public ChangeList()
        {
        }
    }
}
