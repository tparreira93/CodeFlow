using CodeFlowLibrary.Genio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Settings
{
    public class SearchOptions
    {
        public SearchOptions()
        {
        }

        public SearchOptions(Profile profile, string searchTerm, bool wholeWord, bool matchCase)
        {
            SearchTerm = searchTerm;
            WholeWord = wholeWord;
            MatchCase = matchCase;
            SearchProfile = profile;
        }

        public string SearchTerm { get; set; }
        public bool WholeWord { get; set; }
        public bool MatchCase { get; set; }
        public Profile SearchProfile { get; set; }
    }
}
