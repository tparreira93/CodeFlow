using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Operations;
using CodeFlowLibrary.Genio;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public class CodeCreate : ManualChange
    {
        public CodeCreate(IManual mine, Profile profile) : base(mine, null, profile)
        {
        }

        public override IOperation GetOperation()
        {
            return new CreateOperation(this, ChangeProfile);
        }
        public override string GetDescription()
        {
            return "Created";
        }
    }
}
