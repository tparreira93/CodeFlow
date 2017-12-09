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
            ProfileForm connection = new ProfileForm(ProfileForm.Mode.NEW, p);
            connection.ShowDialog();
            LoadProfiles();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadProfiles()
        {
            lstProfiles.Items.Clear();
            foreach (Profile p in PackageOperations.Instance.AllProfiles)
            {
                ListViewItem item = new ListViewItem();
                if (PackageOperations.Instance.GetActiveProfile().ProfileName.Equals(p.ProfileName))
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
                PackageOperations.Instance.RemoveProfile(p2.ProfileName);
                LoadProfiles();
            }
        }

        private void lstProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstProfiles.SelectedItems.Count == 1)
            {
                Profile p = lstProfiles.Items[lstProfiles.SelectedIndices[0]].Tag as Profile;
                ProfileForm connectionForm = new ProfileForm(ProfileForm.Mode.EDIT, p);
                connectionForm.ShowDialog();
                LoadProfiles();
            }
        }

        private void ProfilesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PackageOperations.Instance.SaveProfiles();
        }
    }
}
