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
using CodeFlow.CodeControl.Analyzer;

namespace CodeFlow.Forms
{
    public partial class CommitForm : Form
    {
        private ChangeList differences;
        private ConflictList conflictCode;
        public CommitForm(ChangeAnalyzer difs)
        {
            InitializeComponent();
            differences = difs.Modifications;
            conflictCode = difs.ModifiedConflict;
        }

        private void RefreshControls()
        {
            lblWarning.Visible = false;
            lblManual.Text = String.Format(Properties.Resources.CommitEntries, differences.AsList.Count, conflictCode.AsList.Count);
            lblServer.Text = PackageOperations.Instance.GetActiveProfile().ToString();

            if (!String.IsNullOrEmpty(PackageOperations.Instance.SolutionProps.ClientInfo.Version)
                && !String.IsNullOrEmpty(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion))
                lblSolutionVersion.Text = String.Format(Properties.Resources.SolutionVersion,
                    PackageOperations.Instance.SolutionProps.ClientInfo.Version, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion);
            else
                lblSolutionVersion.Text = String.Format(Properties.Resources.ProfileVersion, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion);

            if (PackageOperations.Instance.GetActiveProfile().GenioConfiguration.ProductionSystem)
            {
                lblProd.Text = String.Format(Properties.Resources.ProfileProd, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.GenioVersion);
                lblProd.ForeColor = Color.DarkRed;

                if (!String.IsNullOrEmpty(PackageOperations.Instance.SolutionProps.ClientInfo.Version)
                    && !String.IsNullOrEmpty(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion)
                    && !PackageOperations.Instance.SolutionProps.ClientInfo.Version.Equals(PackageOperations.Instance.GetActiveProfile().GenioConfiguration.BDVersion))
                {
                    lblWarning.Text = String.Format(Properties.Resources.WarningProfile);
                    lblWarning.Visible = true;
                }
            }
            else
            {
                lblProd.Text = String.Format(Properties.Resources.ProfileDev, PackageOperations.Instance.GetActiveProfile().GenioConfiguration.GenioVersion);
                lblProd.ForeColor = Color.DarkGreen;
            }
            RefreshButtons();
        }

        private void RefreshForm()
        {
            lstCode.Items.Clear();
            foreach(IChange diff in differences.AsList)
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

        private void btnCommit_Click(object sender, EventArgs e)
        {
            if (lstCode.CheckedItems.Count == 0
                || MessageBox.Show(Properties.Resources.ConfirmationExport, 
                    Properties.Resources.Export, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            ListView.CheckedListViewItemCollection items = lstCode.CheckedItems;
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            for (int i = 0; i < items.Count; i++)
            {
                if (!(items[i].Tag is IChange))
                    continue;
                IChange diff = (IChange)items[i].Tag;
                try
                {
                    IOperation operation = diff.GetOperation();
                    if (operation != null && PackageOperations.Instance.ExecuteOperation(operation))
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
            
            if (lstCode.Items.Count == 0)
                this.Close();

            RefreshControls();
        }

        private void ExportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PackageOperations.Instance.GetActiveProfile().GenioConfiguration.CloseConnection();
        }

        private void lstCode_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstCode.SelectedItems.Count == 1)
            {
                ListViewItem item = lstCode.Items[lstCode.SelectedIndices[0]];

                if (item.Tag is Conflict)
                    btnConflict_Click(sender, new EventArgs());

                else if (item.Tag is IChange)
                    btnMerge_Click(sender, new EventArgs());

                item.Checked = !item.Checked;
            }
        }

        private void lstCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool diff = false;
            bool conflict = false;
            foreach (ListViewItem item in lstCode.SelectedItems)
            {
                if(item.Tag is Conflict)
                    conflict = true;
                else if (!(item.Tag is CodeNotFound))
                    diff = true;
            }
            if (diff != conflict)
            {
                if (conflict)
                {
                    btnConflict.Enabled = true;
                    btnMerge.Enabled = false;
                    goToPositionToolStrip.Enabled = false;
                }
                else
                {
                    btnConflict.Enabled = false;
                    goToPositionToolStrip.Enabled = true;

                    if (lstCode.SelectedItems.Count == 1)
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
                    }
                    RefreshControls();
                }
            }
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            if(lstCode.SelectedItems.Count == 1
                && lstCode.SelectedItems[0].Tag is IChange diff)
            {
                if (diff is CodeNotFound)
                    return;

                IChange change = diff.Merge();
                if (change.HasDifference())
                {
                    lstCode.SelectedItems[0].Text = change.GetDescription();
                    lstCode.SelectedItems[0].SubItems[2].Text = change.Merged.ShortOneLineCode();
                    lstCode.SelectedItems[0].ForeColor = lblMerged.ForeColor;
                    lstCode.SelectedItems[0].ImageIndex = GetImageIndex(change);
                    lstCode.SelectedItems[0].Tag = change;
                }
                else
                    lstCode.Items.Remove(lstCode.SelectedItems[0]);
            }
            RefreshControls();
        }

