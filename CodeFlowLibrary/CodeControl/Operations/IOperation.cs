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
        bool Undo();
        bool Redo();
        bool Execute();
        IChange OperationChanges { get; }
        DateTime OperationTime { get; }
        string FullFileName { get; }
        string LocalFileName { get; }
        string OperationType { get; }
        Profile OperationProfile { get; }
    }
}
