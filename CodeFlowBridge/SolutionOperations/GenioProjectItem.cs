using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.SolutionOperations
{
    public class GenioProjectItem
    {
        private string itemName = "";
        private string itemPath = "";

        public GenioProjectItem()
        { }

        public GenioProjectItem(string itemName, string itemPath)
        {
            ItemName = itemName;
            ItemPath = itemPath;
        }
        public string ItemName { get => itemName; set => itemName = value; }
        public string ItemPath { get => itemPath; set => itemPath = value; }
    }
}
