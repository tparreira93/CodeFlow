using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeFlow.CodeControl;

namespace CodeFlow.CodeControl.Rules
{
    [RuleProvider("MatchPatternRule", false)]
    public class MatchPatternRule : CodeRule
    {
        public MatchPatternRule() : base()
        {
        }

        public MatchPatternRule(string pattern, bool commitDefault) : base(pattern, commitDefault)
        {
        }
        public override string Description { get => string.Format("Matches code with the following pattern {0}", Pattern); }
        
        public override object Clone()
        {
            MatchPatternRule rule = new MatchPatternRule(this.Pattern, this.CommitDefault);
            return rule;
        }

        public override bool Validate(IChange modification)
        {
            Regex regex = new Regex(Pattern);
            Match match = regex.Match(modification.Mine.Code);
            return match.Success;
        }
    }
}
