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
    public partial class CodeFlowChangesForm : Form
    {
        private CodeFlowVersions changes;
        public CodeFlowChangesForm(CodeFlowVersions changes)
        {
            InitializeComponent();
            this.changes = changes;
        }

        private void CodeFlowChanges_Load(object sender, EventArgs e)
        {
            foreach (CodeFlowVersionInfo item in changes.Versions)
            {
                foreach (VersionChange ver in item.Changes)
                {
                    ListViewItem viewItem = new ListViewItem(item.Version.ToString());
                    viewItem.SubItems.Add(ver.Description);
                    lstChanges.Items.Add(viewItem);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
