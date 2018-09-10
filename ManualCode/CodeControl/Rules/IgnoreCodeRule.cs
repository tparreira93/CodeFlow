using CodeFlow.CodeControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl.Rules
{
    [RuleProvider("IgnoreCodeRule", true)]
    class IgnoreCodeRule : MatchPatternRule
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
