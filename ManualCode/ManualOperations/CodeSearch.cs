using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeFlow.ManualOperations
{
    class CodeSearch
    {
        /*public IEnumerable<ProjectItem> Recurse(ProjectItems i)
        {
            if (i != null)
            {
                foreach (ProjectItem j in i)
                {
                    foreach (ProjectItem k in Recurse(j))
                    {
                        yield return k;
                    }
                }
            }
        }
        public IEnumerable<ProjectItem> Recurse(ProjectItem i)
        {
            yield return i;
            foreach (ProjectItem j in Recurse(i.ProjectItems))
            {
                yield return j;
            }
        }
        public IEnumerable<ProjectItem> SolutionFiles()
        {
            Solution2 soln = (Solution2)_applicationObject.Solution;
            foreach (Project project in soln.Projects)
            {
                foreach (ProjectItem item in Recurse(project.ProjectItems))
                {
                    yield return item;
                }
            }
        }

        private static string Pattern(string src)
        {
            return ".*" + String.Join(".*", src.ToCharArray());
        }
        private static bool RMatch(string src, string dest)
        {
            try
            {
                return Regex.Match(dest, Pattern(src), RegexOptions.IgnoreCase).Success;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private static List<string> RSearch(string word, IEnumerable<string> wordList, double fuzzyness)
        {
            // Tests have prove that the !LINQ-variant is about 3 times 
            // faster! 
            List<string> foundWords = ( from s in wordList where RMatch(word, s) == true
                                        orderby s.Length 
                                        ascending
                                        select s ).ToList();
            return foundWords;
        }*/
    }
}
