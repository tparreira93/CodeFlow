using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Versions
{
    public class CodeFlowChange : ICodeFlowChange
    {
        private string _description;
        public string Description { get => _description; }


        public CodeFlowChange(string description)
        {
            _description = description;
        }
    }
}
