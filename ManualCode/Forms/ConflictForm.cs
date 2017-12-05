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
            foreach (Difference diff in conflict.DifferenceList.AsList)
            {
                ListViewItem item = new ListViewItem();
                item.Text = diff.Local.ShortOneLineCode();
                item.SubItems.Add(diff.Local.LocalFileName);
                item.Tag = diff;
                lstConflicts.Items.Add(item);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            if (lstConflicts.SelectedItems.Count == 1 && lstConflicts.Items[lstConflicts.SelectedIndices[0]].Tag is Difference diff)
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
                Difference m = (Difference)item.Tag;

                PackageOperations.OpenManualFile(m.Local, false);
            }
        }

        private void lstConflicts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];
                Difference m = (Difference)item.Tag;

                PackageOperations.OpenManualFile(m.Local, false);
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
                Difference diff = (Difference)item.Tag;

                diff.Merge();
                lstConflicts.SelectedItems[0].Text = diff.Local.ShortOneLineCode();
            }
        }
    }
}
