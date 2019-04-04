using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
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
