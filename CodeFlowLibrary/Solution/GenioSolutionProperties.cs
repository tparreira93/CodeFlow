using System;
using System.Collections.Generic;

namespace CodeFlowLibrary.Solution
{
    public class GenioSolutionProperties
    {
        public List<GenioProjectProperties> GenioProjects { get; }
        public ClientInfo ClientInfo { get; set; }
        public string SolutionPath { get; }

        public GenioSolutionProperties(string path, List<GenioProjectProperties> projects)
        {
            SolutionPath = path;
            GenioProjects = projects;
        }
    }
}
