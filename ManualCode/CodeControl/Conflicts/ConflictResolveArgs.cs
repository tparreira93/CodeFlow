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
        private IChange difference;

        public Conflict Conflict { get => conflict; set => conflict = value; }
        public IChange Keep { get => difference; set => difference = value; }
    }
}
