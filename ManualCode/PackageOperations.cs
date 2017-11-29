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
using System.Text;
using System.Reflection;

namespace CodeFlow
{
    internal static class PackageOperations
    {
        public static object lockObject = new object();
        private static Profile activeProfile = new Profile();
        private static bool autoExportSaved;
        private static List<Profile> allProfiles = new List<Profile>();
        private static List<string> openFiles = new List<string>();
        private static Dictionary<string, Type> openManual = new Dictionary<string, Type>();
        private static Dictionary<string, Type> AutoExportFiles { get => openManual; set => openManual = value; }
        private static GenioSolutionProperties solutionProps = new GenioSolutionProperties();
        private static DTE2 dte;
        private static bool wholeWordSearch = false;
        private static bool caseSensitive = false;
        private static string currentSearch = "";
        
        #region ToolOptions
        private static List<string> extensionFilters = new List<string>() { "cpp", "cs", "xml", "h" };
        private static List<string> ignoreFilesFilters = new List<string>();
        private static bool continuousAnalysis = false;
        private static bool parseSolution = false;
        public static bool AutoExportSaved { get => autoExportSaved; set => autoExportSaved = value; }
        #endregion

        public static List<GenioProjectProperties> SavedFiles = new List<GenioProjectProperties>();
        private static Profile ActiveProfile { get => activeProfile; set => activeProfile = value; }
        public static List<Profile> AllProfiles { get => allProfiles; set => allProfiles = value; }
        public static GenioSolutionProperties SolutionProps { get => solutionProps; set => solutionProps = value; }
        public static List<string> ExtensionFilters { get => extensionFilters; set => extensionFilters = value; }
        public static List<string> IgnoreFilesFilters { get => ignoreFilesFilters; set => ignoreFilesFilters = value; }
        public static bool ParseSolution { get => parseSolution; set => parseSolution = value; }
        public static bool ContinuousAnalysis { get => continuousAnalysis; set => continuousAnalysis = value; }
        public static bool AutoVCCTO2008Fix { get; internal set; }
        public static DTE2 DTE { get => dte; set => dte = value; }
        public static bool WholeWordSearch { get => wholeWordSearch; set => wholeWordSearch = value; }
        public static bool CaseSensitive { get => caseSensitive; set => caseSensitive = value; }
        public static string CurrentSearch { get => currentSearch; set => currentSearch = value; }

        #region ApplicationProfileManagement
        public static Profile GetActiveProfile()
        {
            return ActiveProfile;
        }
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
                newProfile.GenioConfiguration.GetGenioInfo();

                if(ActiveProfile.ProfileID.Equals(newProfile.ProfileID))
                    ActiveProfile = newProfile;

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
                ActiveProfile.GenioConfiguration.GetGenioInfo();
                ActiveProfile.GenioConfiguration.ParseGenioFiles();
            }
        }
        #endregion

        #region ProfileSettings
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
        #endregion

        #region AutomationModel
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
        #endregion

        #region FileOps
        public static void AddTempFile(string file)
        {
            openFiles.Add(file);
        }
        public static void RemoveTempFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
            if (openFiles.Contains(file))
                openFiles.Remove(file);
        }
        public static void RemoveTempFiles()
        {
            foreach (string file in openFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            openFiles.Clear();
        }
        public static string OpenManualFile(IManual man, bool autoExport)
        {
            string tmp = "";
            try
            {
                tmp = Path.GetTempPath() + Guid.NewGuid().ToString() + "." + man.GetCodeExtension(ActiveProfile);
                File.WriteAllText(tmp, man.ToString(), System.Text.Encoding.UTF8);

                if(autoExport)
                    AutoExportFiles.Add(tmp, man.GetType());

                DTE.ItemOperations.OpenFile(tmp);
                AddTempFile(tmp);
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return tmp;
        }
        public static IManual GetAutoExportIManual(string path)
        {
            IManual man = null;
            if (AutoExportFiles.TryGetValue(path, out Type t))
            {
                string code = File.ReadAllText(path, GetFileEncoding());
                try
                {
                    List<IManual> l = t.GetMethod("GetManualCode", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { code }) as List<IManual>;
                    if (l.Count == 1)
                        man = l[0];
                }
                catch(Exception)
                { }
            }
            return man;
        }
        public static Encoding GetFileEncoding()
        {
            Encoding enc = null;
            try
            {
                enc = Encoding.GetEncoding("Windows-1250");
                //enc = Encoding.Unicode;
            }
            catch(Exception)
            { }

            return enc;
        }
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
        #endregion
    }
}
