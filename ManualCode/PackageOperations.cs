using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CodeFlow.SolutionOperations;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System.Xml.Serialization;
using System.Text;
using CodeFlow.Utils;
using CodeFlow.CodeControl;
using CodeFlow.GenioManual;

namespace CodeFlow
{
    public sealed class PackageOperations
    {
        private static PackageOperations _instance;
        public static CodeFlowPackage Flow { get; set;  }

        private readonly List<string> _openFiles = new List<string>();
        private Dictionary<string, Type> AutoExportFiles { get; } = new Dictionary<string, Type>();

        private Dictionary<Type, object> openForms = new Dictionary<Type, object>();

        #region ToolOptions

        public bool AutoExportSaved { get; set; }
        public bool LogOperations { private get; set; }
        public List<string> ExtensionFilters { get; } = new List<string>();
        public List<string> IgnoreFilesFilters { get; } = new List<string>();
        public bool ParseSolution { get; set; }
        public bool ContinuousAnalysis { get; set; }
        public bool AutoVccto2008Fix { get; set; }
        public string UseCustomTool { get; set; } = "";
        public bool ForceDOSLine { get; set; }
        public int MaxTaskSolutionCommit { get; set; }
        public bool FixIndexes { get; set; }

        #endregion

        #region Properties
        private Profile ActiveProfile { get; set; } = new Profile();
        public readonly List<GenioProjectProperties> SavedFiles = new List<GenioProjectProperties>();
        public List<Profile> AllProfiles { get; set; } = new List<Profile>();
        public GenioSolutionProperties SolutionProps { get; set; } = new GenioSolutionProperties();
        public DTE2 DTE { get; set; }
        public OperationLog ChangeLog { get; set; } = new OperationLog();
        public static PackageOperations Instance => _instance ?? (_instance = new PackageOperations());

        #endregion

