using CodeFlowLibrary.Genio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlowUI
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
            ProfileForm profileForm = new ProfileForm(p);
            if (profileForm.ShowDialog() == DialogResult.OK)
            {
                if (!PackageOperations.Instance.AddProfile(profileForm.ProfileResult))
                {
                    MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.Configuration,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    LoadProfiles();
            }
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
            Profile p2 = GetSelectedItem();
            if (p2 != null)
            {
                PackageOperations.Instance.RemoveProfile(p2.ProfileName);
                LoadProfiles();
            }
        }

        private void lstProfiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Profile p = GetSelectedItem();
            if(p != null)
            {
                ProfileForm profileForm = new ProfileForm(p);
                if (profileForm.ShowDialog() == DialogResult.OK)
                {
                    if (!PackageOperations.Instance.UpdateProfile(p, profileForm.ProfileResult))
                    {
                        MessageBox.Show(Properties.Resources.ErrorAddProfile, Properties.Resources.Configuration,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                        LoadProfiles();
                }
            }
        }

        private void ProfilesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PackageOperations.Instance.SaveProfiles();
        }

        private Profile GetSelectedItem()
        {
            if (lstProfiles.SelectedItems.Count == 1)
                return lstProfiles.SelectedItems[0].Tag as Profile;
            return null;
        }
    }
}
