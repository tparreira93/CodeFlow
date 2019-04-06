using System.Collections.Generic;

namespace CodeFlowLibrary.Settings
{
    public static class PackageOptions
    {
        public static bool AutoExportSaved { get; set; } = true;
        public static bool LogOperations { get; set; } = true;
        public static List<string> ExtensionFilters { get; } = new List<string>();
        public static List<string> IgnoreFilesFilters { get; } = new List<string>();
        public static bool ParseSolution { get; set; } = false;
        public static bool ContinuousAnalysis { get; set; } = true;
        public static bool AutoVccto2008Fix { get; set; } = true;
        public static string UseCustomTool { get; set; } = "";
        public static int MaxTaskSolutionCommit { get; set; } = 8;
    }
}
