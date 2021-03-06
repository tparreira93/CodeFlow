﻿using CodeFlow.CodeControl;
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
        
        public CodeRule ValidateRules(Profile profile, IChange change)
        {
            List<CodeRule> rulesToValidate = new List<CodeRule>();
            rulesToValidate.AddRange(profile.ProfileRules);
            rulesToValidate.AddRange(GetDefaultRules());

            foreach (var rule in rulesToValidate)
            {
                if (rule.Validate(change))
                    return rule;
            }

            return null;
        }

        public List<CodeRule> GetDefaultRules()
        {
            Dictionary<RuleProvider, Type> providers = Util.GetAtrributes<RuleProvider>();
            List<Type> types = providers.Where(entry => entry.Key.IsDefaultType).Select(entry => entry.Value).ToList();
            List<CodeRule> rules = new List<CodeRule>();

            foreach (var item in types)
            {
                ConstructorInfo constructor = item.GetConstructor(new Type[] { });
                CodeRule r = constructor?.Invoke(null) as CodeRule;
                rules.Add(r);
            }

            return rules;
        }
    }
}
