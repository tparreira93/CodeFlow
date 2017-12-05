using CodeFlow.CodeControl;
using CodeFlow.ManualOperations;
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
        private DifferenceList differences;
        private ConflictList conflictCode;
        public CommitForm(DifferencesAnalyzer difs)
        {
            InitializeComponent();
            differences = difs.Differences;
            conflictCode = difs.ManualConflict;
        }


        private void RefreshControls()
        {
            lblWarning.Visible = false;
            lblManual.Text = String.Format(Properties.Resources.CommitEntries, differences.AsList.Count, conflictCode.AsList.Count);
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
        }

        private void RefreshForm()
        {
            lstCode.Items.Clear();
            foreach(Difference diff in differences.AsList)
                AddListItem(diff, diff.IsMerged ? lblMerged.ForeColor : lblNotMerged.ForeColor, true);

            foreach (Conflict pair in conflictCode.AsList)
                AddListItem(pair, lblConflict.ForeColor, false);

            RefreshControls();
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
            ListView.CheckedListViewItemCollection items = lstCode.CheckedItems;
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i].Tag is Difference diff))
                    continue;

                DialogResult result = CommitMergeCode(diff);
                if (result == DialogResult.Yes)
                {
                    differences.AsList.Remove(diff);
                    itemsToRemove.Add(items[i]);
                }
                else if (result == DialogResult.Cancel)
                    break;
            }

            foreach (ListViewItem item in itemsToRemove)
                lstCode.Items.Remove(item);

            RefreshControls();

            if (lstCode.Items.Count == 0)
                this.Close();
        }

        private DialogResult CommitMergeCode(Difference diff)
        {
            DialogResult funcResult = DialogResult.Yes;
            IManual man = null;
            try
            {
                man = Manual.Merge(diff.Database, diff.Local);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnCommit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.ConfirmationExport,
                Properties.Resources.Export, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            ListView.CheckedListViewItemCollection items = lstCode.CheckedItems;
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i].Tag is Difference))
                    continue;
                Difference diff = (Difference)items[i].Tag;
                try
                {
                    if (diff.Local.Update(PackageOperations.GetActiveProfile()))
                    {
                        differences.AsList.Remove(diff);
                        itemsToRemove.Add(items[i]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorUpdating, ex.Message),
                        Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }

            foreach (ListViewItem item in itemsToRemove)
                lstCode.Items.Remove(item);

            RefreshControls();

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

                if (item.Tag is Conflict conflict)
                    btnConflict_Click(sender, new EventArgs());

                else if (item.Tag is Difference)
                    btnMerge_Click(sender, new EventArgs());
            }
        }

        private void lstCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool diff = false;
            bool conflict = false;
            foreach (ListViewItem item in lstCode.SelectedItems)
            {
                if (item.Tag is Difference)
                    diff = true;
                else
                    conflict = true;
            }
            if (diff != conflict)
            {
                if (conflict)
                {
                    btnConflict.Enabled = true;
                    btnMerge.Enabled = false;
                }
                else
                {
                    btnConflict.Enabled = false;

                    if(lstCode.SelectedItems.Count == 1)
                        btnMerge.Enabled = true;
                }
            }
            else
            {
                btnConflict.Enabled = false;
                btnMerge.Enabled = false;
            }
        }

        // Nao se resolvem conflitos de CustomFunctions
        private void btnConflict_Click(object sender, EventArgs e)
        {
            if (lstCode.SelectedItems.Count == 1)
            {
                if (lstCode.Items[lstCode.SelectedIndices[0]].Tag is Conflict conflict)
                {
                    ConflictForm conflictHandler = new ConflictForm(conflict);
                    conflictHandler.UpdateForm += OnConflictResolve;
                    conflictHandler.ShowDialog(this);
                    RefreshControls();
                }
            }
        }

        private void OnConflictResolve(object sender, ConflictResolveArgs args)
        {
            if (args != null)
            {
                ListViewItem item = null;
                foreach (ListViewItem it in lstCode.Items)
                {
                    if (it.Tag is Conflict
                        && ((Conflict)it.Tag).Id.Equals(args.Conflict.Id))
                    {
                        item = it;
                        break;
                    }
                }

                if (item != null)
                {
                    lstCode.Items.Remove(item);
                    conflictCode.AsList.Remove(args.Conflict);

                    if (args.Keep.HasDifference())
                    {
                        AddListItem(args.Keep, args.Keep.IsMerged ? lblMerged.ForeColor : lblNotMerged.ForeColor, true);
                        differences.AsList.Add(args.Keep);
                        RefreshControls();
                    }
                }
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            if(lstCode.SelectedItems.Count == 1
                && lstCode.SelectedItems[0].Tag is Difference diff)
            {
                diff.Merge();
                if (diff.HasDifference())
                {
                    lstCode.SelectedItems[0].Text = diff.Local.ShortOneLineCode();
                    lstCode.SelectedItems[0].ForeColor = lblMerged.ForeColor;
                }
                else
                    lstCode.Items.Remove(lstCode.SelectedItems[0]);
            }
        }

        private void AddListItem(Difference diff, Color c, bool chk)
        {
            ListViewItem item = new ListViewItem(diff.Local.ShortOneLineCode());
            item.SubItems.Add(diff.Local.LocalFileName);
            item.Tag = diff;
            item.Checked = chk;
            item.ForeColor = c;
            lstCode.Items.Add(item);
        }

        private void AddListItem(Conflict conf, Color c, bool chk)
        {
            ListViewItem item = new ListViewItem(conf.DifferenceList.AsList[0].Local.ShortOneLineCode());
            item.SubItems.Add(conf.DifferenceList.AsList[0].Local.LocalFileName);
            item.Tag = conf;
            item.Checked = chk;
            item.ForeColor = c;
            lstCode.Items.Add(item);
        }

        private void lstCode_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            bool enable = false;
            if (lstCode.CheckedItems.Count > 0)
            {
                foreach (ListViewItem item in lstCode.CheckedItems)
                {
                    if(item.Tag is Difference)
                    {
                        enable = true;
                        break;
                    }
                }
            }

            btnCommit.Enabled = enable;
            btnCompare.Enabled = enable;
        }
    }
}
