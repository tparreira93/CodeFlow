using CodeFlow.ManualOperations;
using System;

namespace CodeFlow.CodeControl
{
    public class CodeEmpty : ManualChange
    {
        public CodeEmpty(IManual mine, IManual theirs) : base(mine, theirs)
        {
        }

        public override IOperation GetOperation()
        {
            return new DeleteOperation(this);
        }
        public override string GetDescription()
        {
            return "Removed";
        }
    }
}
