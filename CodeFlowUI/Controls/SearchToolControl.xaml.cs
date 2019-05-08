namespace CodeFlowUI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;
    using CodeFlowLibrary.Bridge;
    using CodeFlowLibrary.GenioCode;
    using CodeFlowLibrary.Settings;
    using CodeFlowUI.Controls.Editor;

    /// <summary>
    /// Interaction logic for SearchToolControl.
    /// </summary>
    public partial class SearchToolControl : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private ObservableCollection<IManual> results = new ObservableCollection<IManual>();
        public SearchOptions searchOptions = new SearchOptions();
        public ICodeEditor CodeEditor { get; private set; }
        public bool ShowPreview { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchToolControl"/> class.
        /// </summary>
        public SearchToolControl(bool showPreview, ICodeEditor editor = null)
        {
            this.InitializeComponent();
            lstCode.ItemsSource = results;
            lstCode.DataContext = this;
            UpdateOptions(showPreview, editor);
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
                    IManual m = Manual.GetManual(t, (lstCode.SelectedItem as IManual).CodeId, PackageBridge.Flow.Active);

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
            await PackageBridge.Flow.FileOps.OpenTempFileAsync(manual, PackageBridge.Flow.Active, true);

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

        public void UpdateOptions(bool showPreview, ICodeEditor editor)
        {
            ShowPreview = showPreview;
            CodeEditor = editor;

            Visibility vis = Visibility.Collapsed;
            if (showPreview)
            {
                Preview.Content = CodeEditor.GetUIControl();
                vis = Visibility.Visible;
            }
            Preview.Visibility = vis;
            Splitter.Visibility = vis;
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
                man.ShowSVNLog(PackageBridge.Flow.Active);
            }
        }

        private void BlameContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstCode.SelectedIndex >= 0)
            {
                IManual man = lstCode.SelectedItem as IManual;
                man.Blame(PackageBridge.Flow.Active);
            }
        }

        private void ClearContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            results.Clear();
        }

        private void LstCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstCode.SelectedIndex >= 0 && ShowPreview && CodeEditor != null)
            {
                Type t = lstCode.SelectedItem.GetType();
                IManual man = lstCode.SelectedItem as IManual;
                man = Manual.GetManual(t, man.CodeId, PackageBridge.Flow.Active);
                var previewer = Preview.Content;
                var newPreviewer = CodeEditor.GetUIControl(PackageBridge.Flow.Active, man, searchOptions);

                if (previewer != newPreviewer)
                    Preview.Content = newPreviewer;
            }
        }
    }
}