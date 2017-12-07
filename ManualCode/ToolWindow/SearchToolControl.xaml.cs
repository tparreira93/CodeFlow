namespace CodeFlow.ToolWindow
{
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
                    IManual m = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { PackageOperations.GetActiveProfile(), results[lstCode.SelectedIndex].CodeId }) as IManual;

                    if (m == null)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.VerifyProfile,
                            Properties.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    PackageOperations.OpenManualFile(m, true);

                    if (String.IsNullOrEmpty(currentSearch))
                        return;

                    EnvDTE.Find find = PackageOperations.DTE.Find;
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

        public class SortAdorner : Adorner
        {
            private static Geometry ascGeometry =
                    Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

            private static Geometry descGeometry =
                    Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

            public ListSortDirection Direction { get; private set; }

            public SortAdorner(UIElement element, ListSortDirection dir)
                    : base(element)
            {
                this.Direction = dir;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (AdornedElement.RenderSize.Width < 20)
                    return;

                TranslateTransform transform = new TranslateTransform
                        (
                                AdornedElement.RenderSize.Width - 15,
                                (AdornedElement.RenderSize.Height - 5) / 2
                        );
                drawingContext.PushTransform(transform);

                Geometry geometry = ascGeometry;
                if (this.Direction == ListSortDirection.Descending)
                    geometry = descGeometry;
                drawingContext.DrawGeometry(Brushes.Black, null, geometry);

                drawingContext.Pop();
            }
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

        private void lstColumnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            if(e.Source is GridViewColumnHeader column)
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
            }
            */
        }
    }
}