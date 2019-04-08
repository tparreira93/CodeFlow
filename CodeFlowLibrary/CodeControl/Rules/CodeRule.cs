using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CodeFlowLibrary.CodeControl.Changes;
using CodeFlowLibrary.Util;

namespace CodeFlowLibrary.CodeControl.Rules
{
    [Serializable]
    [XmlInclude(typeof(MatchPatternRule))]
    [XmlInclude(typeof(IgnoreCodeRule))]
    public abstract class CodeRule : ICodeRule, ICloneable
    {
        private string pattern;
        protected bool commitDefault;

        protected CodeRule()
        {
            Pattern = "";
            commitDefault = false;
        }

        protected CodeRule(string pattern, bool commitDefault)
        {
            Pattern = pattern;
            this.commitDefault = commitDefault;
        }

        public abstract string Description { get; }

        public abstract bool Validate(IChange modification);

        public bool CommitDefault { get => commitDefault; set => commitDefault = value; }

        public string Pattern { get => pattern; set => pattern = value; }

        public string GetRuleName()
        {
            RuleProvider provider = Helpers.GetAttribute<RuleProvider>(this.GetType()) as RuleProvider;
            if (provider != null)
                return provider.RuleName;
            return "";
        }

        public abstract object Clone();
    }
}
