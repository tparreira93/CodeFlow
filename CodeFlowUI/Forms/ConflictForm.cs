using CodeFlowLibrary.Bridge;
using CodeFlowLibrary.CodeControl.Changes;
using CodeFlowLibrary.CodeControl.Conflicts;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Package;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlowUI
{
    public partial class ConflictForm : Form
    {
        private readonly ICodeFlowPackage package;
        private readonly Profile profile;
        private Conflict conflict;
        public event EventHandler<ConflictResolveArgs> UpdateForm;

        public ConflictForm(ICodeFlowPackage package, Profile profile, Conflict conf)
        {
            InitializeComponent();
            this.package = package;
            this.profile = profile;
            conflict = conf;
        }

        private void RefreshForm()
        {
            foreach (IChange diff in conflict.DifferenceList.AsList)
                AddListItem(diff);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            if (lstConflicts.SelectedItems.Count == 1 && lstConflicts.Items[lstConflicts.SelectedIndices[0]].Tag is IChange diff)
            {
                ConflictResolveArgs args = new ConflictResolveArgs();
                args.Conflict = conflict;
                args.Keep = diff;
                this.UpdateForm(this, args);
                this.Close();
            }
        }

        private void btnViewCode_Click(object sender, EventArgs e)
        {
            if (lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];
                IChange m = (IChange)item.Tag;

                Task.Factory.StartNew( () => package.FileOps.OpenTempFileAsync(m.Merged, profile, false));
            }
        }

        private void lstConflicts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];

                btnMerge_Click(this, new EventArgs());
            }
        }

        private void ConflictHandler_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void ConflictHandler_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void lstConflicts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstConflicts.SelectedItems.Count == 1)
            {
                btnMerge.Enabled = true;
                btnViewCode.Enabled = true;
                btnUse.Enabled = true;
            }
            else
            {
                btnMerge.Enabled = false;
                btnViewCode.Enabled = false;
                btnUse.Enabled = false;
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            if (lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];
                IChange diff = (IChange)item.Tag;

                IChange change = null;
                try
                {
                    change = diff.Merge();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, ex.Message),
                        CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                }
                lstConflicts.SelectedItems[0].ImageIndex = GetImageIndex(change);
                lstConflicts.SelectedItems[0].Tag = change;
                lstConflicts.SelectedItems[0].Text = change.GetDescription();
                lstConflicts.SelectedItems[0].SubItems[1].Text = change.Merged.ShortOneLineCode();
            }
        }
        private void AddListItem(IChange diff)
        {
            ListViewItem item = new ListViewItem(diff.GetDescription());
            item.SubItems.Add(diff.Merged.ShortOneLineCode());
            item.SubItems.Add(diff.Merged.LocalFileName);
            item.ImageIndex = GetImageIndex(diff);
            item.Tag = diff;
            lstConflicts.Items.Add(item);
        }

        private int GetImageIndex(object change)
        {
            if (change is CodeChange)
                return 0;
            else if (change is CodeNotFound)
                return 1;
            else if (change is CodeEmpty)
                return 2;
            else
                return 0;

        }

        private object GetSelectedItem()
        {
            if (lstConflicts.SelectedItems.Count == 1)
                return lstConflicts.Items[lstConflicts.SelectedIndices[0]].Tag;
            return null;
        }
    }
}
