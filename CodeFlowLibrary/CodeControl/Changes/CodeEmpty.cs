using System;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Operations;
using CodeFlowLibrary.Genio;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public class CodeEmpty : ManualChange
    {
        public CodeEmpty(IManual mine, IManual theirs, Profile profile) : base(mine, theirs, profile)
        {
        }

        public override IOperation GetOperation()
        {
            return new DeleteOperation(this, ChangeProfile);
        }
        public override string GetDescription()
        {
            return "Removed";
        }
    }
}
