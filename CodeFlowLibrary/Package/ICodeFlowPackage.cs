using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.CodeControl.Operations;
using CodeFlowLibrary.FileOps;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Settings;
using CodeFlowLibrary.Versions;

namespace CodeFlowLibrary.Package
{
    public interface ICodeFlowPackage
    {
        List<CodeFlowVersion> PackageUpdates { get; set; }
        InternalSettings Settings { get; set; }
        FilesManager FileOps { get; set; }
        Profile Active { get; }
        string SearchPreviewFile { get; }
        Task<bool> OpenOnPositionAsync(string fileName, int position);
        void SetProfile(string profileName);
        void LoadProfile(string profileName);
        bool OpenFile(string fileName);
        Task<bool> OpenFileAsync(string fileName);
        Task FindCodeAsync(SearchOptions searchOptions);
        void SaveSettings();
        bool ExecuteOperation(IOperation operation);
        object GetService(Type t);
    }
}
