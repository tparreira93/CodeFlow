using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.CodeControl
{
    public class Difference
    {
        IManual local;
        IManual bd;
        bool merged = false;

        public Difference(IManual local, IManual bd)
        {
            Local = local ?? throw new ArgumentNullException(nameof(local));
            Database = bd ?? throw new ArgumentNullException(nameof(bd));
        }

        public void Merge()
        {
            Local = Manual.Merge(Database, Local);
            IsMerged = true;
        }

        public bool HasDifference()
        {
            return !Local.Code.Equals(Database.Code);
        }

        public IManual Local { get => local; set => local = value; }
        public IManual Database { get => bd; set => bd = value; }
        public bool IsMerged { get => merged; set => merged = value; }
    }
}
