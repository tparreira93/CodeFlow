using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace CodeFlow
{
    [Serializable]
    public class Profile
    {
        private Genio genioConfiguration;
        private String profileName;
        private Dictionary<string, string> tipos = new Dictionary<string, string>();
        private List<string> plataform = new List<string>();

        public Profile()
        {
            genioConfiguration = new Genio();
            profileName = "";
        }

        public Profile(string profileName, Genio connection)
        {
            GenioConfiguration = connection;
            ProfileName = profileName;
        }

        public Genio GenioConfiguration { get => genioConfiguration; set => genioConfiguration = value; }
        public string ProfileName { get => profileName; set => profileName = value; }

        public static void SaveProfiles(List<Profile> configs)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            foreach (Profile p in configs)
            {
                XmlElement element1 = doc.CreateElement(string.Empty, "ManualConfig", string.Empty);
                doc.AppendChild(element1);

                XmlElement server = doc.CreateElement(string.Empty, "SqlServer", string.Empty);
                XmlText text1 = doc.CreateTextNode(p.genioConfiguration.Server);
                server.AppendChild(text1);
                element1.AppendChild(server);

                XmlElement database = doc.CreateElement(string.Empty, "Database", string.Empty);
                XmlText text2 = doc.CreateTextNode(p.genioConfiguration.Database);
                database.AppendChild(text2);
                element1.AppendChild(database);

                XmlElement username = doc.CreateElement(string.Empty, "User", string.Empty);
                XmlText text3 = doc.CreateTextNode(p.genioConfiguration.Username);
                username.AppendChild(text3);
                element1.AppendChild(username);

                XmlElement password = doc.CreateElement(string.Empty, "Password", string.Empty);
                XmlText text4 = doc.CreateTextNode(p.genioConfiguration.Password);
                password.AppendChild(text4);
                element1.AppendChild(password);

                XmlElement geniouser = doc.CreateElement(string.Empty, "GenioUser", string.Empty);
                XmlText text5 = doc.CreateTextNode(p.genioConfiguration.GenioUser);
                geniouser.AppendChild(text5);
                element1.AppendChild(geniouser);

                XmlElement profilename = doc.CreateElement(string.Empty, "ProfileName", string.Empty);
                XmlText text6 = doc.CreateTextNode(p.ProfileName);
                geniouser.AppendChild(text6);
                element1.AppendChild(profilename);
            }

            Properties.Settings.Default.ConnectionStrings = doc.OuterXml;
            Properties.Settings.Default.Save();
        }

        public String SaveProfile()
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
            XmlText text1 = doc.CreateTextNode(genioConfiguration.Server);
            server.AppendChild(text1);
            element1.AppendChild(server);

            XmlElement database = doc.CreateElement(string.Empty, "Database", string.Empty);
            XmlText text2 = doc.CreateTextNode(genioConfiguration.Database);
            database.AppendChild(text2);
            element1.AppendChild(database);

            XmlElement username = doc.CreateElement(string.Empty, "User", string.Empty);
            XmlText text3 = doc.CreateTextNode(genioConfiguration.Username);
            username.AppendChild(text3);
            element1.AppendChild(username);

            XmlElement password = doc.CreateElement(string.Empty, "Password", string.Empty);
            XmlText text4 = doc.CreateTextNode(genioConfiguration.Password);
            password.AppendChild(text4);
            element1.AppendChild(password);

            XmlElement geniouser = doc.CreateElement(string.Empty, "GenioUser", string.Empty);
            XmlText text5 = doc.CreateTextNode(genioConfiguration.GenioUser);
            geniouser.AppendChild(text5);
            element1.AppendChild(geniouser);

            return doc.OuterXml;
        }

        public static List<Profile> LoadProfiles()
        {
            List<Profile> profiles = new List<Profile>();
            if (Properties.Settings.Default.ConnectionStrings.Length != 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Properties.Settings.Default.ConnectionStrings);

                XmlNodeList nodes = doc.SelectNodes("/ManualConfig");
                foreach (XmlNode item in nodes)
                {
                    Profile p = new Profile();
                    Genio connection = new Genio();
                    XmlNode node = item.SelectSingleNode("/ManualConfig/SqlServer");
                    if (node != null)
                        connection.Server = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/Database");
                    if (node != null)
                        connection.Database = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/User");
                    if (node != null)
                        connection.Username = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/Password");
                    if (node != null)
                        connection.Password = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/GenioUser");
                    if (node != null)
                        connection.GenioUser = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/GenioUser");
                    if (node != null)
                        connection.GenioUser = node.InnerText;

                    profiles.Add(p);
                }
            }
            return profiles;
        }

        public override string ToString()
        {
            return String.Format(profileName + "@" + genioConfiguration.ToString());
        }
    }
}
