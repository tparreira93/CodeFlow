using CodeFlow.CodeControl.Rules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.Forms
{
    public partial class ProfileRules : Form
    {
        Profile profile;
        public ProfileRules(Profile p)
        {
            InitializeComponent();
            this.profile = p;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddRule_Click(object sender, EventArgs e)
        {
            RuleForm form = new RuleForm();
            form.ShowDialog();
            if (form.R != null)
            {
                profile.ProfileRules.Add(form.R);
                RefreshList();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            CodeRule rule = GetSelectedItem();
            if (rule == null)
                return;

            profile.ProfileRules.Remove(rule);
            RefreshList();
        }

        private void ProfileRule_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void lstRules_DoubleClick(object sender, EventArgs e)
        {
            CodeRule rule = GetSelectedItem();
            if (rule != null)
            {
                RuleForm form = new RuleForm(rule);
                if(form.ShowDialog() == DialogResult.OK)
                {
                    profile.ProfileRules.Remove(rule);
                    if(form.R != null)
                        profile.ProfileRules.Add(form.R);

                    RefreshList();
                }
            }
        }

        private void RefreshList()
        {
            lstRules.Items.Clear();
            foreach (var rule in profile.ProfileRules)
            {
                RuleProvider provider = Utils.Util.GetAttribute<RuleProvider> (rule.GetType()) as RuleProvider;
                if (!provider.IsDefaultType)
                {
                    ListViewItem item = new ListViewItem(provider.RuleName);
                    item.SubItems.Add(rule.Description);
                    item.Tag = rule;
                    lstRules.Items.Add(item);
                }
            }
        }

        private CodeRule GetSelectedItem()
        {
            if (lstRules.SelectedItems.Count == 1)
                return lstRules.SelectedItems[0].Tag as CodeRule;
            return null;
        }
    }
}
