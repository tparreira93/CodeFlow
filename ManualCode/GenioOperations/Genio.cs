using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CodeFlow
{
    [Serializable]
    public class Genio
    {
        private string server = "";
        private string database = "";
        private string username = "QUIDGEST";
        private string password = "ZPH2LAB";
        private string genioPath = "";
        private string checkoutPath = "";
        private string genioVersion = "";
        private Dictionary<string, string> tipos = new Dictionary<string, string>();
        private List<string> plataforms = new List<string>();

        private SqlConnection sqlConnection = new SqlConnection();
        private string geniouser = "";

        public Genio()
        {
            GenioUser = Environment.UserName;
        }

        public Genio(string server, string database, string username, string password, string genioUser)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;
            GenioUser = genioUser;
        }

        public void ParseGenioFiles()
        {
            Plataforms = GetPlataforms();
            Tipos = GetTipos();
        }

        public string Server { get => server; set => server = value; }
        public string Database { get => database; set => database = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }

        [XmlIgnore]
        public SqlConnection SqlConnection { get => sqlConnection; set => sqlConnection = value; }
        public string GenioUser { get => geniouser; set => geniouser = value; }
        public string GenioPath { get => genioPath; set => genioPath = value; }
        public string CheckoutPath { get => checkoutPath; set => checkoutPath = value; }
        public string GenioVersion { get => genioVersion; set => genioVersion = value; }

        [XmlIgnore]
        public List<string> Plataforms { get => plataforms; set => plataforms = value; }

        [XmlIgnore]
        public Dictionary<string, string> Tipos { get => tipos; set => tipos = value; }

        public string GetConnectionString()
        {
            return String.Format("Data Source = {0}; Initial Catalog = {1}; User Id = {2}; Password = {3}; ",
                Server, Database, Username, Password);
        }

        public override string ToString()
        {
            return String.Format("Server={0};Database={1};User={2}", Server, Database, GenioUser);
        }

        public bool OpenConnection()
        {
            if (Server.Length != 0 && Database.Length != 0 && Username.Length != 0 && Password.Length != 0)
            {
                try
                {
                    sqlConnection = new SqlConnection(GetConnectionString());
                    sqlConnection.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to connect to the database {0}@{1}!\n{2}", Server, Database, ex.Message));
                }

                return sqlConnection.State == ConnectionState.Open;
            }
            else
                return false;
        }

        public bool ConnectionIsOpen()
        {
            return sqlConnection != null && sqlConnection.State == ConnectionState.Open;
        }

        public void CloseConnection()
        {
            if (SqlConnection != null && SqlConnection.State == ConnectionState.Open)
                SqlConnection.Close();
        }

        /*public void LoadConfig(string XML)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);

            if (doc.SelectSingleNode("/ManualConfig") == null)
                return;

            XmlNode node = doc.DocumentElement.SelectSingleNode("/ManualConfig/SqlServer");
            if (node == null)
                return;

            Server = node.InnerText;

            node = doc.DocumentElement.SelectSingleNode("/ManualConfig/Database");
            if (node == null)
                return;

            Database = node.InnerText;
            if (doc.SelectSingleNode("/ManualConfig/User") == null)
            {
                node = doc.DocumentElement.SelectSingleNode("/ManualConfig/User");
                Username = node.InnerText;
            }
            else
                Username = "QUIDGEST";

            if (doc.SelectSingleNode("/ManualConfig/Password") == null)
            {
                node = doc.DocumentElement.SelectSingleNode("/ManualConfig/Password");
                Password = node.InnerText;
            }
            else
                Password = "ZPH2LAB";

            if (doc.SelectSingleNode("/ManualConfig/GenioUser") == null)
            {
                node = doc.DocumentElement.SelectSingleNode("/ManualConfig/GenioUser");
                GenioUser = node.InnerText;
            }
            else
                GenioUser = "";

            if (doc.SelectSingleNode("/ManualConfig/GenioPath") == null)
            {
                node = doc.DocumentElement.SelectSingleNode("/ManualConfig/GenioPath");
                GenioPath = node.InnerText;
            }
            else
                GenioPath = "";

            if (doc.SelectSingleNode("/ManualConfig/CheckoutPath") == null)
            {
                node = doc.DocumentElement.SelectSingleNode("/ManualConfig/CheckoutPath");
                CheckoutPath = node.InnerText;
            }
            else
                CheckoutPath = "";
        }

        public String SaveConfig()
        {
            XmlDocument doc = new XmlDocument();

            //(1) the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            //(2) string.Empty makes cleaner code
            XmlElement element1 = doc.CreateElement(string.Empty, "ManualConfig", string.Empty);
            doc.AppendChild(element1);

            XmlElement server = doc.CreateElement(string.Empty, "SqlServer", string.Empty);
            XmlText text1 = doc.CreateTextNode(Server);
            server.AppendChild(text1);
            element1.AppendChild(server);

            XmlElement database = doc.CreateElement(string.Empty, "Database", string.Empty);
            XmlText text2 = doc.CreateTextNode(Database);
            database.AppendChild(text2);
            element1.AppendChild(database);

            XmlElement username = doc.CreateElement(string.Empty, "User", string.Empty);
            XmlText text3 = doc.CreateTextNode(Username);
            username.AppendChild(text3);
            element1.AppendChild(username);

            XmlElement password = doc.CreateElement(string.Empty, "Password", string.Empty);
            XmlText text4 = doc.CreateTextNode(Password);
            password.AppendChild(text4);
            element1.AppendChild(password);

            XmlElement geniouser = doc.CreateElement(string.Empty, "GenioUser", string.Empty);
            XmlText text5 = doc.CreateTextNode(GenioUser);
            geniouser.AppendChild(text5);
            element1.AppendChild(geniouser);

            XmlElement geniopath = doc.CreateElement(string.Empty, "GenioPath", string.Empty);
            XmlText text6 = doc.CreateTextNode(GenioPath);
            geniopath.AppendChild(text6);
            element1.AppendChild(geniopath);

            XmlElement checkoutpath = doc.CreateElement(string.Empty, "CheckoutPath", string.Empty);
            XmlText text7 = doc.CreateTextNode(CheckoutPath);
            checkoutpath.AppendChild(text6);
            element1.AppendChild(checkoutpath);

            return doc.OuterXml;
        }*/

        public Dictionary<string, Guid> GetFeatures()
        {
            Dictionary<string, Guid> features = new Dictionary<string, Guid>();
            lock (PackageOperations.lockObject)
            {
                if (!ConnectionIsOpen())
                    OpenConnection();

                if (ConnectionIsOpen())
                {
                    SqlCommand cmd = new SqlCommand("SELECT CODCARAC, NOME FROM GENCARAC WHERE ZZSTATE<>1",
                        PackageOperations.ActiveProfile.GenioConfiguration.SqlConnection);

                    SqlDataReader reader = null;

                    try
                    {
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Guid codcarac = reader.GetGuid(0);
                            string nome = reader.GetString(1);

                            features.Add(nome, codcarac);
                        }

                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                            reader.Close();
                    }
                }
            }

            return features;
        }

        public Dictionary<string, Guid> GetModules()
        {
            Dictionary<string, Guid> modules = new Dictionary<string, Guid>();
            
            lock (PackageOperations.lockObject)
            {
                if (!ConnectionIsOpen())
                    OpenConnection();

                if (ConnectionIsOpen())
                {
                    SqlCommand cmd = new SqlCommand("SELECT CODMODUL, CODIPROG FROM GENMODUL WHERE ZZSTATE<>1",
                PackageOperations.ActiveProfile.GenioConfiguration.SqlConnection);
                    SqlDataReader reader = null;

                    try
                    {
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Guid codmodul = reader.GetGuid(0);
                            string codiprog = reader.GetString(1);

                            modules.Add(codiprog, codmodul);
                        }

                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                            reader.Close();
                    }
                }
            }

            return modules;
        }

        public Dictionary<string, string> GetTipos()
        {
            Dictionary<string, string> tipos = new Dictionary<string, string>();

            try
            {
                XmlDocument doc = new XmlDocument();
                if (GenioPath.Length == 0)
                    doc.LoadXml(Properties.Resources.ManwinInfoData);
                else
                    doc.LoadXml($"{GenioPath}\\ManwinInfoData.xml");

                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Manwins/ManwinTags/Tag");
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        string tipo = "";
                        string lang = "";
                        XmlNode n = node.SelectSingleNode("Identifier");
                        if (n != null)
                            tipo = n.InnerText;

                        n = node.SelectSingleNode("ProgrammingLanguage");
                        if (n != null)
                            lang = n.InnerText;

                        if (tipo != "" && !tipos.ContainsKey(tipo))
                        {
                            tipos.Add(tipo, lang);
                        }
                    }
                }
            }
            catch(Exception)
            { }

            return tipos;
        }

        public List<string> GetPlataforms()
        {
            List<string> plataforms = new List<string>();

            try
            {
                XmlDocument doc = new XmlDocument();
                if (GenioPath.Length == 0)
                    doc.LoadXml(Properties.Resources.ManwinInfoData);
                else
                    doc.LoadXml($"{GenioPath}\\ManwinInfoData.xml");

                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/Manwins/Platforms/Platform");
                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        XmlNode n = node.SelectSingleNode("Identifier");
                        if (n != null)
                            plataforms.Add(n.InnerText);
                    }
                }
            }
            catch(Exception)
            { }

            return plataforms;
        }
    }
}
