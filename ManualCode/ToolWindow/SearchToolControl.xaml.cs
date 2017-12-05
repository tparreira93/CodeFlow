namespace CodeFlow.ToolWindow
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for SearchToolControl.
    /// </summary>
    public partial class SearchToolControl : UserControl
    {
        private ObservableCollection<IManual> results = new ObservableCollection<IManual>();
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchToolControl"/> class.
        /// </summary>
        public SearchToolControl()
        {
            this.InitializeComponent();
            lstFindMan.ItemsSource = results;
            lstFindMan.DataContext = this;
        }

        public void PopulateList(List<IManual> lst)
        {
            results.Clear();
            foreach (IManual m in lst)
                results.Add(m);

            //FindListViewItem(lstFindMan);
        }

        private void lstFindMan_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstFindMan.SelectedIndex >= 0)
            {
                try
                {
                    Type t = results[lstFindMan.SelectedIndex].GetType();
                    IManual m = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { PackageOperations.GetActiveProfile(), results[lstFindMan.SelectedIndex].CodeId }) as IManual;

                    if (m == null)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.VerifyProfile,
                            Properties.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    PackageOperations.OpenManualFile(m, true);

                    if (String.IsNullOrEmpty(PackageOperations.CurrentSearch))
                        return;

                    EnvDTE.Find find = PackageOperations.DTE.Find;
                    find.Action = EnvDTE.vsFindAction.vsFindActionFind;
                    find.MatchWholeWord = PackageOperations.WholeWordSearch;
                    find.MatchCase = PackageOperations.CaseSensitive;
                    find.FindWhat = PackageOperations.CurrentSearch;
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
                    Regex regex = new Regex("(" + PackageOperations.CurrentSearch + ")", RegexOptions.IgnoreCase);
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
    }
}