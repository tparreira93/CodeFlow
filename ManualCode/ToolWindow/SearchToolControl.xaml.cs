namespace CodeFlow.ToolWindow
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

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
                    find.ResultsLocation = EnvDTE.vsFindResultsLocation.vsFindResults2;
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
    }
}