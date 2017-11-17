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

        public Profile(Profile p)
        {
            ProfileID = p.ProfileID;
            ProfileName = p.ProfileName;

            GenioConfiguration.CheckoutPath = p.GenioConfiguration.CheckoutPath;
            GenioConfiguration.Database = p.GenioConfiguration.Database;
            GenioConfiguration.GenioPath = p.GenioConfiguration.GenioPath;
            GenioConfiguration.GenioUser = p.GenioConfiguration.GenioUser;
            GenioConfiguration.GenioVersion = p.GenioConfiguration.GenioVersion;
            GenioConfiguration.Password = p.GenioConfiguration.Password;
            GenioConfiguration.Plataforms = p.GenioConfiguration.Plataforms;
            GenioConfiguration.Server = p.GenioConfiguration.Server;
            GenioConfiguration.Tipos = p.GenioConfiguration.Tipos;
            GenioConfiguration.Username = p.GenioConfiguration.Username;
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
