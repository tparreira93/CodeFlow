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
    public partial class ConflictHandler : Form
    {
        private List<ManuaCode> conflictList;
        public event EventHandler<ManuaCode> UpdateForm;

        public ConflictHandler(List<ManuaCode> conflicts)
        {
            InitializeComponent();
            conflictList = conflicts;
        }

        private void RefreshForm()
        {
            int substringSize = 250;
            int sz = substringSize;
            foreach (ManuaCode m in conflictList)
            {
                ListViewItem item = new ListViewItem();
                if (m.Code.Length < substringSize)
                    sz = m.Code.Length;

                item.Text = m.Code.Substring(0, sz);
                item.Tag = m;
                lstConflicts.Items.Add(item);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUse_Click(object sender, EventArgs e)
        {
            ManuaCode m = null;
            if (lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];
                m = (ManuaCode)item.Tag;
            }

            if (m != null)
            {
                this.UpdateForm(this, m);
                this.Close();
            }
        }

        private void btnViewCode_Click(object sender, EventArgs e)
        {
            if (lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.Items[lstConflicts.SelectedIndices[0]];
                ManuaCode m = (ManuaCode)item.Tag;

                PackageOperations.AddTempFile(m.OpenManual(PackageOperations.GetCurrentDTE(), PackageOperations.ActiveProfile));
            }
        }

        private void lstConflicts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(lstConflicts.SelectedItems.Count == 1)
            {
                ListViewItem item = lstConflicts.SelectedItems[lstConflicts.SelectedIndices[0]];
                ManuaCode m = (ManuaCode)item.Tag;

                PackageOperations.AddTempFile(m.OpenManual(PackageOperations.GetCurrentDTE(), PackageOperations.ActiveProfile));                
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
                btnUse.Enabled = true;
            else
                btnUse.Enabled = false;
        }
    }
}
