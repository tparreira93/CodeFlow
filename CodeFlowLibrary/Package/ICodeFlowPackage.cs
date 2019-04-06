using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.Settings;

namespace CodeFlowLibrary.Package
{
    public interface ICodeFlowPackage
    {
        bool OpenOnPosition(string fileName, int position);
        void SetProfile(string profileName);
        Task<bool> OpenFileAsync(string fileName);
        Task FindCodeAsync(SearchOptions searchOptions);
    }
}
