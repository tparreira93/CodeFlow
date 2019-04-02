using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Genio
{
    public class TipoRotina
    {
        private string identifier = "";
        private string platform = "";
        private string parameterType = "";
        private string description = "";
        private string example = "";
        private string destination = "";
        private string programmingLanguage = "";

        public string Identifier { get => identifier; set => identifier = value; }
        public string Platform { get => platform; set => platform = value; }
        public string ParameterType { get => parameterType; set => parameterType = value; }
        public string Description { get => description; set => description = value; }
        public string Example { get => example; set => example = value; }
        public string Destination { get => destination; set => destination = value; }
        public string ProgrammingLanguage { get => programmingLanguage; set => programmingLanguage = value; }
    }
}
