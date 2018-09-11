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
        private ProjectItem item = null;

        public GenioProjectItem()
        { }

        public GenioProjectItem(ProjectItem item, string itemName, string itemPath)
        {
            Item = item;
            ItemName = itemName;
            ItemPath = itemPath;
        }

        public GenioProjectItem(ProjectItem item)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            Item = item;
            try
            {
                ItemName = item.Name;
                ItemPath = item.FileNames[0];
            }
            catch(Exception)
            {  }
        }

        public ProjectItem Item { get => item; set => item = value; }
        public string ItemName { get => itemName; set => itemName = value; }
        public string ItemPath { get => itemPath; set => itemPath = value; }
    }
}
