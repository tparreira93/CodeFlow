using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using CodeFlow.Versions;
using Version = CodeFlowLibrary.Versions.Version;
using CodeFlowLibrary.Versions;

namespace CodeFlow.Forms
{
    public partial class CodeFlowChangesForm : Form
    {
        private readonly CodeFlowUpdater _changes;
        private readonly Version _currentVersion;
        private readonly Version _previousVersion;

        public CodeFlowChangesForm(CodeFlowUpdater changes, Version currentVersion)
        {
            InitializeComponent();
            _changes = changes;
            _currentVersion = currentVersion;
        }

        public CodeFlowChangesForm(CodeFlowUpdater changes, Version currentVersion, Version previousVersion)
        {
            InitializeComponent();
            _changes = changes;
            _currentVersion = currentVersion;
            _previousVersion = previousVersion;
        }

        private void CodeFlowChanges_Load(object sender, EventArgs e)
        {
            lblVersion.Text = $"Current version is {_currentVersion}";
            var codeFlowVersionInfos = _changes.Versions.OrderByDescending(x => x.Version);
            foreach (CodeFlowVersion item in codeFlowVersionInfos)
            {
                foreach (CodeFlowOptionsCommand ver in item.Changes)
                {
                    ListViewItem viewItem = new ListViewItem(item.Version.ToString());
                    viewItem.SubItems.Add(ver.Description);
                    if (_previousVersion != null && _previousVersion.IsBefore(item.Version))
                    {
                        viewItem.ForeColor = Color.DarkGreen;
                        viewItem.ImageIndex = 0;
                    }
                    lstChanges.Items.Add(viewItem);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
