using System.ComponentModel;

namespace CodeFlowLibrary.Solution
{
    public enum ProjectLanguage
    {
        [Description("C#")]
        CShap,
        [Description("VB.NET")]
        VBasic,
        [Description("Visual C++")]
        VCCPlusPlus,
        [Description("Managed C++")]
        ManagedCPlusPlus,
        [Description("Unknown")]
        Unknown
    }
}
