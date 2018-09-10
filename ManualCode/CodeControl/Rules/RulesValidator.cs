using CodeFlow.CodeControl;
using CodeFlow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl.Rules
{
    public class RulesValidator
    {
        public RulesValidator()
        {
        }
        
        public ICodeRule ValidateRules(Profile profile, IChange change)
        {
            List<ICodeRule> rulesToValidate = new List<ICodeRule>();
            rulesToValidate.AddRange(profile.ProfileRules);
            rulesToValidate.AddRange(GetDefaultRules());

            foreach (var rule in rulesToValidate)
            {
                if (rule.Validate(change))
                    return rule;
            }

            return null;
        }

        public List<ICodeRule> GetDefaultRules()
        {
            Dictionary<RuleProvider, Type> providers = Util.GetAtrributes<RuleProvider>();
            List<Type> types = providers.Where(entry => entry.Key.IsDefaultType).Select(entry => entry.Value).ToList();
            List<ICodeRule> rules = new List<ICodeRule>();

            foreach (var item in types)
            {
                ConstructorInfo constructor = item.GetConstructor(new Type[] { });
                ICodeRule r = constructor?.Invoke(null) as ICodeRule;
                rules.Add(r);
            }

            return rules;
        }
    }
}
