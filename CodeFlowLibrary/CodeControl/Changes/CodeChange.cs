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
    public class CodeChange : ManualChange
    {
        public CodeChange(IManual mine, IManual theirs, Profile profile) : base(mine, theirs, profile)
        {

        }

        public override string GetDescription()
        {
            return "Modified";
        }

        public override IOperation GetOperation()
        {
            return new ChangeOperation(this, ChangeProfile);
        }
    }
}
