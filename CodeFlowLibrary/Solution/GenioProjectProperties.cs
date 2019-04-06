using System.Collections.Generic;

namespace CodeFlowLibrary.Solution
{
    public class GenioProjectProperties
    {        
        public List<GenioProjectItem> ProjectFiles { get; }
        public string ProjectName { get; }
        public ProjectLanguage ProjectLang { get; }

        public GenioProjectProperties(string project, List<GenioProjectItem> projectFiles, ProjectLanguage lang)
        {
            ProjectName = project;
            ProjectLang = lang;
            ProjectFiles = projectFiles;
        }

        public override string ToString()
        {
            return ProjectName + " - " + Util.Helpers.GetDescription(ProjectLang);
        }
    }
}
