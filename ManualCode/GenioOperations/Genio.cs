using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CodeFlow.Utils;
using System.Xml;
using System.Xml.Serialization;
using CodeFlow.GenioOperations;
using System.Reflection;

namespace CodeFlow
{
    [Serializable]
    public class Genio
    {
        private string server = "";
        private string database = "";
        private string username = "QUIDGEST";
        private string password = "ZPH2LAB";
        private string genioPath = "";
        private string checkoutPath = "";
        private double genioVersion = 0.0f;
        private string systemInitials = "";
        private string bdVersion = "";
        private bool productionSystem = false;
        private List<GenioPlataform> plataforms = new List<GenioPlataform>();

        private SqlConnection sqlConnection = new SqlConnection();
        private string geniouser = "";

        public Genio()
        {
            GenioUser = Environment.UserName;
        }

        public Genio(string server, string database, string username, string password, string genioUser)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;
            GenioUser = genioUser;
        }

        public Genio Clone()
        {
            Genio g = new Genio();
            CopyFrom(typeof(Genio), this, g);
            /*
            g.CheckoutPath = this.CheckoutPath;
            g.Database = this.Database;
            g.GenioPath = this.GenioPath;
            g.GenioUser = this.GenioUser;
            g.GenioVersion = this.GenioVersion;
            g.Password = this.Password;
            g.Server = this.Server;
            g.Username = this.Username;
            g.Plataforms = this.Plataforms;
            g.SystemInitials = this.SystemInitials;
            g.BDVersion = this.BDVersion;
            g.ProductionSystem = this.ProductionSystem;*/

            return g;
        }

        public static void CopyFrom(Type type, Genio source, Genio destination)
        {
            FieldInfo[] myObjectFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo fi in myObjectFields)
            {
                fi.SetValue(destination, fi.GetValue(source));
            }
        }

        public void ParseGenioFiles()
        {
            if (GenioPath.Length == 0)
                Plataforms = GenioPlataform.ParseXml(Properties.Resources.ManwinInfoData);
            else
                Plataforms = GenioPlataform.ParseFile($"{GenioPath}\\ManwinInfoData.xml");
        }

        public string Server { get => server; set => server = value; }
        public string Database { get => database; set => database = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }

        [XmlIgnore]
        public SqlConnection SqlConnection { get => sqlConnection; set => sqlConnection = value; }
        public string GenioUser { get => geniouser; set => geniouser = value; }
        public string GenioPath { get => genioPath; set => genioPath = value; }
        public string CheckoutPath { get => checkoutPath; set => checkoutPath = value; }
        public double GenioVersion { get => genioVersion; set => genioVersion = value; }

        [XmlIgnore]
        public List<GenioPlataform> Plataforms { get => plataforms; set => plataforms = value; }
        public string SystemInitials { get => systemInitials; set => systemInitials = value; }
        public string BDVersion { get => bdVersion; set => bdVersion = value; }
        public bool ProductionSystem { get => productionSystem; set => productionSystem = value; }

        public void GetGenioInfo()
        {
            lock (PackageOperations.lockObject)
            {
                if (!ConnectionIsOpen())
                    OpenConnection();

                if (ConnectionIsOpen())
                {
                    SqlCommand cmd = new SqlCommand("SELECT SISTEMA, GENVERS, LOCLPATH, VERSAO FROM GENGLOB", SqlConnection);

                    SqlDataReader reader = null;

                    try
                    {
                        reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            SystemInitials = reader.SafeGetString(0);
                            GenioVersion = reader.SafeGetDouble(1);
                            CheckoutPath = reader.SafeGetString(2);
                            BDVersion = reader.SafeGetString(3);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                            reader.Close();
                    }
                }
            }
        }

        public string GetConnectionString()
        {
            return String.Format("Data Source = {0}; Initial Catalog = {1}; User Id = {2}; Password = {3}; ",
                Server, Database, Username, Password);
        }

        public override string ToString()
        {
            return String.Format("Server={0};Database={1};User={2}", Server, Database, GenioUser);
        }

        public bool OpenConnection()
        {
            if (Server.Length != 0 && Database.Length != 0 && Username.Length != 0 && Password.Length != 0)
            {
                try
                {
                    sqlConnection = new SqlConnection(GetConnectionString());
                    sqlConnection.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to connect to the database {0}@{1}!\n{2}", Server, Database, ex.Message));
                }

                return sqlConnection.State == ConnectionState.Open;
            }
            else
                return false;
        }

        public bool ConnectionIsOpen()
        {
            return sqlConnection != null && sqlConnection.State == ConnectionState.Open;
        }

        public void CloseConnection()
        {
            if (SqlConnection != null && SqlConnection.State == ConnectionState.Open)
                SqlConnection.Close();
        }

        public Dictionary<string, Guid> GetFeatures()
        {
            Dictionary<string, Guid> features = new Dictionary<string, Guid>();
            lock (PackageOperations.lockObject)
            {
                if (!ConnectionIsOpen())
                    OpenConnection();

                if (ConnectionIsOpen())
                {
                    SqlCommand cmd = new SqlCommand("SELECT CODCARAC, NOME FROM GENCARAC WHERE ZZSTATE<>1", SqlConnection);

                    SqlDataReader reader = null;

                    try
                    {
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Guid codcarac = reader.SafeGetGuid(0);
                            string nome = reader.SafeGetString(1);

                            features.Add(nome, codcarac);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                            reader.Close();
                    }
                }
            }

            return features;
        }

        public Dictionary<string, Guid> GetModules()
        {
            Dictionary<string, Guid> modules = new Dictionary<string, Guid>();
            
            lock (PackageOperations.lockObject)
            {
                if (!ConnectionIsOpen())
                    OpenConnection();

                if (ConnectionIsOpen())
                {
                    SqlCommand cmd = new SqlCommand("SELECT CODMODUL, CODIPROG FROM GENMODUL WHERE ZZSTATE<>1", SqlConnection);
                    SqlDataReader reader = null;

                    try
                    {
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            Guid codmodul = reader.SafeGetGuid(0);
                            string codiprog = reader.SafeGetString(1);

                            modules.Add(codiprog, codmodul);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        if (reader != null && !reader.IsClosed)
                            reader.Close();
                    }
                }
            }

            return modules;
        }
    }
}
