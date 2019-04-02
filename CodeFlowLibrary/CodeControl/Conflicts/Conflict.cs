using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.CodeControl.Changes;

namespace CodeFlowLibrary.CodeControl.Conflicts
{
    public class Conflict
    {
        ChangeList codeList = new ChangeList();
        Guid id = Guid.Empty;

        public Conflict()
        {
        }

        public Conflict(Guid id, ChangeList codeList)
        {
            Id = id;
            DifferenceList = codeList;
        }

        public Guid Id { get => id; set => id = value; }
        public ChangeList DifferenceList { get => codeList; set => codeList = value; }
    }
}
