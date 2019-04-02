using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using CodeFlow.SolutionOperations;

namespace CodeFlow
{
    public class OptionsPageGrid : DialogPage
    {
        private string _ignoreFilesFilters;
        private string _extFilters;
        private string _useCustomTool;
        private bool _lightbulbSuggestions;
        private bool _parseSolutionOnStartup;
        private bool _autoVccto2008Fix;
        private bool _autoExportSaved;
        private bool _forceDosLine;
        private bool _logOperations;
        private bool _fixIndexes;
        private int _maxTaskSolutionCommit;

        [Category("Solution")]
        [DefaultValue(true)]
        [DisplayName("Solution parsing")]
        [Description("Parses solution for client, version and system. This allows more lightbulb suggestions.")]
        public bool ParseSolutionOnStartup
        {
            get => _parseSolutionOnStartup; set
            {
                _parseSolutionOnStartup = value;
                PackageOperations.Instance.ParseSolution = value;
            }
        }

        [Category("Solution")]
        [DefaultValue(false)]
        [DisplayName("Auto VCC++ project fix")]
        [Description("Automatically changes VCC++ projects plataform toolset to 2008.")]
        public bool AutoVCCTO2008Fix
        {
            get => _autoVccto2008Fix; set
            {
                _autoVccto2008Fix = value;
                PackageOperations.Instance.AutoVccto2008Fix = value;
            }
        }

        [Category("Code search")]
        [DefaultValue(true)]
        [DisplayName("Auto commit saved")]
        [Description("Allows automatic commit to Genio when file from code search is saved.")]
        public bool AutoExportSaved
        {
            get => _autoExportSaved; set
            {
                _autoExportSaved = value;
                PackageOperations.Instance.AutoExportSaved = value;
            }
        }

        [Category("Code control")]
        [DefaultValue("")]
        [DisplayName("Merge tool")]
        [Description("Use a custom merge tool. Use the options %left is the path of genio copy, %right is the path of the working copy and %result is the merged path. All this options must be specified.")]
        public string UseCustomTool
        {
            get => _useCustomTool; set
            {
                _useCustomTool = value;
                PackageOperations.Instance.UseCustomTool = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(false)]
        [DisplayName("Force DOS line endings ")]
        [Description("For DOS line endings. DOS line endings use \\r\\n.")]
        public bool ForceDOSLine
        {
            get => _forceDosLine;
            set
            {
                _forceDosLine = value;
                PackageOperations.Instance.ForceDOSLine = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(true)]
        [DisplayName("Lightbulb suggestions")]
        [Description("Allows convenient usage of update, merge and commit through the visual studio lightbulb.")]
        public bool LightbulbSuggestions
        {
            get => _lightbulbSuggestions; set
            {
                _lightbulbSuggestions = value;
                PackageOperations.Instance.ContinuousAnalysis = value;
            }
        }

        [Category("Code control")]
        [DefaultValue("*")]
        [DisplayName("Extension filters")]
        [Description("Extension of files to be analyzed on solution commit. Values separeted by ';'")]
        public string ExtensionsFilters
        {
            get => _extFilters;
            set
            {
                _extFilters = value;
                if (value != null)
                {
                    PackageOperations.Instance.ExtensionFilters.Clear();
                    PackageOperations.Instance.ExtensionFilters.AddRange(_extFilters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        [Category("Code control")]
        [DefaultValue("")]
        [DisplayName("Files to ignore")]
        [Description("Files to be ignored on solution commit. Values separeted by ';'")]
        public string IgnoreFilesFilters
        {
            get => _ignoreFilesFilters; set
            {
                _ignoreFilesFilters = value;
                if (value != null)
                {
                    PackageOperations.Instance.IgnoreFilesFilters.Clear();
                    PackageOperations.Instance.IgnoreFilesFilters.AddRange(_ignoreFilesFilters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        [Category("Code control")]
        [DefaultValue(8)]
        [DisplayName("Max number of tasks")]
        [Description("Max number of tasks when doing full solution commit.")]
        public int MaxTaskSolutionCommit
        {
            get => _maxTaskSolutionCommit; set
            {
                _maxTaskSolutionCommit = value;
                PackageOperations.Instance.MaxTaskSolutionCommit = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(true)]
        [DisplayName("Log operations")]
        [Description("Log all updates, commits and creates. It allows undo and redo of operations.")]
        public bool LogOperations
        {
            get => _logOperations; set
            {
                _logOperations = value;
                PackageOperations.Instance.LogOperations = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(true)]
        [DisplayName("Fix indexes")]
        [Description("Fix VCC++ indexes.")]
        public bool FixIndexes
        {
            get => _fixIndexes; set
            {
                _fixIndexes = value;
                PackageOperations.Instance.FixIndexes = value;
            }
        }
    }
}
