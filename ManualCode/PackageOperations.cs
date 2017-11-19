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
using System.Xml.Serialization;

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
        private static DTE2 dte;


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
        public static DTE2 DTE { get => dte; set => dte = value; }

        public static bool AddProfile(Genio connection, string profileName)
        {
            if (AllProfiles.Find(x => x.ProfileName.Equals(profileName) == true) == null)
            {
                Profile profile = new Profile(profileName, connection);
                profile.GenioConfiguration.ParseGenioFiles();
                AllProfiles.Add(profile);
                return true;
            }
            return false;
        }

        public static bool UpdateProfile(string profileName, Profile newProfile)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(newProfile.ProfileName) 
                                                    && !x.ProfileID.Equals(newProfile.ProfileID));
            if(p == null)
            {
                RemoveProfile(profileName);
                AllProfiles.Add(newProfile);
                newProfile.GenioConfiguration.ParseGenioFiles();
                return true;
            }
            return false;
        }

        public static void RemoveProfile(string profileName)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
                AllProfiles.Remove(p);
        }

        public static void SetProfile(string profileName)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
            {
                ActiveProfile = p;
                ActiveProfile.GenioConfiguration.ParseGenioFiles();
            }
        }

        public static void SaveProfiles()
        {
            Properties.Settings.Default.ConnectionStrings = SaveProfiles(AllProfiles);
            Properties.Settings.Default.Save();
        }
        public static void StoreLastProfile(string folder)
        {
            string file = $"{folder}\\LastActiveProfile.xml";
            try
            {
                var stringwriter = new StringWriter();
                var serializer = XmlSerializer.FromTypes(new[] { ActiveProfile.ProfileName.GetType() })[0];
                serializer.Serialize(stringwriter, ActiveProfile.ProfileName);

                File.WriteAllText(file, stringwriter.ToString());
            }
            catch (Exception)
            { }
        }
        public static String SearchLastActiveProfile(string folder)
        {
            string file = $"{folder}\\LastActiveProfile.xml";
            String p = null;
            try
            {
                if (File.Exists(file))
                {
                    var stringReader = new System.IO.StringReader(File.ReadAllText(file));
                    var serializer = XmlSerializer.FromTypes(new[] { ActiveProfile.ProfileName.GetType() })[0];
                    p = serializer.Deserialize(stringReader) as String;
                }
            }
            catch (Exception)
            { }

            return p;
        }
        public static string SaveProfiles(List<Profile> configs)
        {
            var stringwriter = new StringWriter();
            try
            {
                var serializer = XmlSerializer.FromTypes(new[] { configs.GetType() })[0];
                serializer.Serialize(stringwriter, configs);
            }
            catch(Exception)
            { }
            return stringwriter.ToString();
        }

        public static List<Profile> LoadProfiles(string conn)
        {
            List<Profile> profiles = null;
            try
            {
                var stringReader = new System.IO.StringReader(conn);
                var serializer = XmlSerializer.FromTypes(new[] { typeof(List<Profile>) })[0];
                profiles = serializer.Deserialize(stringReader) as List<Profile>;
            }
            catch(Exception)
            { }
            return profiles ?? new List<Profile>(); ;
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
