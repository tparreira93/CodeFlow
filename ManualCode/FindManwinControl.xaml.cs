namespace CodeFlow
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for FindManwinControl.
    /// </summary>
    public partial class FindManwinControl : System.Windows.Controls.UserControl
    {

        private ObservableCollection<IManual> results = new ObservableCollection<IManual>();
        private string currentSearch = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="FindManwinControl"/> class.
        /// </summary>
        public FindManwinControl()
        {
            this.InitializeComponent();
            lstFindMan.ItemsSource = results;
            lstFindMan.DataContext = this;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnSearch.IsEnabled = false;
            currentSearch = txtSearch.Text;
            ItemCollection items = toolBar.Items;

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string error = "";
                List<IManual> res = new List<IManual>();
                try
                {
                    res.AddRange(ManuaCode.Search(PackageOperations.ActiveProfile, currentSearch));
                    res.AddRange(CustomFunction.Search(PackageOperations.ActiveProfile, currentSearch));
                }
                catch(Exception ex)
                {
                    error = ex.Message;
                }


                // Update UI 
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    results.Clear();
                    foreach (IManual m in res)
                        results.Add(m);

                    btnSearch.IsEnabled = true;

                    if(error.Length != 0)
                    {
                        System.Windows.MessageBox.Show(Properties.Resources.ErrorSearch, Properties.Resources.Search,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }), DispatcherPriority.Background);
            });
        }

        private void lstFindMan_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstFindMan.SelectedIndex >= 0)
            {
                try
                {
                    IManual m;
                    if (results[lstFindMan.SelectedIndex] is ManuaCode)
                        m = ManuaCode.GetManual(results[lstFindMan.SelectedIndex].CodeId, PackageOperations.ActiveProfile);
                    else
                        m = CustomFunction.GetManual(results[lstFindMan.SelectedIndex].CodeId, PackageOperations.ActiveProfile);

                    if (m == null)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.VerifyProfile, 
                            Properties.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    PackageOperations.AddTempFile(m.OpenManual(PackageOperations.DTE, PackageOperations.ActiveProfile));

                    Find find = PackageOperations.DTE.Find;
                    find.Action = vsFindAction.vsFindActionFind;
                    find.Backwards = false;
                    find.MatchCase = false;
                    find.FindWhat = currentSearch;
                    find.ResultsLocation = vsFindResultsLocation.vsFindResults2;
                    find.Target = vsFindTarget.vsFindTargetCurrentDocument;
                    find.PatternSyntax = vsFindPatternSyntax.vsFindPatternSyntaxLiteral;
                    find.Target = vsFindTarget.vsFindTargetCurrentDocument;
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

        void ChangeColor(object sender, RoutedEventArgs e)
        {
            CheckBox b = e.Source as CheckBox;
            b.Background = new SolidColorBrush(Colors.Black);
        }

        private void lstFindMan_Initialized(object sender, EventArgs e)
        {
            lstFindMan.ItemsSource = results;
        }
    }
}