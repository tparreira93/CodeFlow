using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Operations;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public class CodeNotFound : ManualChange
    {
        public CodeNotFound(IManual code) : base(code, null)
        { }

        public override IOperation GetOperation()
        {
            return null;
        }
        public override bool HasDifference()
        {
            return true;
        }
        public override IChange Merge()
        {
            return this;

        }
        public override void Compare()
        { }
        public override string GetDescription()
        {
            return "Not found";
        }
    }
}
