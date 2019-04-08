namespace CodeFlowUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using CodeFlowBridge;
    using CodeFlowLibrary.GenioCode;
    using CodeFlowLibrary.Settings;

    /// <summary>
    /// Interaction logic for SearchToolControl.
    /// </summary>
    public partial class SearchToolControl : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private ObservableCollection<IManual> results = new ObservableCollection<IManual>();
        public SearchOptions searchOptions = new SearchOptions();
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchToolControl"/> class.
        /// </summary>
        public SearchToolControl()
        {
            this.InitializeComponent();
            lstCode.ItemsSource = results;
            lstCode.DataContext = this;
        }

        public void Clear()
        {
            results.Clear();
        }

        public void RefreshteList(List<IManual> lst, SearchOptions options)
        {
            searchOptions = options;

            Clear();
            foreach (IManual m in lst)
                results.Add(m);

            FindListViewItem(lstCode);
        }

        private void lstFindMan_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Dispatcher.VerifyAccess();
            if (lstCode.SelectedIndex >= 0)
            {
                try
                {
                    Type t = lstCode.SelectedItem.GetType();
                    IManual m = Manual.GetManual(t, (lstCode.SelectedItem as IManual).CodeId, PackageBridge.Instance.GetActiveProfile());

                    if (m == null)
                    {
                        System.Windows.Forms.MessageBox.Show(CodeFlowResources.Resources.VerifyProfile,
                            CodeFlowResources.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }

                    if (String.IsNullOrEmpty(searchOptions.SearchTerm))
                        return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    PreviewManual(m, searchOptions);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorRequest, ex.Message),
                        CodeFlowResources.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        private async Task PreviewManual(IManual manual, SearchOptions searchOptions)
        {
            PackageBridge.Instance.OpenManualFile(manual, true);

            await PackageBridge.Flow.FindCodeAsync(searchOptions);
        }

        private void lstCodeColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lstCode.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lstCode.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        public void FindListViewItem(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                ListViewItem lv = obj as ListViewItem;
                if (lv != null)
                {
                    HighlightText(lv);
                }
                FindListViewItem(VisualTreeHelper.GetChild(obj as DependencyObject, i));
            }
        }

        private void HighlightText(Object itx)
        {
            if (itx != null)
            {
                if (itx is TextBlock)
                {
                    Regex regex = new Regex("(" + searchOptions.SearchTerm + ")");
                    TextBlock tb = itx as TextBlock;
                    string[] substrings = regex.Split(tb.Text);
                    tb.Inlines.Clear();
                    foreach (var item in substrings)
                    {
                        if (regex.Match(item).Success)
                        {
                            Run runx = new Run(item);
                            runx.Background = Brushes.Yellow;
                            tb.Inlines.Add(runx);
                        }
                        else
                        {
                            tb.Inlines.Add(item);
                        }
                    }
                    return;
                }
                else
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
                    {
                        HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i));
                    }
                }
            }
        }

        private void ShowSVNLogContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstCode.SelectedIndex >= 0)
            {
                IManual man = lstCode.SelectedItem as IManual;
                man.ShowSVNLog(PackageBridge.Instance.GetActiveProfile());
            }
        }

        private void BlameContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstCode.SelectedIndex >= 0)
            {
                IManual man = lstCode.SelectedItem as IManual;
                man.Blame(PackageBridge.Instance.GetActiveProfile());
            }
        }

        private void ClearContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            results.Clear();
        }

        private void lstColumnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*if(e.Source is GridViewColumnHeader column)
            {
                if (column.Tag.Equals("Type"))
                    Properties.Settings.Default.ColTypeSize = column.ActualWidth;

                else if (column.Tag.Equals("Tag"))
                    Properties.Settings.Default.ColTagSize = column.ActualWidth;

                else if (column.Tag.Equals("Tipo"))
                    Properties.Settings.Default.ColTipoSize = column.ActualWidth;

                else if (column.Tag.Equals("Plataform"))
                    Properties.Settings.Default.ColPlatSize = column.ActualWidth;

                else if (column.Tag.Equals("OneLineCode"))
                    Properties.Settings.Default.ColCodeSize = column.ActualWidth;

                Properties.Settings.Default.Save();
            }*/
        }
    }
}