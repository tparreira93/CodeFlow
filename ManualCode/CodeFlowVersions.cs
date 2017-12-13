﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow
{
    public class CodeFlowVersions
    {
        private List<CodeFlowVersionInfo> _allVersions;

        public CodeFlowVersions()
        {
            Versions = new List<CodeFlowVersionInfo>();
            SetVersions();
        }

        private void SetVersions()
        {
            CodeFlowVersionInfo version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 3);
            version.AddChange("Fix in backoffice indexes. It was doing convertion for ManualCode obtained from the database.");
            version.AddChange("Fix in solution analysis. It now uses the correct enconding when comparing changes with database.");
            version.AddChange("Fix in profiles form. It was creating an empty profile for every profile created");
            version.AddChange("Fix in manual code extension when opened from search tool window.");
            version.AddChange("Automatic commit to database when manual code is opened from search tool window.");
            version.AddChange(new VersionChange("Added new suggestions. ALT + ENTER to use.", new DefaultCommand("LightbulbSuggestions", "true")));
            _allVersions.Add(version);


            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 4);
            version.AddChange("Fix in auto commit when manual code is opened from search tool window.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 5);
            version.AddChange("Fix in profiles form.");
            version.AddChange("Fix in command Create.");
            version.AddChange("Changed all icons to Microsof Visual Studio icons.");
            version.AddChange("New search window that uses Microsoft Visual Studio controls and commands.");
            version.AddChange("Commit, Update and Create can now be executed from context menu.");
            version.AddChange("Automatic retrieval of Genio checkout path, System name, Genio versio and current database version.");
            version.AddChange("Automatic parse of all types of routine.");
            version.AddChange("Added auto commit to Tools->Options->Genio. This options is only available for manual code that was opened from search tool window.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 6);
            version.AddChange("Fix in search window.");
            version.AddChange("Commit form only loads differences.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 7);
            version.AddChange("Added information about solution version and information about current genio profile to commit form.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 8);
            version.AddChange("Updated icons.");
            version.AddChange("Commit solution now reads files with correct encoding.");
            version.AddChange("Automatic search for infoReindex.xml in all folders and subfolders of solution.");
            version.AddChange("Fix in FNTX index for VCC++ solutions. It was removing end of line.");
            version.AddChange("Fix in profile selection. If active profile was updated, changes would not persist until it was selected again.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 9);
            version.AddChange("Support for different merge tools.");
            version.AddChange(new VersionChange("Default line endings to DOS line endings.", new DefaultCommand("ForceDOSLine", "true")));
            version.AddChange("Changed behaviour of commit form. It now commits only checked items.");
            version.AddChange("Conflict window now only allows to merge with database.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 10);
            version.AddChange("Fix in custom merge tool.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 11);
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 12);
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 13);
            version.AddChange("Bug fixes.");
            version.AddChange("Sort data in code search.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 14);
            version.AddChange("Bug fixes.");
            version.AddChange("Sort data in code search.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 0);
            version.AddChange("It is now possible to remove manual code from Genio. Empty local manual code will show as manual code deletion in commit form.");
            version.AddChange("Code that is in solution but not in genio database will now be shown as \"Not found\" in commit form.");
            version.AddChange(new VersionChange("Added option to log all commits.", new DefaultCommand("LogOperations", "true")));
            version.AddChange("Added possbility to compare, undo and redo commits.");
            version.AddChange(new VersionChange("Parallel search for manual code tags.", new DefaultCommand("MaxTaskSolutionCommit", "8")));
            version.AddChange("Refactored commit code. Operations are now defined by the type of difference. It gives more flexibility for future types of commit.");
            version.AddChange("Refactored tag match in code. All types of manual code are now registered as ManualMatchProvider. This gives more flexibility for new manual code types.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 1);
            version.AddChange("Added support for Visual Studio 2015.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 2);
            version.AddChange("[Experimental]Parallel solution commit.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 3);
            version.AddChange("Fix in experimental commit solution command.");
            version.AddChange("Keyboard controls (Enter or Space to open and Delete to remove from list) for commit form.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 5);
            VersionChange change = new VersionChange("Added option to enable or disable index fix for VCC++ solutions.", new DefaultCommand("FixIndexes", "true"));
            version.AddChange(change);
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 6);
            version.AddChange("Fix in code search. When search had '[' or ']' it acted as if it was doing a regex search.");
            version.AddChange("Keyboard controls (Enter or Space to open and Delete to remove from list) for commit form.");
            _allVersions.Add(version);
        }

        public Version Execute(string startingVersion, OptionsPageGrid options)
        {
            Version ver = new Version(startingVersion);
            Version maxVersion = ver;
            foreach (CodeFlowVersionInfo item in _allVersions)
            {
                foreach (VersionChange change in item.Changes)
                {
                    if (ver.IsBefore(item.Version) && change.Command != null)
                    {
                        change.Command.Execute(options);
                        maxVersion = item.Version;
                    }
                }
            }

            return maxVersion;
        }

        public List<CodeFlowVersionInfo> Versions { get => _allVersions; set => _allVersions = value; }
    }
}
