using CodeFlowLibrary.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Versions
{
    public class VersionChecker
    {
        public UserSettings Settings { get; set; }
        public List<CodeFlowVersion> Updates { get; set; }

        public VersionChecker(UserSettings settings, List<CodeFlowVersion> updates)
        {
            Settings = settings;
            Updates = updates;
        }

        public bool CheckVersion()
        {
            CodeFlowUpdater updater = new CodeFlowUpdater(Updates);
            Version newVersion = updater.Update(Settings.ToolVersion);
            if (Settings.ToolVersion.CompareTo(newVersion) != 0)
            {
                Settings.OldVersion = Settings.ToolVersion;
                Settings.ToolVersion = newVersion;
                if (!Settings.OldVersion.Equals(Settings.ToolVersion))
                    return true;

            }

            return false;
        }
    }
}
