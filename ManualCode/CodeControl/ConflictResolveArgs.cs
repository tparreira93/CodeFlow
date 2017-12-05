using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class ConflictResolveArgs : EventArgs
    {
        private Conflict conflict;
        private Difference difference;

        public Conflict Conflict { get => conflict; set => conflict = value; }
        public Difference Keep { get => difference; set => difference = value; }
    }
}
