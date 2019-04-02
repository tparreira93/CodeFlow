using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CodeFlowLibrary.Genio
{
    public class GenioPlataform
    {
        private string plataform = "";
        private string id = "";
        private List<TipoRotina> tipoRotina = new List<TipoRotina>();

        private GenioPlataform(string id, string plataform)
        {
            this.ID = id;
            this.Plataform = plataform;
        }

        public string Plataform { get => plataform; set => plataform = value; }
        public List<TipoRotina> TipoRotina { get => tipoRotina; set => tipoRotina = value; }
        public string ID { get => id; set => id = value; }

        public static List<GenioPlataform> ParseFile(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            return Parse(doc);
        }

        private static List<GenioPlataform> Parse(XmlDocument doc)
        {
            List<GenioPlataform> plataforms = new List<GenioPlataform>();

            try
            {
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Manwins/Platforms/Platform");
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode n = node.SelectSingleNode("Identifier");
                        XmlNode desc = node.SelectSingleNode("Description");
                        if (n != null && desc != null)
                        {
                            GenioPlataform plataform = new GenioPlataform(n.InnerText, desc.InnerText);
                            plataforms.Add(plataform);
                            XmlNodeList tags = doc.DocumentElement.SelectNodes($"/Manwins/ManwinTags/Tag[Platform='{n.InnerText}']");
                            if (tags != null)
                            {
                                foreach (XmlNode item in tags)
                                {
                                    TipoRotina t = new TipoRotina();
                                    t.Identifier = item.SelectSingleNode("Identifier")?.InnerText ?? "";
                                    t.Platform = item.SelectSingleNode("Platform")?.InnerText ?? "";
                                    t.Description = item.SelectSingleNode("Description")?.InnerText ?? "";
                                    t.ParameterType = item.SelectSingleNode("ParameterType")?.InnerText ?? "";
                                    t.Example = item.SelectSingleNode("Example")?.InnerText ?? "";
                                    t.Destination = item.SelectSingleNode("Destination")?.InnerText ?? "";
                                    t.ProgrammingLanguage = item.SelectSingleNode("ProgrammingLanguage")?.InnerText ?? "";
                                    plataform.TipoRotina.Add(t);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            { }

            return plataforms;
        }

        public static List<GenioPlataform> ParseXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return Parse(doc);
        }
    }
}
