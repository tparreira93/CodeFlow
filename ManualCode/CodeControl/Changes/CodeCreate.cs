using CodeFlow.ManualOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlow.GenioManual;

namespace CodeFlow.CodeControl
{
    public class CodeCreate : ManualChange
    {
        public CodeCreate(IManual mine, Profile profile) : base(mine, null, profile)
        {
        }

        public override IOperation GetOperation()
        {
            return new CreateOperation(this, ChangeProfile);
        }
        public override string GetDescription()
        {
            return "Created";
        }
    }
}
