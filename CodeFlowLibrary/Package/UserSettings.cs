using CodeFlowLibrary.Genio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Package
{
    public class UserSettings
    {
        public List<Profile> Profiles { get; set; } = new List<Profile>();
        public Versions.Version ToolVersion { get; set; } = new Versions.Version("");
        public Versions.Version OldVersion { get; set; } = new Versions.Version("");
        public Profile LastActiveProfile { get; set; } = new Profile();
    }
}
