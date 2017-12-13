﻿using System;
using System.ComponentModel.Design;
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
using CodeFlow.CodeControl;
using CodeFlow.CodeControl.Analyzer;
using CodeFlow.GenioManual;
using System.Linq;
using CodeFlow.Forms;

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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideOptionPage(typeof(OptionsPageGrid), "Genio", "CodeFlow properties", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindow.SearchTool), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom)]
    [ProvideToolWindow(typeof(ToolWindow.ChangeHistory))]
    public sealed class CodeFlowPackage : Package, IVsSolutionEvents
    {
        /// <summary>
        /// InvokeCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "23ac2f2d-5778-45dd-b5b2-5186260c958c";

        private DocumentEvents documentEnvents;
        private Events dteEvents;
        //private DteInitializer dteInitializer;
        private bool isSolution = false;
        private uint cookie = 0;
        private IVsSolution solution;
        private CodeFlowVersions versions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitCode"/> class.
        /// </summary>
        public CodeFlowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        protected override void Dispose(bool disposing)
        {
            if (cookie != 0)
            {
                solution.UnadviseSolutionEvents(cookie);
                cookie = 0;
            }
            base.Dispose(disposing);
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            CommitCode.Initialize(this);
            UpdateCode.Initialize(this);
            CreateInGenio.Initialize(this);
            ManageProfiles.Initialize(this);
            CommitSolution.Initialize(this);
            ContextMenu.Initialize(this);
            FixVS2008Solution.Initialize(this);
            RefreshSolution.Initialize(this);
            ChangeHistoryCommand.Initialize(this);
            SearchToolCommand.Initialize(this);

            // Try to retrieve the DTE instance at this point
            InitializeDTE();
            //IVsShell shellService;
            // If not retrieved, we must wait for the Visual Studio Shell to be initialized
            if (PackageOperations.Instance.DTE == null)
            {
                // Note: if targetting only VS 2015 and higher, we could use this:
                KnownUIContexts.ShellInitializedContext.WhenActivated(() => this.InitializeDTE());

                // For VS 2005 and higher, we use this:
                /*shellService = this.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsShell)) as IVsShell;

                dteInitializer = new DteInitializer(shellService, this.InitializeDte);*/
            }
            SetupEvents();

            solution = GetService(typeof(SVsSolution)) as IVsSolution;
            solution.AdviseSolutionEvents(this, out cookie);

            if (GetService(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
            {
                // --- Initialize the DropDownCombo
                CommandID menuGenioProfilesComboCommandID =
                  new CommandID(PackageGuidList.guidComboBoxCmdSet,
                  (int)PackageCommandList.cmdGenioProfilesCombo);
                OleMenuCommand menuGenioProfilesComboCommand =
                  new OleMenuCommand(new EventHandler(OnMenuGenioProfilesCombo),
                  menuGenioProfilesComboCommandID);
                menuGenioProfilesComboCommand.ParametersDescription = "$";
                mcs.AddCommand(menuGenioProfilesComboCommand);


                CommandID menuGenioProfilesComboGetListCommandID = new CommandID(PackageGuidList.guidComboBoxCmdSet, (int)PackageCommandList.cmdGenioProfilesComboGetList);
                MenuCommand menuGenioProfilesComboGetListCommand = new OleMenuCommand(new EventHandler(OnMenuGenioProfilesComboGetList), menuGenioProfilesComboGetListCommandID);
                mcs.AddCommand(menuGenioProfilesComboGetListCommand);
            }

            versions = new CodeFlowVersions();
            UpdateVersion();


            if (PackageOperations.Instance.AllProfiles.Count == 0)
                LoadConfig();
        }

        private void UpdateVersion()
        {
            string currentVersion = Settings.Default.ToolVersion;
            Version newVersion = versions.Execute(currentVersion, OptionsPage);
            Settings.Default.ToolVersion = newVersion.ToString();
            Settings.Default.Save();
            if(!currentVersion.Equals(newVersion.ToString()))
            {
                CodeFlowChangesForm changesForm = new CodeFlowChangesForm(versions);
                changesForm.Show();
            }
        }

        private void InitializeDTE()
        {
            PackageOperations.Instance.DTE = this.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
        }

    #endregion

        #region CustomEvents
        private void SetupEvents()
        {
            dteEvents = PackageOperations.Instance.DTE.Events;
            documentEnvents = dteEvents.DocumentEvents;
            documentEnvents.DocumentSaved += OnDocumentSave;
        }

        private void OnDocumentSave(Document Document)
        {
            string path = Document.FullName;
            Project docProject = Document.ProjectItem.ContainingProject;
            List<IManual> man = null;

            if(PackageOperations.Instance.AutoExportSaved)
                man = PackageOperations.Instance.GetAutoExportIManual(path);

            // Se for diferente de null quer dizer que é um ficheiro temporário que pode ser exportado automaticamente
            if(man != null)
            {
                // Check for changes, update and log operation
                ChangeAnalyzer analyzer = new ChangeAnalyzer();
                analyzer.CheckForDifferences(man, PackageOperations.Instance.GetActiveProfile());
                foreach (IChange diff in analyzer.Modifications.AsList)
                {
                    IOperation operation = diff.GetOperation();
                    if(operation != null)
                        PackageOperations.Instance.ExecuteOperation(operation);
                }
                return;
            }
            else if (docProject == null)
                return;

            try
            {
                GenioProjectProperties proj = PackageOperations.Instance.SavedFiles.Find(x => x.ProjectName == docProject.Name);
                GenioProjectItem item = new GenioProjectItem(Document.ProjectItem, Document.Name, Document.FullName);
                if (proj == null)
                    PackageOperations.Instance.SavedFiles.Add(new GenioProjectProperties(docProject, new List<GenioProjectItem>() { item }));
                else
                {
                    GenioProjectItem tmp = proj.ProjectFiles.Find(x => x.ItemName == item.ItemName);
                    if (tmp == null)
                        proj.ProjectFiles.Add(item);
                }
            }
            catch (Exception)
            { }
        }
        #endregion

        #region SolutionEvents
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            PackageOperations.Instance.SavedFiles.Clear();
            PackageOperations.Instance.ChangeLog.Clear();
            String lastActive = "";

            if (PackageOperations.Instance.DTE.Solution != null
                && PackageOperations.Instance.DTE.Solution.FullName.Length != 0)
            {
                isSolution = true;
                try
                {
                    string path = System.IO.Path.GetDirectoryName(PackageOperations.Instance.DTE.Solution.FullName);
                    lastActive = PackageOperations.Instance.SearchLastActiveProfile(path);
                }
                catch (Exception)
                { }
            }

            //Updates combo box
            if (!string.IsNullOrEmpty(lastActive))
                OnMenuGenioProfilesCombo(this, new OleMenuCmdEventArgs(lastActive, IntPtr.Zero));

            if (PackageOperations.Instance.ParseSolution && isSolution)
            {
                PackageOperations.Instance.SolutionProps = GenioSolutionProperties.ParseSolution(PackageOperations.Instance.DTE);
            }
            if (PackageOperations.Instance.AutoVccto2008Fix && isSolution)
            {
                GenioSolutionProperties.ChangeToolset2008(PackageOperations.Instance.DTE);
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
            if(isSolution)
                PackageOperations.Instance.StoreLastProfile(System.IO.Path.GetDirectoryName(PackageOperations.Instance.DTE.Solution.FullName));
            PackageOperations.Instance.GetActiveProfile().GenioConfiguration.CloseConnection();
            PackageOperations.Instance.RemoveTempFiles();
            SaveConfig();
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }
        #endregion

        #region ComboBox options
        private void OnMenuGenioProfilesCombo(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
            {
                throw (new ArgumentException("No event args"));
            }
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                string newChoice = eventArgs.InValue as string;
                IntPtr vOut = eventArgs.OutValue;
                if (vOut != IntPtr.Zero && newChoice != null)
                    throw (new ArgumentException("Ilegal input and output parameters!"));

                else if (vOut != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(PackageOperations.Instance.GetActiveProfile() != null ? PackageOperations.Instance.GetActiveProfile().ProfileName : "", vOut);

                else if (newChoice != null)
                {
                    PackageOperations.Instance.SetProfile(newChoice);
                }
                else
                    throw (new ArgumentException("Invalid input and output!"));
            }
            else
                throw (new ArgumentException("Invalid combo box call!"));
        }
        private void OnMenuGenioProfilesComboGetList(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (PackageOperations.Instance.AllProfiles.Count == 0)
                return;

            string[] dropChoices = new string[PackageOperations.Instance.AllProfiles.Count];
            for (int i = 0; i < PackageOperations.Instance.AllProfiles.Count; i++)
            {
                dropChoices[i] = PackageOperations.Instance.AllProfiles[i].ProfileName;
            }

            if (eventArgs != null)
            {
                object inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (inParam != null)
                    throw (new ArgumentException("Ilegal input parameter!"));

                else if (vOut != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(dropChoices, vOut);

                else
                    throw (new ArgumentException("Output parameter required!"));
            }

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

            PackageOperations.Instance.AllProfiles = PackageOperations.Instance.LoadProfiles(Settings.Default.ConnectionStrings);

            if (PackageOperations.Instance.AllProfiles.Count == 1)
                PackageOperations.Instance.SetProfile(PackageOperations.Instance.AllProfiles[0].ProfileName);
        }

        public void UpdateOnNewSetting()
        {
            OptionsPage.LoadSettingsFromStorage();

            string currentVersion = Settings.Default.ToolVersion;

            OptionsPage.ExtensionsFilters = "*";
            OptionsPage.SaveSettingsToStorage();
        }

        public void SaveConfig()
        {
            Settings.Default.ConnectionStrings = PackageOperations.Instance.SaveProfiles(PackageOperations.Instance.AllProfiles);
            Settings.Default.Save();

            OptionsPage.SaveSettingsToStorage();
        }

        #endregion
    }
}
