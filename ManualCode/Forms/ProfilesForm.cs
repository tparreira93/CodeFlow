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

        private void btnAddProf_Click(object sender, EventArgs e)
        {
            Profile p = new Profile();
            ConnectionForm connection = new ConnectionForm(ConnectionForm.Mode.NEW, p);
            connection.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
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

        private void lstProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstProfiles.SelectedItems.Count == 1)
            {
                Profile p = lstProfiles.Items[lstProfiles.SelectedIndices[0]].Tag as Profile;
                ConnectionForm connectionForm = new ConnectionForm(ConnectionForm.Mode.EDIT, p);
                connectionForm.ShowDialog();
            }
        }

        private void ProfilesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PackageOperations.SaveProfiles();
        }
    }
}
