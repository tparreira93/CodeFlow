using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Operations;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public class CodeCreate : ManualChange
    {
        public CodeCreate(IManual mine) : base(mine, null)
        {
        }

        public override IOperation GetOperation()
        {
            return new CreateOperation(this);
        }
        public override string GetDescription()
        {
            return "Created";
        }
    }
}
