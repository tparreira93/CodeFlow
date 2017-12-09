using CodeFlow.CodeControl;
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

namespace CodeFlow
{
    public partial class ConflictForm : Form
    {
        private Conflict conflict;
        public event EventHandler<ConflictResolveArgs> UpdateForm;

        public ConflictForm(Conflict conf)
        {
            InitializeComponent();
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

                PackageOperations.Instance.OpenManualFile(m.Merged, false);
            }
        }

        private void lstConflicts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];
                IChange m = (IChange)item.Tag;

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

                IChange change = diff.Merge();
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
    }
}
