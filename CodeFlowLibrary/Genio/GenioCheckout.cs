using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Threading;
using CodeFlowLibrary;
using CodeFlowLibrary.Util;

namespace CodeFlowLibrary.Genio
{
    [Serializable]
    public class GenioCheckout
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
        [NonSerialized]
        private Object obj = new Object();
        [NonSerialized]
        private SqlConnection sqlConnection = new SqlConnection();
        private string geniouser = "";

        public GenioCheckout()
        {
            GenioUser = Environment.UserName;
        }

        public GenioCheckout(string server, string database, string username, string password, string genioUser)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;
            GenioUser = genioUser;
        }

        public GenioCheckout Clone()
        {
            GenioCheckout g = new GenioCheckout();
            Util.CopyFrom(typeof(GenioCheckout), this, g);
            sqlConnection = new SqlConnection();
            obj = new object();
            return g;
        }

        public bool ParseGenioFiles()
        {
            try
            {
                Plataforms = GenioPath.Length == 0 ? GenioPlataform.ParseXml(CodeFlowResources.Resources.ManwinInfoData) : GenioPlataform.ParseFile($"{GenioPath}\\ManwinInfoData.xml");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool LockConnection()
        {
            bool lockWasTaken = false;
            Monitor.Enter(obj, ref lockWasTaken);

            return lockWasTaken;
        }

        private void ReleaseConnection()
        {
            Monitor.Exit(obj);
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

        public bool GetGenioInfo()
        {
            bool result = false;
            if (OpenConnection())
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
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    CloseConnection();
                }
            }
            return result;
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
            bool open = false;
            if (Server.Length != 0 && Database.Length != 0 && Username.Length != 0 && Password.Length != 0)
            {
                try
                {
                    if (LockConnection())
                    {
                        sqlConnection = new SqlConnection(GetConnectionString());
                        sqlConnection.Open();
                        open = sqlConnection.State == ConnectionState.Open;
                    }
                }
                catch (Exception ex)
                {
                    ReleaseConnection();
                    throw new Exception(String.Format("Unable to connect to the database {0}@{1}!\n{2}", Server, Database, ex.Message));
                }
            }
            return open;
        }

        public bool ConnectionIsOpen()
        {
            return sqlConnection != null && sqlConnection.State == ConnectionState.Open;
        }

        /*
         * Connections should be closed in the same thread! 
         */
        public void CloseConnection()
        {
            if (ConnectionIsOpen())
            {
                try
                {
                    SqlConnection.Close();
                }
                finally
                {
                    ReleaseConnection();
                }
            }
        }

        public Dictionary<string, Guid> GetFeatures()
        {
            Dictionary<string, Guid> features = new Dictionary<string, Guid>();
            
            if (OpenConnection())
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
                    CloseConnection();
                }
            }

            return features;
        }

        public Dictionary<string, Guid> GetModules()
        {
            Dictionary<string, Guid> modules = new Dictionary<string, Guid>();

            if (OpenConnection())
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
                catch (Exception)
                { }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                        reader.Close();
                    CloseConnection();
                }
            }

            return modules;
        }
    }
}
