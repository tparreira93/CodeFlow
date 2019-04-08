using CodeFlow.ManualOperations;
using System;
using CodeFlow.GenioManual;

namespace CodeFlow.CodeControl
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