        private void AddListItem(IChange diff, Color c, bool chk)
        {
            ListViewItem item = new ListViewItem(diff.GetDescription());
            item.SubItems.Add(diff.Merged.LocalFileName);
            item.SubItems.Add(diff.ChangeProfile.ProfileName);
            item.SubItems.Add(diff.Merged.ShortOneLineCode());
            item.ImageIndex = GetImageIndex(diff);
            item.Tag = diff;
            item.Checked = chk;
            item.ForeColor = c;
            lstCode.Items.Add(item);
        }

        private void AddListItem(Conflict conf, Color c, bool chk)
        {
            ListViewItem item = new ListViewItem(conf.DifferenceList.AsList[0].GetDescription());
            item.SubItems.Add(conf.DifferenceList.AsList[0].Merged.LocalFileName);
            item.SubItems.Add(conf.DifferenceList.AsList[0].ChangeProfile.ProfileName);
            item.SubItems.Add(conf.DifferenceList.AsList[0].Merged.ShortOneLineCode());
            item.ImageIndex = GetImageIndex(conf);
            item.Tag = conf;
            item.Checked = chk;
            item.ForeColor = c;
            lstCode.Items.Add(item);
        }

        private void lstCode_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            bool enable = false;
            foreach (ListViewItem item in lstCode.CheckedItems)
            {
                if (item.Tag is IChange
                    && !(item.Tag is CodeNotFound))
                {
                    enable = true;
                    break;
                }
            }

            btnCommit.Enabled = enable;
        }

        private void lstCode_KeyDown(object sender, KeyEventArgs e)
        {

            if (lstCode.SelectedItems.Count == 1)
            {
                if (e.KeyCode == Keys.Enter)
                    lstCode_MouseDoubleClick(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
                else if (e.KeyCode == Keys.Delete)
                {
                    if(lstCode.SelectedItems[0].Tag is IChange change)
                        differences.AsList.Remove(change);
                    else if(lstCode.SelectedItems[0].Tag is Conflict conflict)
                        conflictCode.AsList.Remove(conflict);
                    else
                        return;

                    lstCode.Items.Remove(lstCode.SelectedItems[0]);
                    RefreshControls();
                }
            }
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

        private void GoToManualCodePosition(IChange change)
        {
            PackageOperations.Instance.OpenOnPosition(change.Mine.FullFileName, change.Mine.LocalMatch.CodeStart);
        }

        private void goToPositionToolStrip_Click(object sender, EventArgs e)
        {
            object item = GetSelectedItem();
            if (item != null && item is IChange)
                GoToManualCodePosition(item as IChange);
        }

        private object GetSelectedItem()
        {
            if (lstCode.SelectedItems.Count == 1)
                return lstCode.Items[lstCode.SelectedIndices[0]].Tag;
            return null;
        }
    }
}
