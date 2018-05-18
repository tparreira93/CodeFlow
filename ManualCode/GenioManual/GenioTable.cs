using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.GenioManual
{
    public class GenioTable : Attribute
    {
        private string tableName;
        public string TableName { get => tableName; set => tableName = value; }

        public GenioTable(string tableName)
        {
            this.TableName = tableName;
        }
    }
}
