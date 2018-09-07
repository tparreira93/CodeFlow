using CodeFlow.CodeControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.Genio
{
    public interface IRule
    {
        bool Validate(IChange modification);

        string Description { get; }

        bool CommitDefault { get; }

        string Pattern { get; set; }

        string GetRuleName();
    }
}
