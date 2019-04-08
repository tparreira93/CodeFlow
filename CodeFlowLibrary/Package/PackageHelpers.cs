using CodeFlowLibrary.Genio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CodeFlowLibrary.Package
{
    public class PackageHelpers
    {
        public static void StoreLastProfile(string folder, Profile profile)
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
        public static string SerializeProfiles(List<Profile> profiles)
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
        public static List<Profile> DeSerializeProfiles(string serializedProfiles)
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
    }
}
