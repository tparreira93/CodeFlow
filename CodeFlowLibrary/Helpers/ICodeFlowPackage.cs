using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Helpers
{
    public interface ICodeFlowPackage
    {
        bool OpenOnPosition(string fileName, int position);
        void SetProfile(string profileName);
        System.Threading.Tasks.Task<bool> OpenFileAsync(string fileName);
    }
}
