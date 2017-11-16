using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.SolutionOperations
{
    public class GenioProjectProperties
    {
        Project genioProject;
        string projectName = "";
        ProjectLanguage projectLang = ProjectLanguage.VCCPlusPlus;
        List<GenioProjectItem> projectFiles = new List<GenioProjectItem>();

        public Project GenioProject { get => genioProject; set => genioProject = value; }
        public List<GenioProjectItem> ProjectFiles { get => projectFiles; set => projectFiles = value; }
        public string ProjectName { get => projectName; set => projectName = value; }
        public ProjectLanguage ProjectLang { get => projectLang; set => projectLang = value; }

        public GenioProjectProperties(Project project, bool loadFiles)
        {
            GenioProject = project;
            ProjectName = project.Name;
            ProjectLang = GetProjectLanguage(GenioProject);
            if(loadFiles)
                ProjectFiles = GetProjectItems(GenioProject);
        }

        public GenioProjectProperties(Project genioProject, List<GenioProjectItem> projectFiles)
        {
            GenioProject = genioProject;
            ProjectFiles = projectFiles;
        }

        public static ProjectLanguage GetProjectLanguage(Project proj)
        {
            ProjectLanguage lang;
            CodeModel model = proj.CodeModel;
            if (model == null)
                return ProjectLanguage.Unknown;
            switch (model.Language)
            {
                case CodeModelLanguageConstants.vsCMLanguageCSharp:
                    lang = ProjectLanguage.CShap;
                    break;
                case CodeModelLanguageConstants.vsCMLanguageVB:
                    lang = ProjectLanguage.VBasic;
                    break;
                case CodeModelLanguageConstants.vsCMLanguageVC:
                    lang = ProjectLanguage.VCCPlusPlus;
                    break;
                case CodeModelLanguageConstants.vsCMLanguageMC:
                    lang = ProjectLanguage.ManagedCPlusPlus;
                    break;
                default:
                    lang = ProjectLanguage.Unknown;
                    break;
            }
            return lang;
        }
        private static List<GenioProjectItem> GetProjectItems(Project project)
        {
            List<GenioProjectItem> projectItems = new List<GenioProjectItem>();
            if (project.ProjectItems == null)
                return projectItems;

            var items = project.ProjectItems.GetEnumerator();
            while (items.MoveNext())
            {
                var item = (ProjectItem)items.Current;
                projectItems.Add(GetFiles(projectItems, item));
            }

            return projectItems;
        }
        private static GenioProjectItem GetFiles(List<GenioProjectItem> projectItems, ProjectItem item)
        {
            if (item.ProjectItems == null)
            {
                string path = "";
                try
                {
                    path = item.FileNames[0];
                }
                catch(Exception)
                { }
                return new GenioProjectItem(item, item.Name, path);
            }

            var items = item.ProjectItems.GetEnumerator();
            while (items.MoveNext())
            {
                var currentItem = (ProjectItem)items.Current;
                projectItems.Add(GetFiles(projectItems, currentItem));
            }

            return new GenioProjectItem(item, item.Name, item.FileNames[0]);
        }

        public override string ToString()
        {
            return ProjectName + " - " + Utils.Util.GetDescription(ProjectLang);
        }

        public enum ProjectLanguage
        {
            [Description("C#")]
            CShap,
            [Description("VB.NET")]
            VBasic,
            [Description("Visual C++")]
            VCCPlusPlus,
            [Description("Managed C++")]
            ManagedCPlusPlus,
            [Description("Unknown")]
            Unknown
        }
    }
}
