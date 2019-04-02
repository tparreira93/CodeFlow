using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.CodeControl.Changes;

namespace CodeFlowLibrary.CodeControl.Operations
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
