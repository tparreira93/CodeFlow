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
        private Mode openMode;
        private Profile current = new Profile();

        public enum Mode
        {
            NEW,
            EDIT
        }

        public ConnectionForm(Mode mode, Profile profile)
        {
            InitializeComponent();
            openMode = mode;
            current = profile;
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
            current.GenioConfiguration.Server = cmbServers.Text ?? "";
            if (current.GenioConfiguration.Server.Length == 0
                || current.GenioConfiguration.Username.Length == 0
                || current.GenioConfiguration.Password.Length == 0)
                return;
            
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=" + current.GenioConfiguration.Server
                + ";Initial Catalog=master;User Id=" + current.GenioConfiguration.Username
                + ";Password=" + current.GenioConfiguration.Password + ";");
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
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorConnect, current.GenioConfiguration.Server, ex.Message), 
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
            txtConfigName.DataBindings.Add("Text", current, "ProfileName");
            txtUsername.DataBindings.Add("Text", current.GenioConfiguration, "Username");
            txtPassword.DataBindings.Add("Text", current.GenioConfiguration, "Password");
            txtGenioUser.DataBindings.Add("Text", current.GenioConfiguration, "GenioUser");
            txtGenioPath.DataBindings.Add("Text", current.GenioConfiguration, "GenioPath");
            txtCheckoutPath.DataBindings.Add("Text", current.GenioConfiguration, "CheckoutPath");

            if (current.GenioConfiguration.Server.Length != 0 && current.GenioConfiguration.Database.Length != 0)
            {
                cmbServers.Items.Add(current.GenioConfiguration.Server);
                cmbServers.SelectedIndex = 0;
                cmbDb.Items.Add(current.GenioConfiguration.Database);
                cmbDb.SelectedIndex = 0;
            }

            if (txtGenioUser.Text.Length == 0)
                txtGenioUser.Text = Environment.UserName;
        }

        private void cmbServers_SelectedIndexChanged(object sender, EventArgs e)
        { }

        private void btnSave_Click(object sender, EventArgs e)
        {
            current.GenioConfiguration.Server = cmbServers.Text ?? "";
            current.GenioConfiguration.Database = cmbDb.Text ?? "";

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            servers = false;
            LoadServers();
        }

        private void btnTry_Click(object sender, EventArgs e)
        {
            current.GenioConfiguration.Server = cmbServers.Text ?? "";
            current.GenioConfiguration.Database = cmbDb.Text ?? "";
            if (current.GenioConfiguration.OpenConnection())
            {
                MessageBox.Show(Properties.Resources.ConnectionOpen, Properties.Resources.ConnectDB, MessageBoxButtons.OK, MessageBoxIcon.Information);
                current.GenioConfiguration.CloseConnection();

                if (openMode == Mode.NEW)
                {
                    if (MessageBox.Show(Properties.Resources.ConfirmAdd, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                else
                {
                    if (MessageBox.Show(Properties.Resources.ConfirmUpdate, Properties.Resources.ConnectDB, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
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
