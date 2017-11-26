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
        private Guid profileID = Guid.NewGuid();
        private Genio genioConfiguration = new Genio();
        private String profileName = "";

        public Profile()
        {
        }

        public Profile Clone()
        {
            Profile p = new Profile();
            p.ProfileID = this.ProfileID;
            p.ProfileName = this.ProfileName;
            p.GenioConfiguration = this.GenioConfiguration.Clone();
            return p;
        }

        public Profile(string profileName, Genio connection)
        {
            GenioConfiguration = connection;
            ProfileName = profileName;
        }

        public Genio GenioConfiguration { get => genioConfiguration; set => genioConfiguration = value; }
        public string ProfileName { get => profileName; set => profileName = value; }
        public Guid ProfileID { get => profileID; set => profileID = value; }

        public override string ToString()
        {
            return String.Format(profileName + "@" + genioConfiguration.ToString());
        }
    }
}
