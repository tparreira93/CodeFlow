using CodeFlowLibrary.CodeControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.CodeControl.Changes;

namespace CodeFlowLibrary.CodeControl.Rules
{
    [RuleProvider("IgnoreCodeRule", true)]
    public class IgnoreCodeRule : MatchPatternRule
    {
        public const string IGNORE_STRING = "INGNORE_THIS_CODE";

        public IgnoreCodeRule() : base(IGNORE_STRING, false)
        {
        }

        public override bool Validate(IChange modification)
        {
            return base.Validate(modification);
        }
    }
}
