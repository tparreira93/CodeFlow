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
        private string ignoreFilesFilters;
        private string extFilters;
        private string useCustomTool;
        private bool lightbulbSuggestions;
        private bool parseSolutionOnStartup;
        private bool autoVCCTO2008Fix;
        private bool autoExportSaved;
        private bool forceDOSLine;

        [Category("Solution")]
        [DefaultValue(true)]
        [DisplayName("Solution parsing")]
        [Description("Parses solution for list of projects, items, client, version and system. This allows more lightbulb suggestions.")]
        public bool ParseSolutionOnStartup
        {
            get => parseSolutionOnStartup; set
            {
                parseSolutionOnStartup = value;
                PackageOperations.ParseSolution = value;
            }
        }

        [Category("Solution")]
        [DefaultValue(false)]
        [DisplayName("Auto VCC++ project fix")]
        [Description("Automatically changes VCC++ projects plataform toolset to 2008.")]
        public bool AutoVCCTO2008Fix
        {
            get => autoVCCTO2008Fix; set
            {
                autoVCCTO2008Fix = value;
                PackageOperations.AutoVCCTO2008Fix = value;
            }
        }

        [Category("Code search")]
        [DefaultValue(true)]
        [DisplayName("Auto commit saved")]
        [Description("Allows automatic commit to Genio when file from code search is saved.")]
        public bool AutoExportSaved
        {
            get => autoExportSaved; set
            {
                autoExportSaved = value;
                PackageOperations.AutoExportSaved = value;
            }
        }

        [Category("Code control")]
        [DefaultValue("")]
        [DisplayName("Merge tool")]
        [Description("Use a custom merge tool. Use the options %left is the path of genio copy, %right is the path of the working copy and %result is the merged path. All this options must be specified.")]
        public string UseCustomTool
        {
            get => useCustomTool; set
            {
                useCustomTool = value;
                PackageOperations.UseCustomTool = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(false)]
        [DisplayName("Force DOS line endings ")]
        [Description("For DOS line endings. DOS line endings use \\r\\n.")]
        public bool ForceDOSLine
        {
            get => forceDOSLine;
            set
            {
                forceDOSLine = value;
                PackageOperations.ForceDOSLine = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(true)]
        [DisplayName("Lightbulb suggestions")]
        [Description("Allows convenient usage of update, merge and commit through the visual studio lightbulb.")]
        public bool LightbulbSuggestions
        {
            get => lightbulbSuggestions; set
            {
                lightbulbSuggestions = value;
                PackageOperations.ContinuousAnalysis = value;
            }
        }

        [Category("Code control")]
        [DefaultValue(".cpp;.cs;.xml;.js")]
        [DisplayName("Extension filters")]
        [Description("Extension of files to be analyzed on solution commit. Values separeted by ';'")]
        public string ExtensionsFilters
        {
            get => extFilters;
            set
            {
                extFilters = value;
                if (value != null)
                {
                    PackageOperations.ExtensionFilters.Clear();
                    PackageOperations.ExtensionFilters.AddRange(extFilters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        [Category("Code control")]
        [DefaultValue("")]
        [DisplayName("Files to ignore")]
        [Description("Files to be ignored on solution commit. Values separeted by ';'")]
        public string IgnoreFilesFilters
        {
            get => ignoreFilesFilters; set
            {
                ignoreFilesFilters = value;
                if (value != null)
                {
                    PackageOperations.IgnoreFilesFilters.Clear();
                    PackageOperations.IgnoreFilesFilters.AddRange(ignoreFilesFilters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }
    }
}
