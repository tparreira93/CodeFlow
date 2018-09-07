using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeFlow.CodeControl;

namespace CodeFlow.Genio
{
    [RuleProvider("MatchPatternRule", false)]
    public class MatchPatternRule : Rule
    {
        public MatchPatternRule() : base()
        {
        }

        public MatchPatternRule(string pattern) : base(pattern)
        {
        }
        public override string Description { get => string.Format("Matches code with the following pattern {0}", Pattern); }

        public override bool CommitDefault => false;

        public override bool Validate(IChange modification)
        {
            Regex regex = new Regex(Pattern);
            Match match = regex.Match(modification.Mine.Code);
            return match.Success;
        }
    }
}
