using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow
{
    public partial class CommitForm : Form
    {
        private List<IManual> exportCode;
        private Dictionary<Guid, List<ManuaCode>> conflictCode;
        public CommitForm()
        {
            InitializeComponent();
            exportCode = new List<IManual>();
            conflictCode = new Dictionary<Guid, List<ManuaCode>>();
        }
        public CommitForm(List<IManual> manual)
        {
            InitializeComponent();
            exportCode = manual;
            conflictCode = new Dictionary<Guid, List<ManuaCode>>();
        }
        public CommitForm(List<IManual> manual, Dictionary<Guid, List<ManuaCode>> conflicts)
        {
            InitializeComponent();
            exportCode = manual;
            conflictCode = conflicts;
        }

        private void RefreshControls()
        {
            lblWarning.Visible = false;
            lblManual.Text = String.Format(Properties.Resources.CommitEntries,exportCode.Count, conflictCode.Count);
            lblServer.Text = PackageOperations.GetActiveProfile().ToString();

            if (!String.IsNullOrEmpty(PackageOperations.SolutionProps.ClientInfo.Version)
                && !String.IsNullOrEmpty(PackageOperations.GetActiveProfile().GenioConfiguration.BDVersion))
                lblSolutionVersion.Text = String.Format(Properties.Resources.SolutionVersion, 
                    PackageOperations.SolutionProps.ClientInfo.Version, PackageOperations.GetActiveProfile().GenioConfiguration.BDVersion);
            else
                lblSolutionVersion.Text = String.Format(Properties.Resources.ProfileVersion, PackageOperations.GetActiveProfile().GenioConfiguration.BDVersion);

            if (PackageOperations.GetActiveProfile().GenioConfiguration.ProductionSystem)
            {
                lblProd.Text = String.Format(Properties.Resources.ProfileProd, PackageOperations.GetActiveProfile().GenioConfiguration.GenioVersion);
                lblProd.ForeColor = Color.DarkRed;

                if (!String.IsNullOrEmpty(PackageOperations.SolutionProps.ClientInfo.Version)
                    && !String.IsNullOrEmpty(PackageOperations.GetActiveProfile().GenioConfiguration.BDVersion)
                    && !PackageOperations.SolutionProps.ClientInfo.Version.Equals(PackageOperations.GetActiveProfile().GenioConfiguration.BDVersion))
                {
                    lblWarning.Text = String.Format(Properties.Resources.WarningProfile);
                    lblWarning.Visible = true;
                }
            }
            else
            {
                lblProd.Text = String.Format(Properties.Resources.ProfileDev, PackageOperations.GetActiveProfile().GenioConfiguration.GenioVersion);
                lblProd.ForeColor = Color.DarkGreen;
            }
            btnCompare.Enabled = false;
            btnConflict.Enabled = false;
            btnExport.Enabled = false;
        }

        private void RefreshForm()
        {
            lstCode.Items.Clear();
            for (int i = 0; i < exportCode.Count; i++)
            {
                ListViewItem item = new ListViewItem(exportCode[i].ShortOneLineCode());
                item.Tag = exportCode[i];
                lstCode.Items.Add(item);
            }
            foreach (KeyValuePair<Guid, List<ManuaCode>> pair in conflictCode)
            {
                ListViewItem item = new ListViewItem(pair.Value[0].ShortOneLineCode());
                item.Tag = pair;
                item.ForeColor = Color.DarkRed;
                lstCode.Items.Add(item);
            }
            RefreshControls();
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            ProfilesForm profilesForm = new ProfilesForm();
            profilesForm.ShowDialog();
            RefreshForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {
            RefreshForm();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection items = lstCode.SelectedItems;
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            List<IManual> manualToRemove = new List<IManual>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i].Tag is IManual))
                    continue;
                IManual man = (IManual)items[i].Tag;

                DialogResult result = CompareCode(man);
                if (result == DialogResult.Yes)
                    exportCode.Remove(man);
                else if(result == DialogResult.Cancel)
                    break;
            }

            RefreshForm();

            if (lstCode.Items.Count == 0)
                this.Close();
        }

        private DialogResult CompareCode(IManual man)
        {
            DialogResult funcResult = DialogResult.Yes;
            try
            {
                Type t = man.GetType();
                IManual bd = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { PackageOperations.GetActiveProfile(), man.CodeId }) as IManual;

                if (bd == null)
                {
                    MessageBox.Show(String.Format(Properties.Resources.VerifyProfile), Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    funcResult = DialogResult.Cancel;
                }
                else
                    man = Manual.Merge(man, bd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorComparing, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                funcResult = DialogResult.Cancel;
            }

            if (funcResult != DialogResult.Yes)
                return funcResult;

            funcResult = MessageBox.Show(Properties.Resources.ExportedMerged,
                Properties.Resources.Export, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            try
            {
                if (funcResult == DialogResult.Yes)
                {
                    man.Update(PackageOperations.GetActiveProfile());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorUpdating, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                funcResult = DialogResult.Cancel;
            }

            return funcResult;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show(Properties.Resources.ConfirmationExport, Properties.Resources.Export, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            ListView.ListViewItemCollection items = lstCode.Items;
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            List<Manual> manualToRemove = new List<Manual>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i].Tag is IManual))
                    continue;
                IManual man = (IManual)items[i].Tag;
                try
                {
                    if (man.Update(PackageOperations.GetActiveProfile()))
                        exportCode.Remove(man);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorUpdating, ex.Message),
                        Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }

            RefreshForm();

            if (lstCode.Items.Count == 0)
                this.Close();
        }

        private void ExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PackageOperations.GetActiveProfile().GenioConfiguration.CloseConnection();
        }

        private void lstCode_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstCode.SelectedItems.Count == 1)
            {
                ListViewItem item = lstCode.Items[lstCode.SelectedIndices[0]];
                if (item.Tag is KeyValuePair<Guid, List<ManuaCode>> conflict)
                {
                    ConflictHandler conflictHandler = new ConflictHandler(conflict.Value);
                    conflictHandler.UpdateForm += onConflictResolve;
                    conflictHandler.Show(this);
                }
                else if (item.Tag is IManual)
                {
                    DialogResult result = CompareCode((IManual)item.Tag);
                    if(result == DialogResult.Yes)
                        exportCode.Remove((IManual)item.Tag);
                }
            }

            RefreshForm();

            if (lstCode.Items.Count == 0)
                this.Close();
        }

        private void lstCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool manual = false;
            bool conflict = false;
            foreach (ListViewItem item in lstCode.SelectedItems)
            {
                if (item.Tag is IManual)
                    manual = true;
                else
                    conflict = true;
            }
            if (manual != conflict)
            {
                if (conflict)
                {
                    btnCompare.Enabled = false;
                    btnExport.Enabled = false;
                    btnConflict.Enabled = true;
                }
                else
                {
                    btnCompare.Enabled = true;
                    btnExport.Enabled = true;
                    btnConflict.Enabled = false;
                }
            }
            else
            {
                btnCompare.Enabled = false;
                btnExport.Enabled = false;
                btnConflict.Enabled = false;
            }
        }

        // Nao se resolvem conflitos de CustomFunctions
        private void btnConflict_Click(object sender, EventArgs e)
        {
            if (lstCode.SelectedItems.Count == 1)
            {
                ListViewItem item = lstCode.Items[lstCode.SelectedIndices[0]];
                if (item.Tag is KeyValuePair<Guid, List<ManuaCode>> conflict)
                {
                    ConflictHandler conflictHandler = new ConflictHandler(conflict.Value);
                    conflictHandler.UpdateForm += onConflictResolve;
                    conflictHandler.Show(this);
                }
            }
        }

        // Nao se resolvem conflitos de CustomFunctions
        private void onConflictResolve(object sender, ManuaCode code)
        {
            if (code != null)
            {
                ListViewItem item = null;
                foreach (ListViewItem it in lstCode.Items)
                {
                    if (it.Tag is KeyValuePair<Guid, List<ManuaCode>>
                        && ((KeyValuePair<Guid, List<ManuaCode>>)it.Tag).Key.Equals(code.CodeId))
                    {
                        item = it;
                        break;
                    }
                }

                if (item != null)
                {
                    lstCode.Items.Remove(item);
                    item = new ListViewItem(code.ShortOneLineCode());
                    item.Tag = code;
                    lstCode.Items.Add(item);
                    exportCode.Add(code);
                    conflictCode.Remove(code.CodeId);
                    RefreshControls();
                }
            }
        }
    }
}
