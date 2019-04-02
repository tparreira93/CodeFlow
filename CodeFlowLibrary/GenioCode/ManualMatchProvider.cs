using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.GenioCode
{
    public class ManualMatchProvider : Attribute
    {
        string matchBeginnig;
        string matchEnd;

        public ManualMatchProvider(string matchBeginnig, string matchEnd)
        {
            MatchBeginnig = matchBeginnig ?? throw new ArgumentNullException(nameof(matchBeginnig));
            MatchEnd = matchEnd ?? throw new ArgumentNullException(nameof(matchEnd));
        }

        public string MatchBeginnig { get => matchBeginnig; set => matchBeginnig = value; }
        public string MatchEnd { get => matchEnd; set => matchEnd = value; }
    }
}
