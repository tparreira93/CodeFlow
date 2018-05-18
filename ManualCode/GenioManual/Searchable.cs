using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.GenioManual
{
    public class Searchable : Attribute
    {
        private bool searchable;
        public bool Searchable { get => searchable; set => searchable = value; }

        private bool lineSearch;
        public bool LineSearch { get => lineSearch; set => lineSearch = value; }
        
        private bool multipleLinesResult;
        public bool MultipleLinesResult { get => multipleLinesResult; set => multipleLinesResult = value; }

        public Searchable(bool searchable, bool lineSearch, bool multipleLinesResult = false)
        {
            this.Searchable = searchable;
            this.LineSearch = lineSearch;
            this.MultipleLinesResult = multipleLinesResult;
        }
    }
}
