using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using CodeFlow.Properties;
using CodeFlow.SolutionOperations;
using System.Collections.Generic;
using CodeFlow.Commands;
using CodeFlow.Versions;
using Version = CodeFlowLibrary.Versions.Version;
using System.Windows.Threading;
using CodeFlow.ToolWindow;
using Microsoft;
using EnvDTE80;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using CodeFlowLibrary.Versions;
using CodeFlowLibrary.CodeControl.Analyzer;
using CodeFlowLibrary.CodeControl.Changes;
using CodeFlowLibrary.CodeControl.Operations;
using CodeFlowLibrary.GenioCode;
using CodeFlowUI;
using CodeFlowUI.Manager;
using CodeFlowLibrary.Solution;
using CodeFlowBridge;
using CodeFlow.SolutionAnalyzer;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Settings;
using System.Threading.Tasks;
using CodeFlowLibrary.Package;

namespace CodeFlow
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideService(typeof(CodeFlowPackage), IsAsyncQueryable = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(OptionsPageGrid), "Genio", "CodeFlow properties", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SearchTool), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom)]
    [ProvideToolWindow(typeof(ChangeHistory))]
    public sealed class CodeFlowPackage : AsyncPackage, IVsSolutionEvents, ICodeFlowPackage
    {
        /// <summary>
        /// InvokeCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "23ac2f2d-5778-45dd-b5b2-5186260c958c";

        private DocumentEvents _documentEnvents;
        private Events _dteEvents;
        //private DteInitializer dteInitializer;
        private bool _isSolution;
        public Version OldVersion { get; private set; }
        public Version CurrentVersion { get; private set; }
        public DTE2 DTE { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitCode"/> class.
        /// </summary>
        public CodeFlowPackage()
        {
            PackageBridge.Flow = this;
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, we *may* be on a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            // Otherwise, remove the switch to the UI thread if you don't need it.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await ContextMenuCommand.InitializeAsync(this);
            await ChangeHistoryCommand.InitializeAsync(this);
            await CommitCode.InitializeAsync(this);
            await UpdateCode.InitializeAsync(this);
            await CreateInGenio.InitializeAsync(this);
            await ManageProfiles.InitializeAsync(this);
            await CommitSolution.InitializeAsync(this);
            await FixVS2008Solution.InitializeAsync(this);
            await RefreshSolution.InitializeAsync(this);
            await SearchToolCommand.InitializeAsync(this);
            await ViewVersionsCommand.InitializeAsync(this);
            await GenioProfilesCommand.InitializeAsync(this);
            
            base.Initialize();

            // Try to retrieve the DTE instance at this point
            InitializeDte();
            //IVsShell shellService;
            // If not retrieved, we must wait for the Visual Studio Shell to be initialized
            if (DTE == null)
            {
                // Note: if targetting only VS 2015 and higher, we could use this:
                KnownUIContexts.ShellInitializedContext.WhenActivated(InitializeDte);

                // For VS 2005 and higher, we use this:
                /*shellService = this.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsShell)) as IVsShell;

                dteInitializer = new DteInitializer(shellService, this.InitializeDte);*/
            }
            _dteEvents = DTE.Events;
            _documentEnvents = _dteEvents.DocumentEvents;
            _documentEnvents.DocumentSaved += OnDocumentSave;
            _documentEnvents.DocumentClosing += OnDocumentClose;
            CheckVersion();
            if (PackageBridge.Instance.AllProfiles.Count == 0)
                LoadConfig();
        }

        private void CheckVersion()
        {
            CodeFlowUpdater updater = new CodeFlowUpdater(VersionUpdates.LoadChangeList(this));
            string tmp = Settings.Default.ToolVersion;
            CurrentVersion = new Version(Settings.Default.ToolVersion);
            OldVersion = new Version(Settings.Default.OldVersion);
            Version newVersion = updater.Update(CurrentVersion);
            if(CurrentVersion.CompareTo(newVersion) != 0)
            {
                Settings.Default.OldVersion = String.IsNullOrEmpty(tmp) ? newVersion.ToString() :  CurrentVersion.ToString();
                Settings.Default.ToolVersion = newVersion.ToString();
                Settings.Default.Save();
                OldVersion = CurrentVersion;
                CurrentVersion = newVersion;

                if (!Settings.Default.OldVersion.Equals(Settings.Default.ToolVersion))
                {
                    CodeFlowChangesForm changesForm = new CodeFlowChangesForm(updater, CurrentVersion, OldVersion);
                    CodeFlowUIManager.Open(changesForm);
                }
            }
        }

        private void InitializeDte()
        {
            DTE = GetService(typeof(SDTE)) as DTE2;
        }

        #region CustomEvents

        private void OnDocumentClose(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string path = document.FullName;
            if (PackageBridge.Instance.IsTempFile(path))
                PackageBridge.Instance.RemoveTempFile(path);
        }

        private void OnDocumentSave(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string path = document.FullName;
            Project docProject = document.ProjectItem.ContainingProject;
            List<IManual> man = null;

            if(PackageOptions.AutoExportSaved)
                man = PackageBridge.Instance.GetAutoExportIManual(path);

            // Se for diferente de null quer dizer que é um ficheiro temporário que pode ser exportado automaticamente
            if(man != null)
            {
                try
                {
                    // Check for changes, update and log operation
                    ChangeAnalyzer analyzer = new ChangeAnalyzer();
                    analyzer.CheckForDifferences(man, PackageBridge.Instance.GetActiveProfile());
                    foreach (IChange diff in analyzer.Modifications.AsList)
                    {
                        IOperation operation = diff.GetOperation();
                        if (operation != null)
                            PackageBridge.Instance.ExecuteOperation(operation);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Resources.UnableToExecuteOperation, ex.Message),
                        Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
                return;
            }
            else if (docProject == null)
                return;

            try
            {
                GenioProjectProperties proj = PackageBridge.Instance.SavedFiles.Find(x => { ThreadHelper.ThrowIfNotOnUIThread(); return x.ProjectName == docProject.Name; });
                GenioProjectItem item = new GenioProjectItem(document.Name, document.FullName);
                if (proj == null)
                    PackageBridge.Instance.SavedFiles.Add(new GenioProjectProperties(docProject.Name, new List<GenioProjectItem>() { item }, SolutionParser.GetProjectLanguage(docProject?.CodeModel?.Language)));
                else
                {
                    GenioProjectItem tmp = proj.ProjectFiles.Find(x => x.ItemName == item.ItemName);
                    if (tmp == null)
                        proj.ProjectFiles.Add(item);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        #endregion

        #region SolutionEvents
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            PackageBridge.Instance.SavedFiles.Clear();
            PackageBridge.Instance.ChangeLog.Clear();
            String lastActive = "";

            if (DTE.Solution != null && DTE.Solution.FullName.Length != 0)
            {
                _isSolution = true;
                try
                {
                    string path = Path.GetDirectoryName(DTE.Solution.FullName);
                    lastActive = PackageBridge.Instance.SearchLastActiveProfile(path);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            //Updates combo box
            if (!string.IsNullOrEmpty(lastActive))
                SetProfile(lastActive);
            
            if (PackageOptions.AutoVccto2008Fix && _isSolution)
            {
                ISolutionParser solution = new SolutionParser(this);
#pragma warning disable VSTHRD110 // Observe result of async calls
                solution.ChangeToolset2008Async();
#pragma warning restore VSTHRD110 // Observe result of async calls
            }
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_isSolution)
                PackageBridge.Instance.StoreLastProfile(Path.GetDirectoryName(DTE.Solution.FullName));
            PackageBridge.Instance.GetActiveProfile().GenioConfiguration.CloseConnection();
            PackageBridge.Instance.RemoveTempFiles();
            SaveSettings(PackageBridge.Instance.AllProfiles);
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
        #endregion

        #region OptionsPage
        public OptionsPageGrid OptionsPage
        {
            get
            {
                OptionsPageGrid page = (OptionsPageGrid)GetDialogPage(typeof(OptionsPageGrid));
                return page;
            }
        }
        #endregion

        #region OptionsPageSave

        public void LoadConfig()
        {
            OptionsPage.LoadSettingsFromStorage();

            PackageBridge.Instance.AllProfiles = PackageBridge.Instance.LoadProfiles(Settings.Default.ConnectionStrings);

            if (PackageBridge.Instance.AllProfiles.Count == 1)
                PackageBridge.Instance.SetProfile(PackageBridge.Instance.AllProfiles[0].ProfileName);
        }

        private void SaveConfig()
        {
            Settings.Default.ConnectionStrings = PackageBridge.Instance.SaveProfiles(PackageBridge.Instance.AllProfiles);
            Settings.Default.Save();

            OptionsPage.SaveSettingsToStorage();
        }

        #endregion

        public bool OpenOnPosition(string fileName, int position)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                Window window = DTE.ItemOperations.OpenFile(fileName);
                window.Activate();

                CommandHandler.CommandHandler command = new CommandHandler.CommandHandler();
                command.GetCurrentViewText(out int pos, out Microsoft.VisualStudio.Text.Editor.IWpfTextView textView);
                int linePos = textView.TextSnapshot.GetLineNumberFromPosition(position);

                TextSelection textSelection = window.Document.Selection as TextSelection;
                textSelection.MoveToLineAndOffset(linePos, 1);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void SetProfile(string profileName)
        {
            GenioProfilesCommand.Instance.OnMenuGenioProfilesCombo(this, new OleMenuCmdEventArgs(profileName, IntPtr.Zero));
        }


        public async System.Threading.Tasks.Task<bool> OpenFileAsync(string fileName)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            
            try
            {
                DTE.ItemOperations.OpenFile(fileName);

                return true;
            }
            catch
            { return false; }
        }

        #endregion

        public async System.Threading.Tasks.Task FindInCurrentFileAsync(SearchOptions searchOptions)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            Find find = DTE.Find;
            find.Action = vsFindAction.vsFindActionFind;
            find.MatchWholeWord = searchOptions.WholeWord;
            find.MatchCase = searchOptions.MatchCase;
            find.FindWhat = searchOptions.SearchTerm;
            find.Target = vsFindTarget.vsFindTargetCurrentDocument;
            find.PatternSyntax = vsFindPatternSyntax.vsFindPatternSyntaxLiteral;
            find.Backwards = false;
            find.KeepModifiedDocumentsOpen = true;
            find.Execute();
        }
        
        public WritableSettingsStore GetWritableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(this);
            return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public SettingsStore GetReadableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(this);
            return shellSettingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
        }

        public void SaveSettings(List<Profile> profiles)
        {
            var settings = GetWritableSettingsStore();
            string collection = "CodeFlow";
            bool exists = settings.CollectionExists(collection);
            if (!exists)
                settings.CreateCollection(collection);

            settings.SetString(collection, "Profiles", SerializeProfiles(profiles));
        }

        public List<Profile> LoadSettings()
        {
            var settings = GetReadableSettingsStore();
            string collection = "CodeFlow";
            List<Profile> profiles = new List<Profile>();

            if (settings.CollectionExists(collection))
                profiles = DeSerializeProfiles(settings.GetString(collection, "Profiles"));

            return profiles;
        }

        public string SerializeProfiles(List<Profile> profiles)
        {
            var stringwriter = new StringWriter();
            try
            {
                var serializer = System.Xml.Serialization.XmlSerializer.FromTypes(new[] { profiles.GetType() })[0];
                serializer.Serialize(stringwriter, profiles);
            }
            catch (Exception)
            {
                // ignored
            }
            return stringwriter.ToString();
        }

        public List<Profile> DeSerializeProfiles(string serializedProfiles)
        {
            List<Profile> profiles = null;
            try
            {
                var stringReader = new System.IO.StringReader(serializedProfiles);
                var serializer = System.Xml.Serialization.XmlSerializer.FromTypes(new[] { typeof(List<Profile>) })[0];
                profiles = serializer.Deserialize(stringReader) as List<Profile>;
            }
            catch (Exception)
            {
                // ignored
            }
            return profiles ?? new List<Profile>();
        }
        
        public async Task<string> GetActiveFileFullNameAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            return DTE.ActiveDocument.FullName;
        }
    }
}
