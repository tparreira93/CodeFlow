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
                Helpers.DetectTextEncoding(path, out string code);
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

        #endregion
    }
}
