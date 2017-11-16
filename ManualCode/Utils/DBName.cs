using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow
{
    public class DBName : Attribute
    {
        private string dbName;

        public string DbName { get => dbName; set => dbName = value; }

        public DBName(string dbName)
        {
            DbName = dbName;
        }
    }
}
