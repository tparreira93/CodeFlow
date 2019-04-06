using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Solution
{
    public class ClientInfo
    {
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

        public string Client { get; set; }
        public string Version { get; set; }
        public string System { get; set; }
        public string GenioVersion { get; set; }
    }
}
