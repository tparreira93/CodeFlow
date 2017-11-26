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
        private bool lightbulbSuggestions;
        private bool parseSolutionOnStartup;
        private bool autoVCCTO2008Fix;
        private bool autoExportSaved;

        [Category("Solution")]
        [DefaultValue(".cpp;.cs;.xml;.js")]
        [DisplayName("Extension filters")]
        [Description("Extension of files to be analyzed on solution export. Values separeted by ';'")]
        public string ExtensionsFilters
        {
            get => extFilters;
            set
            {
                extFilters = value;
                if (value != null)
                {
                    PackageOperations.ExtensionFilters.Clear();
                    PackageOperations.ExtensionFilters.AddRange(extFilters.Split(';'));
                }
            }
        }


        [Category("Solution")]
        [DefaultValue("")]
        [DisplayName("Files to ignore")]
        [Description("Files to be ignored on solution export. Values separeted by ';'")]
        public string IgnoreFilesFilters { get => ignoreFilesFilters; set
            {
                ignoreFilesFilters = value;
                if(value != null)
                {
                    PackageOperations.IgnoreFilesFilters.Clear();
                    PackageOperations.IgnoreFilesFilters.AddRange(ignoreFilesFilters.Split(';'));
                }
            }
        }

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

        [Category("Lightbulb")]
        [DefaultValue(true)]
        [DisplayName("Lightbulb suggestions")]
        [Description("Allows convenient usage of import, merge and export through the visual studio lightbulb.")]
        public bool LightbulbSuggestions { get => lightbulbSuggestions; set
            {
                lightbulbSuggestions = value;
                PackageOperations.ContinuousAnalysis = value;
            }
        }


        [Category("Code search")]
        [DefaultValue(true)]
        [DisplayName("Auto export saved")]
        [Description("Allows automatic export to database when file from code search is saved.")]
        public bool AutoExportSaved
        {
            get => autoExportSaved; set
            {
                autoExportSaved = value;
                PackageOperations.AutoExportSaved = value;
            }
        }
    }
}
