using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow
{
    public partial class ExportForm : Form
    {
        private List<IManual> exportCode;
        private Dictionary<Guid, List<ManuaCode>> conflictCode;

        public ExportForm()
        {
            InitializeComponent();
            exportCode = new List<IManual>();
            conflictCode = new Dictionary<Guid, List<ManuaCode>>();
        }
        public ExportForm(List<IManual> manual)
        {
            InitializeComponent();
            exportCode = manual;
            conflictCode = new Dictionary<Guid, List<ManuaCode>>();
        }
        public ExportForm(List<IManual> manual, Dictionary<Guid, List<ManuaCode>> conflicts)
        {
            InitializeComponent();
            exportCode = manual;
            conflictCode = conflicts;
        }

        private void RefreshForm()
        {
            lstCode.Items.Clear();
            for (int i = 0; i < exportCode.Count; i++)
            {
                ListViewItem item = new ListViewItem(exportCode[i].ShortCode);
                item.Tag = exportCode[i];
                lstCode.Items.Add(item);
            }
            foreach (KeyValuePair<Guid, List<ManuaCode>> pair in conflictCode)
            {
                ListViewItem item = new ListViewItem(pair.Value[0].ShortCode);
                item.Tag = pair;
                item.ForeColor = Color.DarkRed;
                lstCode.Items.Add(item);
            }

            lblManual.Text = String.Format("There {0} {1} manual code entrie{2} to export and {3} conflicts!",
                exportCode.Count > 1 ? "are" : "is", exportCode.Count > 1 ? "s" : "", exportCode.Count, conflictCode.Count);

            lblServer.Text = PackageOperations.ActiveProfile.ToString();
            btnCompare.Enabled = false;
            btnConflict.Enabled = false;
            btnExport.Enabled = false;
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
                if (!(items[i].Tag is Manual))
                    continue;
                IManual man = (IManual)items[i].Tag;

                if (CompareCode(man))
                    exportCode.Remove(man);
                else
                    break;
            }

            RefreshForm();
        }

        private bool CompareCode(IManual man)
        {
            IManual bd;
            bool funcResult = true;
            try
            {
                if (man is ManuaCode)
                    bd = ManuaCode.GetManual(man.CodeId, PackageOperations.ActiveProfile);
                else
                    bd = CustomFunction.GetManual(man.CodeId, PackageOperations.ActiveProfile);

                if (bd == null)
                {
                    MessageBox.Show(String.Format(Properties.Resources.VerifyProfile), Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    funcResult = false;
                }
                else
                    man = Manual.Merge(man, bd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorComparing, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                funcResult = false;
            }

            if (!funcResult)
                return funcResult;

            DialogResult result = MessageBox.Show(Properties.Resources.ExportedMerged,
                Properties.Resources.Export, MessageBoxButtons.YesNoCancel);

            try
            {
                if (result == DialogResult.Yes)
                    return man.Update(PackageOperations.ActiveProfile);
                else if (result == DialogResult.Cancel)
                    funcResult = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorUpdating, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                funcResult = false;
            }

            return funcResult;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DialogResult result;

            result = MessageBox.Show(Properties.Resources.ConfirmationExport, Properties.Resources.Export, MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
                return;

            ListView.ListViewItemCollection items = lstCode.Items;
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            List<Manual> manualToRemove = new List<Manual>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i].Tag is Manual))
                    continue;
                Manual man = (Manual)items[i].Tag;
                try
                {
                    if (man.Update(PackageOperations.ActiveProfile))
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
        }

        private void ExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PackageOperations.ActiveProfile.GenioConfiguration.CloseConnection();
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
                else if (item.Tag is Manual)
                {
                    if (CompareCode((Manual)item.Tag))
                        exportCode.Remove((Manual)item.Tag);
                }
            }
        }

        private void lstCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool manual = false;
            bool conflict = false;
            foreach (ListViewItem item in lstCode.SelectedItems)
            {
                if (item.Tag is KeyValuePair<Guid, List<Manual>>)
                    conflict = true;
                else
                    manual = true;
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
                    try
                    {
                        ManuaCode bd = ManuaCode.GetManual(code.CodeId, PackageOperations.ActiveProfile);
                        if (bd == null)
                        {
                            MessageBox.Show(Properties.Resources.VerifyProfile,
                                Properties.Resources.Conflict, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (!bd.Code.Equals(code.Code))
                        {
                            item = new ListViewItem(code.ShortCode);
                            item.Tag = code;
                            lstCode.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format(Properties.Resources.ErrorResolvingConflict, ex.Message),
                            Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
