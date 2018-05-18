using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.GenioManual
{
    public class GenioField : Attribute
    {
        private string fieldName;
        public string FieldName { get => fieldName; set => fieldName = value; }

        public GenioTable(string fieldName)
        {
            this.FieldName = fieldName;
        }
    }
}
