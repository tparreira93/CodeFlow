using CodeFlowLibrary.Bridge.SolutionOperations;
using CodeFlowLibrary.Util;
using CodeFlowLibrary.Solution;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VCProjectEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using CodeFlowLibrary.Package;

namespace CodeFlow.SolutionAnalyzer
{
    public class SolutionParser : ISolutionParser
    {
        public ICodeFlowPackage Flow { get; }

        public SolutionParser(ICodeFlowPackage codeflow)
        {
            Flow = codeflow;
        }

        public async Task<GenioSolutionProperties> ParseAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            CodeFlowPackage package = Flow as CodeFlowPackage;
            GenioSolutionProperties solution = await GetSolutionProjectsAsync(package.DTE);

            try
            {
                if (Directory.Exists(solution.SolutionPath))
                    solution.ClientInfo = ParseClientOptions(Path.GetDirectoryName(solution.SolutionPath));
            }
            catch (Exception)
            { }

            return solution;
        }

        public async Task ChangeToolset2008Async()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            CodeFlowPackage package = Flow as CodeFlowPackage;
            foreach (Project project in package.DTE.Solution.Projects)
            {
                CodeModel model = project.CodeModel;
                if (project.Object != null && project.Object is VCProject && GetProjectLanguage(model?.Language) == ProjectLanguage.VCCPlusPlus)
                {
                    foreach (VCConfiguration vccon in (project.Object as VCProject).Configurations)
                    {
                        IVCRulePropertyStorage generalRule = vccon.Rules.Item("ConfigurationGeneral");
                        var currentPlatformToolset = generalRule.GetUnevaluatedPropertyValue("PlatformToolset");
                        if (!currentPlatformToolset.Equals("v90"))
                            generalRule.SetPropertyValue("PlatformToolset", "v90");
                    }
                }
            }
        }

        private async Task<GenioSolutionProperties> GetSolutionProjectsAsync(DTE2 dte)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            List<GenioProjectProperties> projects = new List<GenioProjectProperties>();
            var project = dte.Solution.Projects.GetEnumerator();

            while (project.MoveNext())
            {
                if (project.Current is Project proj)
                {
                    if (proj.Kind == SolutionConstants.vsProjectKindSolutionFolder)
                        projects.AddRange(await GetProjectAsync(dte, proj));
                    else
                        projects.Add(new GenioProjectProperties(proj.Name, await GetProjectItemsAsync(proj), GetProjectLanguage(proj.CodeModel?.Language)));
                }
                else
                    continue;
            }

            return new GenioSolutionProperties(dte.DTE?.Solution?.FullName ?? "", projects);
        }

        private async Task<List<GenioProjectProperties>> GetProjectAsync(DTE2 dte, Project solution)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            List<GenioProjectProperties> projs = new List<GenioProjectProperties>();

            for (int i = 1; i <= solution.ProjectItems.Count; i++)
            {
                var proj = solution.ProjectItems.Item(i).SubProject;
                if (proj == null)
                    continue;

                else if (proj.Kind == SolutionConstants.vsProjectKindSolutionFolder)
                    projs.AddRange(await GetProjectAsync(dte, proj));

                else
                    projs.Add(new GenioProjectProperties(proj.Name, await GetProjectItemsAsync(proj), GetProjectLanguage(proj.CodeModel?.Language)));
            }

            return projs;
        }

        public ClientInfo ParseClientOptions(string solutionPath)
        {
            ClientInfo client = new ClientInfo();
            var fileList = new DirectoryInfo(solutionPath).GetFiles("infoReindex.xml", SearchOption.AllDirectories);

            if (fileList.Length != 0)
            {
                string path = fileList[0].FullName;
                if (File.Exists(path))
                {
                    client.Version = Helpers.MatchCodeDeclaration("(<Versao>)([0-9]*)(</Versao>)", 2, path);
                    client.Client = Helpers.MatchCodeDeclaration("(<Cliente>)([0-9a-zA-Z]*)(</Cliente>)", 2, path);
                    client.System = Helpers.MatchCodeDeclaration("(<Sistema>)([0-9a-zA-Z]*)(</Sistema>)", 2, path);
                }
            }
            return client;
        }

        public static ProjectLanguage GetProjectLanguage(string language)
        {
            ProjectLanguage lang;
            if (string.IsNullOrEmpty(language))
                return ProjectLanguage.Unknown;
            switch (language)
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

        private async Task<List<GenioProjectItem>> GetProjectItemsAsync(Project project)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            List<GenioProjectItem> projectItems = new List<GenioProjectItem>();
            if (project.ProjectItems == null)
                return projectItems;

            var items = project.ProjectItems.GetEnumerator();
            while (items.MoveNext())
            {
                var item = (ProjectItem)items.Current;
                projectItems.Add(await GetFilesAsync(projectItems, item));
            }

            return projectItems;
        }

        private async Task<GenioProjectItem> GetFilesAsync(List<GenioProjectItem> projectItems, ProjectItem item)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (item.ProjectItems == null)
            {
                string path = "";
                try
                {
                    path = item.FileNames[0];
                }
                catch (Exception)
                { }
                return new GenioProjectItem(item.Name, path);
            }

            var items = item.ProjectItems.GetEnumerator();
            while (items.MoveNext())
            {
                var currentItem = (ProjectItem)items.Current;
                projectItems.Add(await GetFilesAsync(projectItems, currentItem));
            }

            return new GenioProjectItem(item.Name, item.FileNames[0]);
        }

    }
}
