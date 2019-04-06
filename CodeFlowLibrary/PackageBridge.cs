using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.CodeControl.Operations;
using CodeFlowLibrary.Util;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Solution;
using System.Threading.Tasks;
using CodeFlowLibrary.Settings;
using CodeFlowLibrary.Package;

namespace CodeFlowBridge
{
    public sealed class PackageBridge
    {
        private static PackageBridge _instance;
        public static ICodeFlowPackage Flow { get; set;  }
        private readonly List<string> _openFiles = new List<string>();
        private Dictionary<string, Type> AutoExportFiles { get; } = new Dictionary<string, Type>();

        #region Properties
        private Profile ActiveProfile { get; set; } = new Profile();
        public readonly List<GenioProjectProperties> SavedFiles = new List<GenioProjectProperties>();
        public List<Profile> AllProfiles { get; set; } = new List<Profile>();
        public GenioSolutionProperties SolutionProps { get; set; }
        public OperationLog ChangeLog { get; set; } = new OperationLog();
        public static PackageBridge Instance => _instance ?? (_instance = new PackageBridge());

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
                Helpers.CopyFrom(typeof(GenioCheckout), newProfile.GenioConfiguration, oldProfile.GenioConfiguration);
                oldProfile.ProfileName = newProfile.ProfileName;
                oldProfile.ProfileRules.Clear();
                oldProfile.ProfileRules.AddRange(newProfile.ProfileRules);

                // Load genio data
                if (newProfile.GenioConfiguration.ParseGenioFiles() && newProfile.GenioConfiguration.GetGenioInfo())
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
                    Flow.SetProfile(String.Empty);
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
        public void SaveProfiles() => Flow.SaveSettings(AllProfiles);

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

            if (Flow.OpenFileAsync(tmp).Result)
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
        #endregion

        #region Operations
        public bool ExecuteOperation(IOperation operation)
        {
            bool result = operation.Execute(GetActiveProfile());
            if (result && PackageOptions.LogOperations)
                ChangeLog.LogOperation(operation);

            return result;
        }
        #endregion

        #region PackageCommands

        #endregion
    }
}
