namespace CodeFlow.ToolWindow
{
    using CodeFlow.CodeControl;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;

    /// <summary>
    /// Interaction logic for ChangeHistoryControl.
    /// </summary>
    public partial class ChangeHistoryControl : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeHistoryControl"/> class.
        /// </summary>
        public ChangeHistoryControl()
        {
            this.InitializeComponent();
            lstHistory.ItemsSource = PackageOperations.Instance.ChangeLog.OperationList;
            lstHistory.DataContext = this;
        }
        private void lstHistory_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstHistory.SelectedIndex >= 0)
            {
                try
                {
                    IOperation op = lstHistory.SelectedItem as IOperation;
                    Compare(op);
                }
                catch(Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message,
                        Properties.Resources.History, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        private void ViewChangeContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if(lstHistory.SelectedIndex >= 0)
            {
                try
                {
                    IOperation op = lstHistory.SelectedItem as IOperation;
                    Compare(op);
                }
                    catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message,
                        Properties.Resources.History, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        private void UndoContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstHistory.SelectedIndex >= 0)
            {
                try
                {
                    IOperation op = lstHistory.SelectedItem as IOperation;
                    if(op.Undo(PackageOperations.Instance.GetActiveProfile()))
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.OperationComplete,
                            Properties.Resources.History, System.Windows.Forms.MessageBoxButtons.OK, 
                            System.Windows.Forms.MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message,
                        Properties.Resources.History, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        private void RedoContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstHistory.SelectedIndex >= 0)
            {
                try
                {
                    IOperation op = lstHistory.SelectedItem as IOperation;
                    if (op.Redo(PackageOperations.Instance.GetActiveProfile()))
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.OperationComplete,
                            Properties.Resources.History, System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message,
                        Properties.Resources.History, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
        private void ClearContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            PackageOperations.Instance.ChangeLog.Clear();
            // Force collection, we might have to many changes and stuff might get heavy
            System.GC.Collect();
        }
        private void lstHistoryColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            if (column != null)
            {
                string sortBy = column.Tag.ToString();
                if (listViewSortCol != null)
                {
                    AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                    lstHistory.Items.SortDescriptions.Clear();
                }

                ListSortDirection newDir = ListSortDirection.Ascending;
                if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                    newDir = ListSortDirection.Descending;

                listViewSortCol = column;
                listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
                AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
                lstHistory.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
            }
        }
        private void Compare(IOperation op)
        {
            op.OperationChanges.Compare();
        }
    }
}