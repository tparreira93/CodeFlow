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
    public partial class ProfileForm : Form
    {
        private bool _servers = false;
        private Mode _openMode;
        private Profile _tmpProfile = null;
        private Profile _oldProfile = null;

        public enum Mode
        {
            New,
            Edit
        }

        public ProfileForm(Mode mode, Profile profile = null)
        {
            InitializeComponent();
            _openMode = mode;
            if (profile != null && _openMode == Mode.Edit)
            {
                _oldProfile = profile;
                _tmpProfile = _oldProfile.Clone();
            }
            else
                _tmpProfile = new Profile();
            DialogResult = DialogResult.Cancel;
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
            _tmpProfile.GenioConfiguration.Server = cmbServers.Text ?? "";
            if (_tmpProfile.GenioConfiguration.Server.Length == 0
                || _tmpProfile.GenioConfiguration.Username.Length == 0
                || _tmpProfile.GenioConfiguration.Password.Length == 0)
                return;
            
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + _tmpProfile.GenioConfiguration.Server
                + ";Initial Catalog=master;User Id=" + _tmpProfile.GenioConfiguration.Username
                + ";Password=" + _tmpProfile.GenioConfiguration.Password + ";");
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
                MessageBox.Show(String.Format(Properties.Resources.ErrorConnect, _tmpProfile.GenioConfiguration.Server, ex.Message), 
                    Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                    sqlConnection.Close();
            }
        }

        private void ConnectionForm_Load(object sender, EventArgs e)
        {
            txtConfigName.DataBindings.Add("Text", _tmpProfile, "ProfileName");
            txtUsername.DataBindings.Add("Text", _tmpProfile.GenioConfiguration, "Username");
            txtPassword.DataBindings.Add("Text", _tmpProfile.GenioConfiguration, "Password");
            txtGenioUser.DataBindings.Add("Text", _tmpProfile.GenioConfiguration, "GenioUser");
            txtGenioPath.DataBindings.Add("Text", _tmpProfile.GenioConfiguration, "GenioPath");
            chkProd.DataBindings.Add("Checked", _tmpProfile.GenioConfiguration, "ProductionSystem");
            
            if (_tmpProfile.GenioConfiguration.Server.Length != 0 && _tmpProfile.GenioConfiguration.Database.Length != 0)
            {
                cmbServers.Items.Add(_tmpProfile.GenioConfiguration.Server);
                cmbServers.SelectedIndex = 0;
                cmbDb.Items.Add(_tmpProfile.GenioConfiguration.Database);
                cmbDb.SelectedIndex = 0;
            }

            if (txtGenioUser.Text.Length == 0)
                txtGenioUser.Text = Environment.UserName;
        }

        private void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        { }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _tmpProfile.GenioConfiguration.Server = cmbServers.Text ?? "";
            _tmpProfile.GenioConfiguration.Database = cmbDb.Text ?? "";

            if ((_openMode == Mode.New && AddProfile()) || (_openMode == Mode.Edit && SaveProfile(_oldProfile, _tmpProfile)))
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            return;

        }

        private bool SaveProfile(Profile oldProfile, Profile newProfile)
        {
            if (!PackageOperations.Instance.UpdateProfile(oldProfile, newProfile))
            {
                MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.Configuration,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DialogResult = DialogResult.OK;
            return true;
        }

        private bool AddProfile()
        {
            if (!PackageOperations.Instance.AddProfile(_tmpProfile.GenioConfiguration, _tmpProfile.ProfileName))
            {
                MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.Configuration,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            DialogResult = DialogResult.OK;
            return true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _servers = false;
            LoadServers();
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            _tmpProfile.GenioConfiguration.Server = cmbServers.Text ?? "";
            _tmpProfile.GenioConfiguration.Database = cmbDb.Text ?? "";
            if (_tmpProfile.GenioConfiguration.OpenConnection())
            {
                _tmpProfile.GenioConfiguration.CloseConnection();

                MessageBox.Show(Properties.Resources.ConnectionOpen, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (_openMode == Mode.New
                    && MessageBox.Show(Properties.Resources.ConfirmAdd, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    && AddProfile())
                        this.Close();

                else if (_openMode == Mode.Edit
                    && MessageBox.Show(Properties.Resources.ConfirmUpdate, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    && SaveProfile(_oldProfile, _tmpProfile))
                        this.Close();
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
    }
}
