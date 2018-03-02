using System.Collections.Generic;

namespace CodeFlow.Versions
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

            version.Version = new Version(0, 1, 0);
            version.AddChange("Initial release.");
            version.AddChange("Merge and commit of manual code.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(0, 2, 0);
            version.AddChange("Added possibility to merge and commit all manual code in selection.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(0, 3, 0);
            version.AddChange("Added option to commit all solution files.");
            version.AddChange("New window for merge and commit of manual code.");
            version.AddChange("Multiple profiles for Genio checkouts.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(0, 4, 0);
            version.AddChange("Added option to create manual code in database.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(0, 5, 0);
            version.AddChange("Added option to update manual code in solution.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(1, 0, 0);
            version.AddChange("Update UI. Added toolbar container for all commands. (View -> Toolbars -> Code Flow).");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(1, 1, 0);
            version.AddChange("Added search tool for manual code in Genio database.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(1, 1, 1);
            version.AddChange("Fix in search tool.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(1, 1, 2);
            version.AddChange("Bug fix for empty code tags matching.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 0, 0);
            version.AddChange("Updated UI. Update forms and new icons.");
            version.AddChange("Auto replace of IXN indexes to /[FTNX ..]/ indexes in VCC++ solutions.");
            version.AddChange(new VersionChange("Added lightbulb options. It allows comparison, merge and commit.", grid => grid.LightbulbSuggestions = true));
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 1, 0);
            version.AddChange("Bug fixes.");
            version.AddChange(new VersionChange("Automatic parse of solution files (client, version and system name).", grid =>
            {
                grid.ExtensionsFilters = "*";
                return true;
            }));
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 2, 0);
            version.AddChange("Fix in backoffice indexes. It was doing convertion for ManualCode obtained from the database.");
            version.AddChange("Fix in solution commit. It now uses the correct enconding when comparing changes with database.");
            version.AddChange("Fix in profiles form. It was creating an empty profile for every profile created.");
            version.AddChange("Fix in manual code extension when opened from search tool window.");
            version.AddChange(new VersionChange("Added auto commit to Tools->Options->Genio. This options is only available for manual code that was opened from search tool window.", grid => grid.AutoExportSaved = true));
            version.AddChange("Added open tortoise svn suggestion. ALT + ENTER to use.");
            _allVersions.Add(version);


            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 2, 1);
            version.AddChange("Fix in auto commit when manual code is opened from search tool window.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 3, 0);
            version.AddChange("Fix in profiles form.");
            version.AddChange("Fix in command Create.");
            version.AddChange("Changed all icons to Microsof Visual Studio icons.");
            version.AddChange("New search window that uses Microsoft Visual Studio controls and commands.");
            version.AddChange("Commit, Update and Create can now be executed from context menu.");
            version.AddChange("Automatic retrieval of Genio checkout path, System name, Genio versio and current database version.");
            version.AddChange("Automatic parse of all types of routine.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 3, 1);
            version.AddChange("Fix in search window.");
            version.AddChange("Commit form only loads differences.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 4, 0);
            version.AddChange("Added information about solution version and information about current genio profile to commit form.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 5, 0);
            version.AddChange("Updated icons.");
            version.AddChange("Commit solution now reads files with correct encoding.");
            version.AddChange("Automatic search for infoReindex.xml in all folders and subfolders of solution.");
            version.AddChange("Fix in FNTX index for VCC++ solutions. It was removing end of line.");
            version.AddChange("Fix in profile selection. If active profile was updated, changes would not persist until it was selected again.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 6, 0);
            version.AddChange("Support for different merge tools.");
            version.AddChange(new VersionChange("Default line endings to DOS line endings.", grid => grid.ForceDOSLine = true));
            version.AddChange("Changed behaviour of commit form. It now commits only checked items.");
            version.AddChange("Conflict window now only allows to merge with database.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 6, 1);
            version.AddChange("Fix in custom merge tool.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 6, 2);
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 6, 3);
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(2, 6, 4);
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 0, 0);
            version.AddChange("Sort data in code search.");
            version.AddChange("It is now possible to remove manual code from Genio. Empty local manual code will show as manual code deletion in commit form.");
            version.AddChange("Code that is in solution but not in genio database will now be shown as \"Not found\" in commit form.");
            version.AddChange(new VersionChange("Added option to log all commits.", grid => grid.LogOperations = true));
            version.AddChange("Added possbility to compare, undo and redo commits.");
            version.AddChange(new VersionChange("Parallel search for manual code tags.", grid =>
            {
                grid.MaxTaskSolutionCommit = 8;
                return true;
            }));
            version.AddChange("Refactored commit code. Operations are now defined by the type of difference. It gives more flexibility for future types of commit.");
            version.AddChange("Refactored tag match in code. All types of manual code are now registered as ManualMatchProvider. This gives more flexibility for new manual code types.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 1, 0);
            version.AddChange("Added support for Visual Studio 2015.");
            version.AddChange("[Experimental]Parallel solution commit.");
            version.AddChange("Keyboard controls (Enter or Space to open and Delete to remove from list) for commit form.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 1, 1);
            version.AddChange("Fix in experimental parallel solution commit.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 2, 0);
            VersionChange change = new VersionChange("Added option to enable or disable index fix for VCC++ solutions.", grid => grid.FixIndexes = true);
            version.AddChange(change);
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 2, 1);
            version.AddChange("Fix in code search. When search had '[' or ']' it acted as if it was doing a regex search.");
            version.AddChange("Fix in plataform toolset retarget. Due to incompatibility with project dll versions between visual studio 2015 and 2017, plataform retarget will only work for visual studio 2017 projects.");
            version.AddChange("This version breaks visual studio 2015 compatibility when visual studio 2017 is not installed because of dll incompatibility of visual studio projects.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 3, 0);
            version.AddChange("New form with all updates to CodeFlow version.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 3, 1);
            version.AddChange("Added missing icons to CodeFlow updates form.");
            version.AddChange("Removed Visual Studio 2015 compatibility.");
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 3, 2);
            version.AddChange("Fix in commit form merge.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 4, 0);
            version.AddChange("Added form with help for plataform and type for code creation.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 4, 1);
            version.AddChange("Fix in plataform and type selection for \"Create in genio\".");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 4, 2);
            version.AddChange("Bug fixes.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 5, 0);
            version.AddChange("It is now possible to show SVN log of search items.");
            version.AddChange("It is now possible to clear search results.");
            version.AddChange("Code search now includes parameter for manual code.");
            _allVersions.Add(version);
            
            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 6, 0);
            version.AddChange("It is now possible to SVN Blame manual code.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 7, 0);
            version.AddChange("Improved messages when errors occur.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 8, 0);
            version.AddChange("Asynchronous profile selection. It improves system responsiveness on start-up.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 9, 0);
            version.AddChange("Clear of previous search results before new search.");
            _allVersions.Add(version);

            version = new CodeFlowVersionInfo();
            version.Version = new Version(3, 10, 0);
            //version.AddChange("It is now possible to go to change position from commit form.");
            version.AddChange("Tags are now ordered in creation form.");
            _allVersions.Add(version);
        }

        public Version Execute(Version startingVersion, OptionsPageGrid options)
        {
            Version maxVersion = startingVersion;
            options.LoadSettingsFromStorage();
            foreach (CodeFlowVersionInfo item in _allVersions)
            {
                foreach (VersionChange change in item.Changes)
                {
                    if (startingVersion.IsBefore(item.Version))
                    {
                        if(change.Command != null)
                            change.Execute(options);
                        maxVersion = item.Version;
                    }
                }
            }
            options.SaveSettingsToStorage();

            return maxVersion;
        }

        public List<CodeFlowVersionInfo> Versions { get => _allVersions; set => _allVersions = value; }
    }
}
