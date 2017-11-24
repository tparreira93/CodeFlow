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
    using System.Reflection;
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
        private bool caseSensitive = false;
        private bool wholeWord = false;
        private ComboBox combo = null;

        public ComboBox LangCombo { get => combo; set => combo = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FindManwinControl"/> class.
        /// </summary>
        public FindManwinControl()
        {
            this.InitializeComponent();
            lstFindMan.ItemsSource = results;
            lstFindMan.DataContext = this;
            foreach (object item in toolBar.Items)
            {
                if(item is ComboBox)
                {
                    LangCombo = item as ComboBox;
                    break;
                }
            }
            List<string> plats = PackageOperations.ActiveProfile.GenioConfiguration.Plataforms;
            SetComboData(plats);
        }

        public void SetComboData(List<string> data)
        {
            if (LangCombo != null)
            {
                LangCombo.Items.Clear();
                LangCombo.Items.Add("All");
                LangCombo.SelectedIndex = 0;
                foreach (string plat in data)
                    LangCombo.Items.Add(plat);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            btnSearch.IsEnabled = false;
            currentSearch = txtSearch.Text;
            string lang = ((string)LangCombo?.SelectedValue) ?? "";
            if (lang.Equals("All"))
                lang = "";

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string error = "";
                List<IManual> res = new List<IManual>();
                Profile p = PackageOperations.ActiveProfile;
                try
                {
                    res.AddRange(ManuaCode.Search(p, currentSearch, caseSensitive: caseSensitive, wholeWord: wholeWord, plataform: lang));
                    res.AddRange(CustomFunction.Search(p, currentSearch, caseSensitive: caseSensitive, wholeWord: wholeWord, plataform: lang));
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
                        System.Windows.MessageBox.Show(String.Format(Properties.Resources.ErrorSearch, error), Properties.Resources.Search,
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
                    Type t = results[lstFindMan.SelectedIndex].GetType();
                    IManual m = t.GetMethod("GetManual", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, new object[] { PackageOperations.ActiveProfile, results[lstFindMan.SelectedIndex].CodeId }) as IManual;

                    if (m == null)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.VerifyProfile, 
                            Properties.Resources.Search, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                    PackageOperations.OpenManualFile(m, true);

                    if (currentSearch == String.Empty)
                        return;

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

        private void caseCheck(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.Source as CheckBox;
            caseSensitive = checkBox?.IsChecked ?? false;
        }

        private void wholeChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.Source as CheckBox;
            wholeWord = checkBox?.IsChecked ?? false;
        }

        private void txtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                btnSearch_Click(sender, new RoutedEventArgs());
        }
    }
}