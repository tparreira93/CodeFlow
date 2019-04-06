using CodeFlowBridge;
using CodeFlowLibrary.Genio;
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

namespace CodeFlowUI
{
    public partial class ProfileForm : Form
    {
        private bool _servers = false;
        private Profile _oldProfile = null;
        public Profile ProfileResult { get; private set; } = null;
        
        public ProfileForm(Profile profile)
        {
            InitializeComponent();
            if (profile != null)
            {
                ProfileResult = profile.Clone();
                _oldProfile = profile;
            }
            else
            {
                ProfileResult = new Profile();
            }
        }

        private void LoadServers()
        {
            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow row in dt.Rows)
            {
                string server = (string)row["ServerName"];
                string instance = row["InstanceName"].ToString();
                string name;
                if (string.IsNullOrEmpty(instance))
                    name = server;
                else
                    name = server + "\\" + instance;
                cmbServers.Items.Add(name);
            }
        }
        private void LoadDatabases()
        {
            ProfileResult.GenioConfiguration.Server = cmbServers.Text ?? "";
            if (ProfileResult.GenioConfiguration.Server.Length == 0
                || ProfileResult.GenioConfiguration.Username.Length == 0
                || ProfileResult.GenioConfiguration.Password.Length == 0)
                return;
            
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + ProfileResult.GenioConfiguration.Server
                + ";Initial Catalog=master;User Id=" + ProfileResult.GenioConfiguration.Username
                + ";Password=" + ProfileResult.GenioConfiguration.Password + ";");
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT name FROM sys.databases";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection;
                
                sqlConnection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                cmbDb.Items.Clear();
                while (reader.Read())
                {
                    string div = (string)reader.GetSqlString(0);
                    if (div.Substring(0, 3) == "GEN")
                        cmbDb.Items.Add(div);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorConnect, ProfileResult.GenioConfiguration.Server, ex.Message), 
                    CodeFlowResources.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            txtConfigName.DataBindings.Add("Text", ProfileResult, "ProfileName");
            txtUsername.DataBindings.Add("Text", ProfileResult.GenioConfiguration, "Username");
            txtPassword.DataBindings.Add("Text", ProfileResult.GenioConfiguration, "Password");
            txtGenioUser.DataBindings.Add("Text", ProfileResult.GenioConfiguration, "GenioUser");
            txtGenioPath.DataBindings.Add("Text", ProfileResult.GenioConfiguration, "GenioPath");
            chkProd.DataBindings.Add("Checked", ProfileResult.GenioConfiguration, "ProductionSystem");
            
            if (ProfileResult.GenioConfiguration.Server.Length != 0 && ProfileResult.GenioConfiguration.Database.Length != 0)
            {
                cmbServers.Items.Add(ProfileResult.GenioConfiguration.Server);
                cmbServers.SelectedIndex = 0;
                cmbDb.Items.Add(ProfileResult.GenioConfiguration.Database);
                cmbDb.SelectedIndex = 0;
            }

            if (txtGenioUser.Text.Length == 0)
                txtGenioUser.Text = Environment.UserName;
        }

        private void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        { }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ProfileResult.GenioConfiguration.Server = cmbServers.Text ?? "";
            ProfileResult.GenioConfiguration.Database = cmbDb.Text ?? "";
            Profile p = PackageBridge.Instance.FindProfile(ProfileResult.ProfileName);

            if (_oldProfile != null && p != null && !p.ProfileID.Equals(_oldProfile.ProfileID))
                MessageBox.Show(CodeFlowResources.Resources.ErrorAddProfile, CodeFlowResources.Resources.Configuration,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _servers = false;
            LoadServers();
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            ProfileResult.GenioConfiguration.Server = cmbServers.Text ?? "";
            ProfileResult.GenioConfiguration.Database = cmbDb.Text ?? "";
            if (ProfileResult.GenioConfiguration.OpenConnection())
            {
                ProfileResult.GenioConfiguration.CloseConnection();

                MessageBox.Show(CodeFlowResources.Resources.ConnectionOpen, CodeFlowResources.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if(!_servers)
                    LoadServers();
                _servers = true;
            }
        }

        private void cmbDb_MouseClick(object sender, MouseEventArgs e)
        {
            if (cmbDb.DroppedDown)
                LoadDatabases();
        }

        private void btnRules_Click(object sender, EventArgs e)
        {
            ProfileRules form = new ProfileRules(ProfileResult);
            form.ShowDialog();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtGenioPath.Text = folderDialog.SelectedPath;
                ProfileResult.GenioConfiguration.GenioPath = folderDialog.SelectedPath;
            }
        }
    }
}
