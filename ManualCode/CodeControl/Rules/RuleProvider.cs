using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl.Rules
{
    public class RuleProvider : Attribute
    {
        public string RuleName { get; set; }
        public bool IsDefaultType { get; set; }

        public RuleProvider(string ruleName, bool autoRule)
        {
            RuleName = ruleName;
            IsDefaultType = autoRule;
        }
    }
}
