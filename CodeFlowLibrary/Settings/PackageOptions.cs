using CodeFlowLibrary.Solution;
using System.Collections.Generic;

namespace CodeFlowLibrary.Settings
{
    public static class PackageOptions
    {
        private static bool _showPreview;
        private static PreviewOption _showPreviewOption;

        public static bool AutoExportSaved { get; set; } = true;
        public static bool LogOperations { get; set; } = true;
        public static List<string> ExtensionFilters { get; } = new List<string>();
        public static List<string> IgnoreFilesFilters { get; } = new List<string>();
        public static bool ContinuousAnalysis { get; set; } = true;
        public static bool AutoVccto2008Fix { get; set; } = true;
        public static PreviewOption SearchPreviewOption {
            get { return _showPreviewOption; }
            set
            {
                _showPreviewOption = value;
                OnPreviewOptionChanged?.Invoke(ShowPreview, SearchPreviewOption);

            }
        }
        public static string UseCustomTool { get; set; } = "";
        public static int MaxTaskSolutionCommit { get; set; } = 8;
        public static bool ShowPreview
        {
            get { return _showPreview; }
            set
            {
                _showPreview = value;
                OnPreviewOptionChanged?.Invoke(ShowPreview, SearchPreviewOption);

            }
        }

        public delegate void ChangedPreview(bool searchPreview, PreviewOption option);
        public static event ChangedPreview OnPreviewOptionChanged;
    }
}
