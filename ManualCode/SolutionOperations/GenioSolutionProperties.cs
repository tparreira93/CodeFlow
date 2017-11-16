﻿using CodeFlow.Utils;
using EnvDTE;
using Microsoft.VisualStudio.VCProjectEngine;
using System.Collections.Generic;
using System.IO;

namespace CodeFlow.SolutionOperations
{
    public class GenioSolutionProperties
    {
        List<GenioProjectProperties> genioProjects;
        ClientInfo clientInfo = new ClientInfo();

        public static List<GenioProjectProperties> SavedFiles = new List<GenioProjectProperties>();

        public GenioSolutionProperties()
        {
            GenioProjects = new List<GenioProjectProperties>();
        }

        public GenioSolutionProperties(List<GenioProjectProperties> projects)
        {
            GenioProjects = projects;
        }
        
        public List<GenioProjectProperties> GenioProjects { get => genioProjects; set => genioProjects = value; }
        internal ClientInfo ClientInfo { get => clientInfo; set => clientInfo = value; }

        public static GenioSolutionProperties ParseSolution(DTE dte, bool loadFiles = false)
        {
            GenioSolutionProperties properties = new GenioSolutionProperties();

            properties.GenioProjects = GetProjects(dte, loadFiles);
            properties.ClientInfo = ParseClientOptions(dte.Solution != null ? System.IO.Path.GetDirectoryName(dte.Solution.FullName) : "");

            return properties;
        }

        public static void ChangeToolset2008(DTE dte)
        {
            foreach (Project project in dte.Solution.Projects)
            {
                if (project.Object is VCProject
                    && GenioProjectProperties.GetProjectLanguage(project) == GenioProjectProperties.ProjectLanguage.VCCPlusPlus)
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

        public static ClientInfo ParseClientOptions(string solutionPath)
        {
            ClientInfo client = new ClientInfo();

            string path = "";
            string p = $"{solutionPath}\\infoReindex.xml";
            if (File.Exists(p))
                path = p;
            else if (File.Exists(p = $"{solutionPath}\\Qweb3\\Interface\\Bin\\GIP_ReIdx\\infoReindex.xml"))
                path = p;

            if (path.Length != 0)
            {
                client.Version = Util.MatchCodeDeclaration("(<Versao>)([0-9]*)(</Versao>)", 2, path);
                client.Client = Util.MatchCodeDeclaration("(<Cliente>)([0-9a-zA-Z]*)(</Cliente>)", 2, path);
                client.System = Util.MatchCodeDeclaration("(<Sistema>)([0-9a-zA-Z]*)(</Sistema>)", 2, path);
            }
            return client;
        }

        private static List<GenioProjectProperties> GetProjects(DTE dte, bool loadFiles)
        {
            List<GenioProjectProperties> projects = new List<GenioProjectProperties>();
            var project = dte.Solution.Projects.GetEnumerator();

            while (project.MoveNext())
            {
                Project proj = project.Current as Project;
                if (proj == null)
                    continue;
                else if (proj.Kind == SolutionConstants.vsProjectKindSolutionFolder)
                    projects.AddRange(GetProjects(dte, proj, loadFiles));
                else
                    projects.Add(new GenioProjectProperties(proj, loadFiles));
            }

            return projects;
        }
        private static List<GenioProjectProperties> GetProjects(DTE dte, Project solution, bool loadFiles)
        {
            List<GenioProjectProperties> projs = new List<GenioProjectProperties>();

            for(int i = 1; i <= solution.ProjectItems.Count; i++)
            {
                var proj = solution.ProjectItems.Item(i).SubProject;
                if (proj == null)
                    continue;

                else if (proj.Kind == SolutionConstants.vsProjectKindSolutionFolder)
                    projs.AddRange(GetProjects(dte, proj, loadFiles));

                else
                    projs.Add(new GenioProjectProperties(proj, loadFiles));
            }

            return projs;
        }
    }
}
