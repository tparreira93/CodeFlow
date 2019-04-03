using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Versions
{
    public interface ICodeFlowChangeCommand
    {
        bool Execute();
    }
}
