using CodeFlow.CodeControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.Genio
{
    [RuleProvider("IgnoreCodeRule", true)]
    class IgnoreCodeRule : MatchPatternRule
    {
        public const string IGORE_STRING = "INGNORE_THIS_CODE";

        public IgnoreCodeRule() : base(IGORE_STRING)
        {
        }

        public override bool Validate(IChange modification)
        {
            return base.Validate(modification);
        }
    }
}
