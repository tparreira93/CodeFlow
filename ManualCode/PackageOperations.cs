using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CodeFlow.SolutionOperations;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodeFlow
{
    public static class PackageOperations
    {
        public static object lockObject = new object();

        private static Profile activeProfile = new Profile();
        private static List<Profile> allProfiles = new List<Profile>();
        private static List<string> openFiles = new List<string>();


        private static GenioSolutionProperties solutionProps = null;
        private static List<string> extensionFilters = new List<string>() { "cpp", "cs", "xml", "h" };
        private static List<string> ignoreFilesFilters = new List<string>();
        private static Boolean continuousAnalysis = false;
        private static Boolean parseSolution = false;
        private static ClientInfo client = new ClientInfo();
        private static List<Form> openForms = new List<Form>();


        internal static Profile ActiveProfile { get => activeProfile; set => activeProfile = value; }
        internal static List<Profile> AllProfiles { get => allProfiles; set => allProfiles = value; }
        public static List<string> OpenFiles { get => openFiles; set => openFiles = value; }
        public static GenioSolutionProperties SolutionProps { get => solutionProps; set => solutionProps = value; }
        public static List<string> ExtensionFilters { get => extensionFilters; set => extensionFilters = value; }
        public static List<string> IgnoreFilesFilters { get => ignoreFilesFilters; set => ignoreFilesFilters = value; }
        public static bool ParseSolution { get => parseSolution; set => parseSolution = value; }
        public static bool ContinuousAnalysis { get => continuousAnalysis; set => continuousAnalysis = value; }
        internal static ClientInfo Client { get => client; set => client = value; }
        public static List<Form> OpenForms { get => openForms; set => openForms = value; }
        public static bool AutoVCCTO2008Fix { get; internal set; }

        public static bool AddProfile(Genio connection, string profileName)
        {
            if (AllProfiles.Find(x => x.ProfileName.Equals(profileName) == true) == null)
            {
                Profile profile = new Profile(profileName, connection);
                AllProfiles.Add(profile);
                return true;
            }
            return false;
        }

        public static bool AddProfile(string server, string database, string dbuser, string password, string username, string profileName)
        {
            Genio connection = new Genio(server, database, dbuser, password, username);
            return AddProfile(connection, profileName);
        }

        public static void UpdateProfile(string profileName, Profile newProfile)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(profileName) == true);
            if (p != null)
            {
                p.GenioConfiguration.CloseConnection();
                p.ProfileName = newProfile.ProfileName;
                p.GenioConfiguration = newProfile.GenioConfiguration;
            }
        }

        public static void RemoveProfile(string profileName)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
            {
                p.GenioConfiguration.CloseConnection();
                AllProfiles.Remove(p);
            }
        }

        public static void SetProfile(string profileName)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
                ActiveProfile = p;
        }

        public static void SaveProfiles()
        {
            Properties.Settings.Default.ConnectionStrings = SaveProfiles(AllProfiles);
            Properties.Settings.Default.Save();
        }

        public static string SaveProfiles(List<Profile> configs)
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
                XmlText text1 = doc.CreateTextNode(p.GenioConfiguration.Server);
                server.AppendChild(text1);
                element1.AppendChild(server);

                XmlElement database = doc.CreateElement(string.Empty, "Database", string.Empty);
                XmlText text2 = doc.CreateTextNode(p.GenioConfiguration.Database);
                database.AppendChild(text2);
                element1.AppendChild(database);

                XmlElement username = doc.CreateElement(string.Empty, "User", string.Empty);
                XmlText text3 = doc.CreateTextNode(p.GenioConfiguration.Username);
                username.AppendChild(text3);
                element1.AppendChild(username);

                XmlElement password = doc.CreateElement(string.Empty, "Password", string.Empty);
                XmlText text4 = doc.CreateTextNode(p.GenioConfiguration.Password);
                password.AppendChild(text4);
                element1.AppendChild(password);

                XmlElement geniouser = doc.CreateElement(string.Empty, "GenioUser", string.Empty);
                XmlText text5 = doc.CreateTextNode(p.GenioConfiguration.GenioUser);
                geniouser.AppendChild(text5);
                element1.AppendChild(geniouser);

                XmlElement profilename = doc.CreateElement(string.Empty, "ProfileName", string.Empty);
                XmlText text6 = doc.CreateTextNode(p.ProfileName);
                profilename.AppendChild(text6);
                element1.AppendChild(profilename);

                XmlElement geniocheckout = doc.CreateElement(string.Empty, "CheckoutPath", string.Empty);
                XmlText text7 = doc.CreateTextNode(p.GenioConfiguration.CheckoutPath);
                geniocheckout.AppendChild(text7);
                element1.AppendChild(geniocheckout);

                XmlElement geniopath = doc.CreateElement(string.Empty, "GenioPath", string.Empty);
                XmlText text8 = doc.CreateTextNode(p.GenioConfiguration.GenioPath);
                geniopath.AppendChild(text8);
                element1.AppendChild(geniopath);
            }

            return doc.OuterXml;
        }

        public static List<Profile> LoadProfiles(string conn)
        {
            List<Profile> profiles = new List<Profile>();
            if (conn.Length != 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(conn);

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

                    node = item.SelectSingleNode("/ManualConfig/ProfileName");
                    if (node != null)
                        p.ProfileName = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/CheckoutPath");
                    if (node != null)
                        connection.CheckoutPath = node.InnerText;

                    node = item.SelectSingleNode("/ManualConfig/GenioPath");
                    if (node != null)
                        connection.GenioPath = node.InnerText;
                    connection.GenioPath = node.InnerText;

                    p.GenioConfiguration = connection;
                    profiles.Add(p);
                }
            }
            return profiles;
        }

        public static void AddTempFile(string file)
        {
            OpenFiles.Add(file);
        }

        public static void RemoveTempFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
            if (OpenFiles.Contains(file))
                OpenFiles.Remove(file);
        }

        public static void RemoveTempFiles()
        {
            foreach (string file in OpenFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        private static Form FindForm(Form form)
        {
            return OpenForms.Find(x => x.GetType() == form.GetType());
        }

        public static DTE GetCurrentDTE(IServiceProvider provider)
        {
            /*ENVDTE. */
            DTE vs = (DTE)provider.GetService(typeof(DTE));
            if (vs == null) throw new InvalidOperationException("DTE not found.");
            return vs;
        }
        public static DTE GetCurrentDTE()
        {
            return GetCurrentDTE(/* Microsoft.VisualStudio.Shell. */ServiceProvider.GlobalProvider);
        }

        public static DTE2 GetCurrentDTE2()
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(SDTE));

            return dte;
        }
    }
}
