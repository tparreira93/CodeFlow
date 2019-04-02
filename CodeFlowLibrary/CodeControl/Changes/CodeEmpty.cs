using System;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Operations;

namespace CodeFlowLibrary.CodeControl.Changes
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
