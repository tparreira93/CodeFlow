using CodeFlow.GenioManual;

namespace CodeFlow.ToolWindow
{
    using CodeFlow.ManualOperations;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for SearchToolControl.
    /// </summary>
    public partial class SearchToolControl : UserControl
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private ObservableCollection<IManual> results = new ObservableCollection<IManual>();
        private string currentSearch = "";
        private bool wholeWord = false;
        private bool caseSensitive = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchToolControl"/> class.
        /// </summary>
        public SearchToolControl()
        {
            this.InitializeComponent();
            lstCode.ItemsSource = results;
            lstCode.DataContext = this;

            
            /*if (lstCode.View is GridView grid)
            {
                foreach (GridViewColumn item in grid.Columns)
                {
                    GridViewColumnHeader column = item.Header as GridViewColumnHeader;
                    if (column == null)
                        continue;

                    if (column.Tag.Equals("Type"))
                        column.Width = Properties.Settings.Default.ColTypeSize;

                    else if (column.Tag.Equals("Tag"))
                        column.Width = Properties.Settings.Default.ColTagSize;

                    else if (column.Tag.Equals("Tipo"))
                        column.Width = Properties.Settings.Default.ColTipoSize;

                    else if (column.Tag.Equals("Plataform"))
                        column.Width = Properties.Settings.Default.ColPlatSize;

                    else if (column.Tag.Equals("OneLineCode"))
                        column.Width = Properties.Settings.Default.ColCodeSize;
                }
            }*/
        }

        public void RefreshteList(List<IManual> lst, string currentSearch, bool wholeWord, bool caseSensitive)
        {
            this.currentSearch = currentSearch;
            this.wholeWord = wholeWord;
            this.caseSensitive = caseSensitive;
            results.Clear();
            foreach (IManual m in lst)
                results.Add(m);

            FindListViewItem(lstCode);
        }

        private void lstFindMan_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstCode.SelectedIndex >= 0)
            {
                try
                {
                    Type t = lstCode.SelectedItem.GetType();
                    IManual m = Manual.GetManual(t, (lstCode.SelectedItem as IManual).CodeId, 
                                                    PackageOperations.Instance.GetActiveProfile());

                    if (m == null)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.VerifyProfile,
                            Properties.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    PackageOperations.Instance.OpenManualFile(m, true);

                    if (String.IsNullOrEmpty(currentSearch))
                        return;

                    EnvDTE.Find find = PackageOperations.Instance.DTE.Find;
                    find.Action = EnvDTE.vsFindAction.vsFindActionFind;
                    find.MatchWholeWord = wholeWord;
                    find.MatchCase = caseSensitive;
                    find.FindWhat = currentSearch;
                    find.Target = EnvDTE.vsFindTarget.vsFindTargetCurrentDocument;
                    find.PatternSyntax = EnvDTE.vsFindPatternSyntax.vsFindPatternSyntaxLiteral;
                    find.Backwards = false;
                    find.KeepModifiedDocumentsOpen = true;
                    find.Execute();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(String.Format(Properties.Resources.ErrorRequest, ex.Message),
                        Properties.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
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
                    Regex regex = new Regex("(" + currentSearch + ")");
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
                man.ShowSVNLog(PackageOperations.Instance.GetActiveProfile());
            }
        }

        private void BlameContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (lstCode.SelectedIndex >= 0)
            {
                IManual man = lstCode.SelectedItem as IManual;
                man.Blame(PackageOperations.Instance.GetActiveProfile());
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