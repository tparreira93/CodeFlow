using CodeFlow.CodeControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl.Rules
{
    public interface ICodeRule
    {
        bool Validate(IChange modification);

        string Description { get; }

        bool CommitDefault { get; set; }

        string Pattern { get; set; }

        string GetRuleName();
    }
}
