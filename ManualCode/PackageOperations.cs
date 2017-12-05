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
using CodeFlow.Utils;

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
        private static string useCustomTool = "";

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
        public static string UseCustomTool { get => useCustomTool; set => useCustomTool = value; }
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
        public static bool UpdateProfile(Profile oldProfile, Profile newProfile)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(newProfile.ProfileName) 
                                                    && !x.ProfileID.Equals(newProfile.ProfileID));
            if(p == null)
            {
                //Copy stuff
                oldProfile.GenioConfiguration.CloseConnection();
                Util.CopyFrom(typeof(Genio), newProfile.GenioConfiguration, oldProfile.GenioConfiguration);
                oldProfile.ProfileName = newProfile.ProfileName;

                // Load genio data
                newProfile.GenioConfiguration.ParseGenioFiles();
                newProfile.GenioConfiguration.GetGenioInfo();

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
                string fileName = Path.GetFileName(path);
                try
                {
                    List<IManual> l = t.GetMethod("GetManualCode", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { code, fileName }) as List<IManual>;
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

        // Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
        // & big endian), and local default codepage, and potentially other codepages.
        // 'taster' = number of bytes to check of the file (to save processing). Higher
        // value is slower, but more reliable (especially UTF-8 with special characters
        // later on may appear to be ASCII initially). If taster = 0, then taster
        // becomes the length of the file (for maximum reliability). 'text' is simply
        // the string with the discovered encoding applied to the file.
        public static Encoding DetectTextEncoding(string filename, out String text, int taster = 1000)
        {
            byte[] b = File.ReadAllBytes(filename);

            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { text = Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4); return Encoding.GetEncoding("utf-32BE"); }  // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { text = Encoding.UTF32.GetString(b, 4, b.Length - 4); return Encoding.UTF32; }    // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { text = Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2); return Encoding.BigEndianUnicode; }     // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { text = Encoding.Unicode.GetString(b, 2, b.Length - 2); return Encoding.Unicode; }              // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { text = Encoding.UTF8.GetString(b, 3, b.Length - 3); return Encoding.UTF8; } // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { text = Encoding.UTF7.GetString(b, 3, b.Length - 3); return Encoding.UTF7; } // UTF-7


            //////////// If the code reaches here, no BOM/signature was found, so now
            //////////// we need to 'taste' the file to see if can manually discover
            //////////// the encoding. A high taster value is desired for UTF-8
            if (taster == 0 || taster > b.Length) taster = b.Length;    // Taster size can't be bigger than the filesize obviously.


            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: https://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: https://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            int i = 0;
            bool utf8 = false;
            while (i < taster - 4)
            {
                if (b[i] <= 0x7F) { i += 1; continue; }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false; break;
            }
            if (utf8 == true)
            {
                text = Encoding.UTF8.GetString(b);
                return Encoding.UTF8;
            }


            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            int count = 0;
            for (int n = 0; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.BigEndianUnicode.GetString(b); return Encoding.BigEndianUnicode; }
            count = 0;
            for (int n = 1; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { text = Encoding.Unicode.GetString(b); return Encoding.Unicode; } // (little-endian)


            // Finally, a long shot - let's see if we can find "charset=xyz" or
            // "encoding=xyz" to identify the encoding:
            for (int n = 0; n < taster - 9; n++)
            {
                if (
                    ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
                    ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
                    )
                {
                    if (b[n + 0] == 'c' || b[n + 0] == 'C') n += 8; else n += 9;
                    if (b[n] == '"' || b[n] == '\'') n++;
                    int oldn = n;
                    while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
                    { n++; }
                    byte[] nb = new byte[n - oldn];
                    Array.Copy(b, oldn, nb, 0, n - oldn);
                    try
                    {
                        string internalEnc = Encoding.ASCII.GetString(nb);
                        text = Encoding.GetEncoding(internalEnc).GetString(b);
                        return Encoding.GetEncoding(internalEnc);
                    }
                    catch { break; }    // If C# doesn't recognize the name of the encoding, break.
                }
            }


            // If all else fails, the encoding is probably (though certainly not
            // definitely) the user's local codepage! One might present to the user a
            // list of alternative encodings as shown here: https://stackoverflow.com/questions/8509339/what-is-the-most-common-encoding-of-each-language
            // A full list can be found using Encoding.GetEncodings();
            text = Encoding.Default.GetString(b);
            return Encoding.Default;
        }
        #endregion
    }
}
