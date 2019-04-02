using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow
{
    static class PackageGuidList
    {
        public const string CodeFlowPackage = "23ac2f2d-5778-45dd-b5b2-5186260c958c";
        public const string CodeFlowCommandSet = "657c211f-0665-4969-81bc-d3a266b0aac4";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Guid guidComboBoxPkg = new Guid(CodeFlowPackage);
        public static readonly Guid guidComboBoxCmdSet = new Guid(CodeFlowCommandSet);
    };
}
