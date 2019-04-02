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
        CodeFlowVersions Versions { get; private set; }
    }
}
