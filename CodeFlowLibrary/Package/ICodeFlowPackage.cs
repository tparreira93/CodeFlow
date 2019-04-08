using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Settings;
using CodeFlowLibrary.Versions;

namespace CodeFlowLibrary.Package
{
    public interface ICodeFlowPackage
    {
        List<CodeFlowVersion> PackageUpdates { get; set; }
        UserSettings Settings { get; set; }

        Task<bool> OpenOnPositionAsync(string fileName, int position);
        void SetProfile(string profileName);
        Task<bool> OpenFileAsync(string fileName);
        Task FindCodeAsync(SearchOptions searchOptions);
        void SaveSettings();
    }
}
