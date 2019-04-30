using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
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
using CodeFlowLibrary.Bridge;
using CodeFlow.SolutionAnalyzer;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Settings;
using System.Threading.Tasks;
using CodeFlowLibrary.Package;
using CodeFlowResources;
using Microsoft.VisualStudio.Text.Editor;
using System.Xml.Serialization;
using CodeFlowLibrary.FileOps;

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
    [ProvideToolWindow(typeof(ChangeHistory), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom)]
    public sealed class CodeFlowPackage : AsyncPackage, IVsSolutionEvents, ICodeFlowPackage
    {
        /// <summary>
        /// InvokeCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "23ac2f2d-5778-45dd-b5b2-5186260c958c";

        private DocumentEvents _documentEnvents;
        private Events _dteEvents;
        //private DteInitializer dteInitializer;
        private string settingsCollection;
        private bool _isSolution;
        private uint _cookie;
        private IVsSolution _solution;
        public DTE2 DTE { get; set; }
        public List<CodeFlowVersion> PackageUpdates { get; set; }
        public InternalSettings Settings { get; set; }
        public FilesManager FileOps { get; set; }
        public Profile Active => Settings?.ActiveProfile ?? new Profile();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitCode"/> class.
        /// </summary>
        public CodeFlowPackage()
        {
            PackageBridge.Flow = this;
            FileOps = new FilesManager(this);
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

            PackageUpdates = VersionUpdates.LoadChangeList(this);

            // When initialized asynchronously, we *may* be on a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            // Otherwise, remove the switch to the UI thread if you don't need it.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            settingsCollection = $"CodeFlow.v{Utils.VSVersion.FullVersion.Major}";
            Settings = LoadUserSettings();
            if (Utils.VSVersion.VS2017)
                ReadOldSettings();

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


            _solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            _solution?.AdviseSolutionEvents(this, out _cookie);

            VersionChecker checker = new VersionChecker(Settings, PackageUpdates);
            if (checker.CheckVersion())
            {
                SaveSettings();
                CodeFlowChangesForm changesForm = new CodeFlowChangesForm(PackageUpdates, Settings.ToolVersion, Settings.OldVersion);
                CodeFlowUIManager.Open(changesForm);
            }
        }

        private void ReadOldSettings()
        {
            var oldSettings = Properties.Settings.Default;
            Version oldSettingsVersion = new Version(oldSettings.ToolVersion);
            Version newSettingsVersion = new Version("4.0.0");
            if (oldSettingsVersion != new Version("0.0.0") && oldSettingsVersion.IsBefore(newSettingsVersion))
            {
                if (Settings.Profiles.Count == 0)
                {
                    Settings.Profiles = PackageBridge.Instance.DeSerializeProfiles(oldSettings.ConnectionStrings);
                    Settings.OldVersion = new Version(oldSettings.OldVersion);
                    Settings.ToolVersion = oldSettingsVersion;
                }
                
                oldSettings.ToolVersion = newSettingsVersion.ToString();
                oldSettings.OldVersion = newSettingsVersion.ToString();

                oldSettings.Save();
            }
        }

        protected override void Dispose(bool disposing)
        {
            Dispatcher.CurrentDispatcher.VerifyAccess();
            if (_cookie != 0 && _solution != null)
            {
                _solution.UnadviseSolutionEvents(_cookie);
                _cookie = 0;
            }
            base.Dispose(disposing);
        }

        private void InitializeDte()
        {
            DTE = GetService(typeof(SDTE)) as DTE2;
            Assumes.Present(DTE);
        }

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

        #region CustomEvents

        private void OnDocumentClose(Document document)
        {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            string path = Utils.AsyncHelper.RunSyncUI(() => document.FullName);
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

            FileOps.Remove(path);
        }

        private void OnDocumentSave(Document document)
        {
            Utils.AsyncHelper.RunSyncUI(() =>
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                string path = document.FullName;
                string name = document.Name;
                Project docProject = document.ProjectItem.ContainingProject;
                string projName = docProject?.Name ?? "";
                ProjectLanguage lang = SolutionParser.GetProjectLanguage(docProject?.CodeModel?.Language);
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                List <IManual> man = null;

                if (PackageOptions.AutoExportSaved)
                    man = FileOps.GetAutoExportIManual(path);

                // Se for diferente de null quer dizer que é um ficheiro temporário que pode ser exportado automaticamente
                if (man != null)
                {
                    try
                    {
                        // Check for changes, update and log operation
                        ChangeAnalyzer analyzer = new ChangeAnalyzer();
                        analyzer.CheckForDifferences(man, Active);
                        foreach (IChange diff in analyzer.Modifications.AsList)
                        {
                            IOperation operation = diff.GetOperation();
                            if (operation != null)
                                ExecuteOperation(operation);
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
                    GenioProjectProperties proj = CodeFlowLibrary.Bridge.PackageBridge.Instance.SavedFiles.Find(x => { ThreadHelper.ThrowIfNotOnUIThread(); return x.ProjectName == projName; });
                    GenioProjectItem item = new GenioProjectItem(name, path);
                    if (proj == null)
                        PackageBridge.Instance.SavedFiles.Add(new GenioProjectProperties(projName, new List<GenioProjectItem>() { item }, lang));
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
            });
        }
        #endregion

        #region SolutionEvents
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            Utils.AsyncHelper.RunSyncUI(() =>
           {
               Dispatcher.CurrentDispatcher.VerifyAccess();
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
           });
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
            Utils.AsyncHelper.RunSyncUI(() =>
            {
                Dispatcher.CurrentDispatcher.VerifyAccess();
                Profile active = Active;

                if (_isSolution && active.IsValid())
                    PackageBridge.Instance.StoreLastProfile(Path.GetDirectoryName(DTE.Solution.FullName), active);
                active.GenioConfiguration.CloseConnection();
                FileOps.RemoveAll();
                SaveSettings();
            });
            return VSConstants.S_OK;
        }


        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
        #endregion


        public void SetProfile(string profileName)
        {
            GenioProfilesCommand.Instance.OnMenuGenioProfilesCombo(this, new OleMenuCmdEventArgs(profileName, IntPtr.Zero));
        }

        public void LoadProfile(string profileName)
        {
            Profile p = Settings.Profiles.Find(x => x.ProfileName.Equals(profileName));
            if (p != null)
            {
                Settings.ActiveProfile = p;
                Settings.ActiveProfile.GenioConfiguration.GetGenioInfo();
                Settings.ActiveProfile.GenioConfiguration.ParseGenioFiles();
            }
        }



        #endregion

        #region InternalSettings
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
        #endregion

        #region ICodeFlowPackage

        public void SaveSettings()
        {
            var settings = GetWritableSettingsStore();
            bool exists = settings.CollectionExists(settingsCollection);
            if (!exists)
                settings.CreateCollection(settingsCollection);

            settings.SetString(settingsCollection, "Profiles", PackageBridge.Instance.SerializeProfiles(Settings.Profiles));
            settings.SetString(settingsCollection, "ToolVersion", Settings.ToolVersion.ToString());
            settings.SetString(settingsCollection, "OldVersion", Settings.OldVersion.ToString());
        }

        public InternalSettings LoadUserSettings()
        {
            var settings = GetReadableSettingsStore();
            List<Profile> profiles = new List<Profile>();
            InternalSettings user = new InternalSettings();

            if (settings.CollectionExists(settingsCollection))
            {
                user.Profiles = PackageBridge.Instance.DeSerializeProfiles(settings.GetString(settingsCollection, "Profiles"));
                user.ToolVersion = new Version(settings.GetString(settingsCollection, "ToolVersion"));
                user.OldVersion = new Version(settings.GetString(settingsCollection, "OldVersion"));
            }

            return user;
        }

        public async Task<string> GetActiveFileFullNameAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            return DTE.ActiveDocument.FullName;
        }

        public async System.Threading.Tasks.Task FindCodeAsync(SearchOptions searchOptions)
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

        public async Task<bool> OpenFileAsync(string fileName)
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

        public async Task<bool> OpenOnPositionAsync(string fileName, int position)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                Window window = DTE.ItemOperations.OpenFile(fileName);
                window.Activate();

                Handlers.CommandHandler command = new Handlers.CommandHandler(this);
                var (code, cursorPos, textView, fullDocumentName) = await command.GetCurrentViewTextAsync();
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

        #endregion

        #region Operations
        public bool ExecuteOperation(IOperation operation)
        {
            bool result = operation.Execute();
            if (result && PackageOptions.LogOperations)
                PackageBridge.Instance.ChangeLog.LogOperation(operation);

            return result;
        }
        #endregion
    }
}
