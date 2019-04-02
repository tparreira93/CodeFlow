using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public class ChangeList
    {
        public List<IChange> AsList { get; set; } = new List<IChange>();

        public ChangeList(List<IChange> diffs)
        {
            AsList = diffs ?? throw new ArgumentNullException(nameof(diffs));
        }

        public ChangeList()
        {
        }
    }
}
