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

namespace CodeFlowLibrary.Bridge
{
    public sealed class PackageBridge
    {
        private static PackageBridge _instance;
        public static ICodeFlowPackage Flow { get; set;  }
        private readonly List<string> _openFiles = new List<string>();
        private Dictionary<string, Type> AutoExportFiles { get; } = new Dictionary<string, Type>();

        #region Properties
        public readonly List<GenioProjectProperties> SavedFiles = new List<GenioProjectProperties>();
        public static PackageBridge Instance => _instance ?? (_instance = new PackageBridge());
        public OperationLog ChangeLog { get; } = new OperationLog();

        #endregion

        #region ApplicationProfileManagement
        public bool AddProfile(Profile profile)
        {
            if (Flow.Settings.Profiles.Find(x => x.ProfileName.Equals(profile.ProfileName) == true) == null)
            {
                if (profile.GenioConfiguration.ParseGenioFiles()
                        && profile.GenioConfiguration.GetGenioInfo())
                {
                    Flow.Settings.Profiles.Add(profile);
                    return true;
                }
            }
            return false;
        }
        public bool UpdateProfile(Profile oldProfile, Profile newProfile)
        {
            Profile p = Flow.Settings.Profiles.Find(x => x.ProfileName.Equals(newProfile.ProfileName) 
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
            return Flow.Settings.Profiles.Find(x => x.ProfileName.Equals(profileName));
        }
        public void RemoveProfile(string profileName)
        {
            Profile p = Flow.Settings.Profiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
            {
                Flow.Settings.Profiles.Remove(p);
                if (Flow.Settings.Profiles.Count == 0)
                    Flow.SetProfile(String.Empty);
            }
        }
        #endregion

        #region Helpers
        public void StoreLastProfile(string folder, Profile profile)
        {
            string file = $"{folder}\\LastActiveProfile.xml";
            try
            {
                var stringwriter = new StringWriter();
                var serializer = XmlSerializer.FromTypes(new[] { profile.ProfileName.GetType() })[0];
                serializer.Serialize(stringwriter, profile.ProfileName);

                File.WriteAllText(file, stringwriter.ToString());
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public string SerializeProfiles(List<Profile> profiles)
        {
            var stringwriter = new StringWriter();
            try
            {
                var serializer = XmlSerializer.FromTypes(new[] { profiles.GetType() })[0];
                serializer.Serialize(stringwriter, profiles);
            }
            catch (Exception)
            {
                // ignored
            }
            return stringwriter.ToString();
        }
        public List<Profile> DeSerializeProfiles(string serializedProfiles)
        {
            List<Profile> profiles = null;
            try
            {
                var stringReader = new StringReader(serializedProfiles);
                var serializer = XmlSerializer.FromTypes(new[] { typeof(List<Profile>) })[0];
                profiles = serializer.Deserialize(stringReader) as List<Profile>;
            }
            catch (Exception)
            {
                // ignored
            }
            return profiles ?? new List<Profile>();
        }
        public String SearchLastActiveProfile(string folder)
        {
            string file = $"{folder}\\LastActiveProfile.xml";
            String p = "";
            try
            {
                if (File.Exists(file))
                {
                    var stringReader = new StringReader(File.ReadAllText(file));
                    var serializer = XmlSerializer.FromTypes(new[] { typeof(string) })[0];
                    p = serializer.Deserialize(stringReader) as String;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return p;
        }
        #endregion
    }
}
