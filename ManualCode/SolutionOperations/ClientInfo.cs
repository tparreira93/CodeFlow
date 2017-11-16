using CodeFlow.Utils;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow.SolutionOperations
{
    public class ClientInfo
    {
        string client;
        string version;
        string system;
        string genioVersion;

        public ClientInfo()
        {
            Client = "";
            Version = "";
            System = "";
            GenioVersion = "";
        }

        public ClientInfo(string client, string version, string system, string genioVersion)
        {
            Client = client;
            Version = version;
            System = system;
            GenioVersion = genioVersion;
        }

        public string Client { get => client; set => client = value; }
        public string Version { get => version; set => version = value; }
        public string System { get => system; set => system = value; }
        public string GenioVersion { get => genioVersion; set => genioVersion = value; }
    }
}
