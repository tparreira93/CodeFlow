using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class Conflict
    {
        DifferenceList codeList = new DifferenceList();
        Guid id = Guid.Empty;

        public Conflict()
        {
        }

        public Conflict(Guid id, DifferenceList codeList)
        {
            Id = id;
            DifferenceList = codeList;
        }

        public Guid Id { get => id; set => id = value; }
        public DifferenceList DifferenceList { get => codeList; set => codeList = value; }
    }
}