        #region ApplicationProfileManagement
        public Profile GetActiveProfile()
        {
            return ActiveProfile;
        }
        public bool AddProfile(Profile profile)
        {
            if (AllProfiles.Find(x => x.ProfileName.Equals(profile.ProfileName) == true) == null)
            {
                if (profile.GenioConfiguration.ParseGenioFiles()
                        && profile.GenioConfiguration.GetGenioInfo())
                {
                    AllProfiles.Add(profile);
                    return true;
                }
            }
            return false;
        }
        public bool UpdateProfile(Profile oldProfile, Profile newProfile)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(newProfile.ProfileName) 
                                                    && !x.ProfileID.Equals(newProfile.ProfileID));
            if(p == null)
            {
                //Copy stuff
                oldProfile.GenioConfiguration.CloseConnection();
                Util.CopyFrom(typeof(GenioCheckout), newProfile.GenioConfiguration, oldProfile.GenioConfiguration);
                oldProfile.ProfileName = newProfile.ProfileName;

                // Load genio data
                if (newProfile.GenioConfiguration.ParseGenioFiles()
                        && newProfile.GenioConfiguration.GetGenioInfo())
                    return true;
            }
            return false;
        }
        public Profile FindProfile(string profileName)
        {
            return AllProfiles.Find(x => x.ProfileName.Equals(profileName));
        }
        public void RemoveProfile(string profileName)
        {
            Profile p = AllProfiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
            {
                AllProfiles.Remove(p);
                if (AllProfiles.Count == 0)
                    Flow.OnMenuGenioProfilesCombo(this, new OleMenuCmdEventArgs(String.Empty, IntPtr.Zero));
            }
        }
        public void SetProfile(string profileName)
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
        public void SaveProfiles()
        {
            Properties.Settings.Default.ConnectionStrings = SaveProfiles(AllProfiles);
            Properties.Settings.Default.Save();
        }
        public void StoreLastProfile(string folder)
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
            {
                // ignored
            }
        }
        public String SearchLastActiveProfile(string folder)
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
            {
                // ignored
            }

            return p;
        }
        public string SaveProfiles(List<Profile> configs)
        {
            var stringwriter = new StringWriter();
            try
            {
                var serializer = XmlSerializer.FromTypes(new[] { configs.GetType() })[0];
                serializer.Serialize(stringwriter, configs);
            }
            catch (Exception)
            {
                // ignored
            }
            return stringwriter.ToString();
        }
        public List<Profile> LoadProfiles(string conn)
        {
            List<Profile> profiles = null;
            try
            {
                var stringReader = new System.IO.StringReader(conn);
                var serializer = XmlSerializer.FromTypes(new[] { typeof(List<Profile>) })[0];
                profiles = serializer.Deserialize(stringReader) as List<Profile>;
            }
            catch (Exception)
            {
                // ignored
            }
            return profiles ?? new List<Profile>();
        }
        #endregion

        #region AutomationModel

        public DTE GetCurrentDTE(IServiceProvider provider)
        {
            /*ENVDTE. */
            DTE vs = (DTE)provider.GetService(typeof(DTE));
            return vs;
        }
        public DTE GetCurrentDTE()
        {
            return GetCurrentDTE(/* Microsoft.VisualStudio.Shell. */ServiceProvider.GlobalProvider);
        }
        public DTE2 GetCurrentDTE2()
        {
            DTE2 dte = (DTE2)Package.GetGlobalService(typeof(SDTE));

            return dte;
        }
        #endregion

        #region FileOps


        private void AddTempFile(string file)
        {
            if (!_openFiles.Contains(file))
                _openFiles.Add(file);
        }
        public void RemoveTempFile(string file)
        {
            if (IsAutoExportManual(file))
                AutoExportFiles.Remove(file);

            if (_openFiles.Contains(file))
            {
                _openFiles.Remove(file);
                if (File.Exists(file))
                    File.Delete(file);
            }
        }
        public bool IsTempFile(string file)
        {
            return _openFiles.Contains(file);
        }
        public void RemoveTempFiles()
        {
            foreach (string file in _openFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
            _openFiles.Clear();
        }
        public void OpenManualFile(IManual man, bool autoExport)
        {
            var tmp = $"{Path.GetTempPath()}{man.CodeId}.{man.GetCodeExtension(ActiveProfile)}";
            File.WriteAllText(tmp, man.ToString(), Encoding.UTF8);

            if (autoExport)
            {
                if (!AutoExportFiles.ContainsKey(tmp))
                    AutoExportFiles.Add(tmp, man.GetType());
                else
                    AutoExportFiles[tmp] = man.GetType();
            }

            DTE.ItemOperations.OpenFile(tmp);
            AddTempFile(tmp);
        }
        public bool IsAutoExportManual(string path)
        {
            return AutoExportFiles.TryGetValue(path, out Type _);
        }
        public List<IManual> GetAutoExportIManual(string path)
        {
            List<IManual> man = null;
            if (IsAutoExportManual(path))
            {
                string code = File.ReadAllText(path, GetFileEncoding());
                string fileName = Path.GetFileName(path);
                try
                {
                    man = new VSCodeManualMatcher(code, fileName).Match();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            return man;
        }
        public bool OpenOnPosition(string fileName, int position)
        {
            try
            {
                Window window = DTE.ItemOperations.OpenFile(fileName);
                window.Activate();

                CommandHandler.CommandHandler command = new CommandHandler.CommandHandler();
                command.GetCurrentViewText(out int pos, out Microsoft.VisualStudio.Text.Editor.IWpfTextView textView);
                int linePos = textView.TextSnapshot.GetLineNumberFromPosition(position);

                TextSelection textSelection = window.Document.Selection as TextSelection;
                textSelection.MoveToLineAndOffset(linePos, 1);
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        private static Encoding GetFileEncoding()
        {
            Encoding enc = null;
            try
            {
                enc = Encoding.GetEncoding("Windows-1250");
                //enc = Encoding.Unicode;
            }
            catch (Exception)
            {
                // ignored
            }

            return enc;
        }

        // Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
        // & big endian), and local default codepage, and potentially other codepages.
        // 'taster' = number of bytes to check of the file (to save processing). Higher
        // value is slower, but more reliable (especially UTF-8 with special characters
        // later on may appear to be ASCII initially). If taster = 0, then taster
        // becomes the length of the file (for maximum reliability). 'text' is simply
        // the string with the discovered encoding applied to the file.
        public Encoding DetectTextEncoding(string filename, out String text, int taster = 1000)
        {
            byte[] b = File.ReadAllBytes(filename);

            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { text = Encoding.GetEncoding("utf-32BE").GetString(b, 4, b.Length - 4); return Encoding.GetEncoding("utf-32BE"); }  // UTF-32, big-endian 
            if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { text = Encoding.UTF32.GetString(b, 4, b.Length - 4); return Encoding.UTF32; }    // UTF-32, little-endian
            if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { text = Encoding.BigEndianUnicode.GetString(b, 2, b.Length - 2); return Encoding.BigEndianUnicode; }     // UTF-16, big-endian
            if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { text = Encoding.Unicode.GetString(b, 2, b.Length - 2); return Encoding.Unicode; }              // UTF-16, little-endian
            if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { text = Encoding.UTF8.GetString(b, 3, b.Length - 3); return Encoding.UTF8; } // UTF-8
            if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { text = Encoding.UTF7.GetString(b, 3, b.Length - 3); return Encoding.UTF7; } // UTF-7


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
            if (utf8)
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
            text = Encoding.GetEncoding(1252).GetString(b);
            return Encoding.GetEncoding(1252);
        }
        #endregion

        #region Operations

        public bool ExecuteOperation(IOperation operation)
        {
            bool result = operation.Execute();
            if (result && LogOperations)
                ChangeLog.LogOperation(operation);

            return result;
        }

        public void FormIsOpen(Type t)
        {                
        }

        #endregion
    }
}
