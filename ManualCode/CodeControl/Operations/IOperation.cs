using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public interface IOperation
    {
        bool Undo(Profile profile);
        bool Redo(Profile profile);
        bool Execute(Profile profile);
        IChange OperationChanges { get; set; }
        DateTime OperationTime { get; set; }
        string LocalFileName { get; }
        string OperationType { get;  }
    }
}
