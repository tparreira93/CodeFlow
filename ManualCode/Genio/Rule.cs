using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlow.CodeControl;

namespace CodeFlow.Genio
{
    public abstract class Rule : IRule
    {
        protected Rule()
        {
            Pattern = "";
        }

        protected Rule(string pattern)
        {
            Pattern = pattern;
        }

        public abstract string Description { get; }

        public abstract bool Validate(IChange modification);

        public abstract bool CommitDefault { get; }

        public string Pattern { get; set; }

        public string GetRuleName()
        {
            RuleProvider provider = Utils.Util.GetAttribute<RuleProvider>(this.GetType()) as RuleProvider;
            if (provider != null)
                return provider.RuleName;
            return "";
        }
    }
}
