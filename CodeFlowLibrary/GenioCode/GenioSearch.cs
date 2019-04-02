using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeFlow.ManualOperations;
using CodeFlow.Utils;

namespace CodeFlow.GenioManual
{
    public class GenioSearch
    {
        public GenioSearch()
        {

        }

        private Dictionary<Type, GenioTable> GetSearchableTables()
        {
            return GetTypeAttribute<GenioTable>(Assembly.GetExecutingAssembly(), typeof(GenioTable));
        }

        private List<string> GetSearchableFields()
        {
            List<string> fields = new List<string>();


            return fields;
        }

        

        private Dictionary<Type, T> GetTypeAttribute<T>(Assembly assembly, Type attribute)
        {
            var providers = new Dictionary<Type, T>();

            foreach (Type type in assembly.GetTypes())
            {
                foreach(var att in type.GetCustomAttributes(attribute, true))
                if (att is T provider)
                    providers.Add(type, provider);
            }
            return providers;
        }
    }
}