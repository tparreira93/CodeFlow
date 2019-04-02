using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Operations;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public class CodeChange : ManualChange
    {
        public CodeChange(IManual mine, IManual theirs) : base(mine, theirs)
        {

        }

        public override string GetDescription()
        {
            return "Modified";
        }

        public override IOperation GetOperation()
        {
            return new ChangeOperation(this);
        }
    }
}
