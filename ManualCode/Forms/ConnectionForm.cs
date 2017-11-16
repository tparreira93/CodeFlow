using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace CodeFlow
{
    public partial class ConnectionForm : Form
    {
        private bool servers = false;
        private bool databases = false;
        private Mode openMode;
        private Profile current;
        public enum Mode
        {
            NEW,
            EDIT
        }

        public ConnectionForm(Mode mode = Mode.NEW, Profile profile = null)
        {
            InitializeComponent();
            openMode = mode;
            current = profile;
        }

        private void LoadServers()
        {
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow row in dt.Rows)
            {
                string server = (string)row["ServerName"];
                string instance = row["InstanceName"].ToString();
                string name = "";
                if (instance == "")
                    name = server;
                else
                    name = server + "\\" + instance.ToString();
                cmbServers.Items.Add(name);
            }
        }

        private void LoadDatabases()
        {
            string server = cmbServers.Text;
            string user = "QUIDGEST";
            string pass = "ZPH2LAB";
            if (server == null || server.Length == 0)
                return;

            if (txtUsername.Text.Length > 0)
                user = txtUsername.Text;
            if (txtPassword.Text.Length > 0)
                pass = txtPassword.Text;

            string connectionStr = @"Data Source=" + server + ";Initial Catalog=master;User Id=" + user + ";Password=" + pass + ";";
            string query = "SELECT name FROM sys.databases";

            SqlConnection sqlConnection = new SqlConnection(connectionStr);
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = query,
                    CommandType = CommandType.Text,
                    Connection = sqlConnection
                };
                sqlConnection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                cmbDb.Items.Clear();
                while (reader.Read())
                {
                    string div = (string)reader.GetSqlString(0);
                    if (div.Substring(0, 3) == "GEN")
                        cmbDb.Items.Add(div);
                }
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorConnect, server, ex.Message), Properties.Resources.ConnectDB, 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            if (current.GenioConfiguration.Server.Length != 0 && current.GenioConfiguration.Database.Length != 0)
            {
                cmbServers.Items.Add(current.GenioConfiguration.Server);
                cmbServers.SelectedIndex = 0;
                cmbDb.Items.Add(current.GenioConfiguration.Database);
                cmbDb.SelectedIndex = 0;
            }
            txtUsername.Text = current.GenioConfiguration.Username;
            txtPassword.Text = current.GenioConfiguration.Password;
            txtGenioUser.Text = current.GenioConfiguration.GenioUser;
            txtGenioPath.Text = current.GenioConfiguration.GenioPath;
            txtCheckoutPath.Text = current.GenioConfiguration.CheckoutPath;

            if (txtUsername.Text.Length == 0)
            {
                txtUsername.Text = "QUIDGEST";
                txtPassword.Text = "ZPH2LAB";
            }
            if(txtGenioUser.Text.Length == 0)
                txtGenioUser.Text = Environment.UserName;
        }

        private void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDatabases();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sv = (string)cmbServers.Text;
            string db = (string)cmbDb.Text;
            string us = "QUIDGEST";
            string pw = "ZPH2LAB";
            string genioPath = txtGenioPath.Text;
            string checkoutPath = txtCheckoutPath.Text;

            if (txtUsername.Text.Length != 0)
            {
                us = txtUsername.Text;
                if (txtPassword.Text.Length != 0)
                    pw = txtPassword.Text;
            }

            if(us.Length == 0)
                pw = "";

            if (openMode == Mode.NEW && !PackageOperations.AddProfile(sv, db, us, pw, txtGenioUser.Text, txtConfigName.Text))
            {
                MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (openMode == Mode.EDIT && current != null)
            {
                Genio genio = new Genio
                {
                    Server = sv,
                    Database = db,
                    Username = us,
                    Password = pw,
                    GenioUser = txtGenioUser.Text
                };
                genio.GenioPath = genioPath;
                genio.CheckoutPath = checkoutPath;
                PackageOperations.UpdateProfile(current.ProfileName, new Profile(txtConfigName.Text, genio));
            }

            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadServers();
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            string server = cmbServers.Text;
            string db = cmbDb.Text;
            string username = "QUIDGEST";
            string password = "ZPH2LAB";
            string genioPath = txtGenioPath.Text;
            string checkoutPath = txtCheckoutPath.Text;

            if (txtUsername.Text.Length != 0)
            {
                username = txtUsername.Text;
                if (txtPassword.Text.Length != 0)
                    password = txtPassword.Text;
            }

            if (username.Length == 0)
                password = "";

            if (server == null || db == null || server.Length == 0 || db.Length == 0)
            {
                MessageBox.Show(Properties.Resources.ErrorSelectDB, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Genio genio = new Genio
            {
                Server = server,
                Database = db,
                Username = username,
                Password = password,
                GenioUser = txtGenioUser.Text
            };
            genio.GenioPath = genioPath;
            genio.CheckoutPath = checkoutPath;

            if (genio.OpenConnection())
            {
                MessageBox.Show(Properties.Resources.ConnectionOpen, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Information);
                genio.CloseConnection();

                if (openMode == Mode.NEW)
                {
                    if (MessageBox.Show(Properties.Resources.ConfirmAdd, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (!PackageOperations.AddProfile(genio, txtConfigName.Text))
                            MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        else
                            this.Close();
                    }
                }
                else
                {
                    if (MessageBox.Show(Properties.Resources.ConfirmUpdate, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        UpdateProfile(current.ProfileName, new Profile(txtConfigName.Text, genio));
                        this.Close();
                    }
                }
            }
        }

        private void UpdateProfile(string profileName, Profile profile)
        {
            Profile p = PackageOperations.AllProfiles.Find(x => x.ProfileName.Equals(profileName) == true);
            if(p != null)
            {
                p.GenioConfiguration.CloseConnection();
                p.ProfileName = profile.ProfileName;
                p.GenioConfiguration = profile.GenioConfiguration;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbServers_MouseClick(object sender, MouseEventArgs e)
        {
            if(cmbServers.DroppedDown)
            {
                if(!servers)
                    LoadServers();
                servers = true;
            }
        }

        private void cmbDb_MouseClick(object sender, MouseEventArgs e)
        {
            if (cmbServers.DroppedDown)
            {
                if(!databases)
                    LoadDatabases();
                databases = true;
            }
        }
    }
}
