using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow
{
    public partial class ProfilesForm : Form
    {
        public ProfilesForm()
        {
            InitializeComponent();
        }

        private void ProfilesForm_Load(object sender, EventArgs e)
        {
            LoadProfiles();
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (lstProfiles.SelectedItems.Count > 0)
            {
                var item = lstProfiles.SelectedItems[0];
                Profile profile = (Profile)item.Tag;

                try
                {
                    PackageOperations.ActiveProfile = profile;
                    LoadProfiles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnAddProf_Click(object sender, EventArgs e)
        {
            ConnectionForm connection = new ConnectionForm();
            connection.ShowDialog();
            LoadProfiles();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            PackageOperations.SaveProfiles();
            this.Close();
        }

        private void LoadProfiles()
        {
            lstProfiles.Items.Clear();
            foreach (Profile p in PackageOperations.AllProfiles)
            {
                ListViewItem item = new ListViewItem();
                if (PackageOperations.ActiveProfile.ProfileName.Equals(p.ProfileName))
                    item.BackColor = Color.GreenYellow;
                item.Text = p.ProfileName;
                item.Tag = p;
                item.SubItems.Add(p.GenioConfiguration.ToString());
                lstProfiles.Items.Add(item);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(lstProfiles.SelectedIndices.Count > 0)
            {
                Profile p2 = (Profile)lstProfiles.SelectedItems[0].Tag;
                PackageOperations.RemoveProfile(p2.ProfileName);
                LoadProfiles();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            PackageOperations.SaveProfiles();
        }

        private void lstProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstProfiles.SelectedItems.Count == 1)
            {
                ConnectionForm connectionForm = new ConnectionForm(ConnectionForm.Mode.EDIT, (Profile)lstProfiles.Items[lstProfiles.SelectedIndices[0]].Tag);
                connectionForm.ShowDialog();
            }
        }
    }
}
