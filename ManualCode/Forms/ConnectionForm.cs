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
        private bool servers = false;
        private Mode openMode;
        private Profile tmpProfile = null;
        private Profile oldProfile = null;

        public enum Mode
        {
            NEW,
            EDIT
        }

        public ProfileForm(Mode mode, Profile profile = null)
        {
            InitializeComponent();
            openMode = mode;
            if (profile != null && openMode == Mode.EDIT)
            {
                oldProfile = profile;
                tmpProfile = oldProfile.Clone();
            }
            else
                tmpProfile = new Profile();
            DialogResult = DialogResult.Cancel;
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
            tmpProfile.GenioConfiguration.Server = cmbServers.Text ?? "";
            if (tmpProfile.GenioConfiguration.Server.Length == 0
                || tmpProfile.GenioConfiguration.Username.Length == 0
                || tmpProfile.GenioConfiguration.Password.Length == 0)
                return;
            
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + tmpProfile.GenioConfiguration.Server
                + ";Initial Catalog=master;User Id=" + tmpProfile.GenioConfiguration.Username
                + ";Password=" + tmpProfile.GenioConfiguration.Password + ";");
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
                MessageBox.Show(String.Format(Properties.Resources.ErrorConnect, tmpProfile.GenioConfiguration.Server, ex.Message), 
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
            txtConfigName.DataBindings.Add("Text", tmpProfile, "ProfileName");
            txtUsername.DataBindings.Add("Text", tmpProfile.GenioConfiguration, "Username");
            txtPassword.DataBindings.Add("Text", tmpProfile.GenioConfiguration, "Password");
            txtGenioUser.DataBindings.Add("Text", tmpProfile.GenioConfiguration, "GenioUser");
            txtGenioPath.DataBindings.Add("Text", tmpProfile.GenioConfiguration, "GenioPath");
            chkProd.DataBindings.Add("Checked", tmpProfile.GenioConfiguration, "ProductionSystem");
            
            if (tmpProfile.GenioConfiguration.Server.Length != 0 && tmpProfile.GenioConfiguration.Database.Length != 0)
            {
                cmbServers.Items.Add(tmpProfile.GenioConfiguration.Server);
                cmbServers.SelectedIndex = 0;
                cmbDb.Items.Add(tmpProfile.GenioConfiguration.Database);
                cmbDb.SelectedIndex = 0;
            }

            if (txtGenioUser.Text.Length == 0)
                txtGenioUser.Text = Environment.UserName;
        }

        private void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        { }

        private void btnSave_Click(object sender, EventArgs e)
        {
            tmpProfile.GenioConfiguration.Server = cmbServers.Text ?? "";
            tmpProfile.GenioConfiguration.Database = cmbDb.Text ?? "";

            if ((openMode == Mode.NEW && addProfile(tmpProfile)) || (openMode == Mode.EDIT && saveProfile(oldProfile, tmpProfile)))
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            return;

        }

        private bool saveProfile(Profile oldProfile, Profile newProfile)
        {
            if (!PackageOperations.UpdateProfile(oldProfile, newProfile))
            {
                MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.Configuration, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DialogResult = DialogResult.OK;
            return true;
        }

        private bool addProfile(Profile newProfile)
        {
            if (!PackageOperations.AddProfile(tmpProfile.GenioConfiguration, tmpProfile.ProfileName))
            {
                MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.Configuration, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DialogResult = DialogResult.OK;
            return true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            servers = false;
            LoadServers();
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            tmpProfile.GenioConfiguration.Server = cmbServers.Text ?? "";
            tmpProfile.GenioConfiguration.Database = cmbDb.Text ?? "";
            if (tmpProfile.GenioConfiguration.OpenConnection())
            {
                tmpProfile.GenioConfiguration.CloseConnection();

                MessageBox.Show(Properties.Resources.ConnectionOpen, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (openMode == Mode.NEW
                    && MessageBox.Show(Properties.Resources.ConfirmAdd, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    && addProfile(tmpProfile))
                        this.Close();

                else if (openMode == Mode.EDIT
                    && MessageBox.Show(Properties.Resources.ConfirmUpdate, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                    && saveProfile(oldProfile, tmpProfile))
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
                if(!servers)
                    LoadServers();
                servers = true;
            }
        }

        private void cmbDb_MouseClick(object sender, MouseEventArgs e)
        {
            if (cmbDb.DroppedDown)
                LoadDatabases();
        }
    }
}
