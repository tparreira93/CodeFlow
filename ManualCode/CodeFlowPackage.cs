using System;
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
    [Guid(CodeFlowPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideOptionPage(typeof(OptionsPageGrid), "Genio", "CodeFlow properties", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(CodeFlow.ToolWindow.SearchTool), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Bottom)]
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
            ToolWindow.SearchToolCommand.Initialize(this);

            // Try to retrieve the DTE instance at this point
            InitializeDte();
            //IVsShell shellService;
            // If not retrieved, we must wait for the Visual Studio Shell to be initialized
            if (PackageOperations.DTE == null)
            {
                // Note: if targetting only VS 2015 and higher, we could use this:
                KnownUIContexts.ShellInitializedContext.WhenActivated(() => this.InitializeDte());

                // For VS 2005 and higher, we use this:
                /*shellService = this.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsShell)) as IVsShell;

                dteInitializer = new DteInitializer(shellService, this.InitializeDte);*/
            }
            SetupEvents();

            IVsSolution solution = GetService(typeof(SVsSolution)) as IVsSolution;
            uint cookie = 0;
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
            FixVS2008Solution.Initialize(this);
            RefreshSolution.Initialize(this);


            if (PackageOperations.AllProfiles.Count == 0)
                LoadConfig();
        }


        private void InitializeDte()
        {
            PackageOperations.DTE = this.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
            //dteInitializer = null;
        }

    #endregion

        #region CustomEvents
        private void SetupEvents()
        {
            dteEvents = PackageOperations.DTE.Events;
            documentEnvents = dteEvents.DocumentEvents;
            documentEnvents.DocumentSaved += OnDocumentSave;
        }

        private void OnDocumentSave(Document Document)
        {
            string path = Document.FullName;
            Project docProject = Document.ProjectItem.ContainingProject;
            IManual man = null;

            if(PackageOperations.AutoExportSaved)
                man = PackageOperations.GetAutoExportIManual(path);

            // Se for diferente de null quer dizer que é um ficheiro temporário que pode ser exportado automaticamente
            if(man != null)
            {
                man.Update(PackageOperations.GetActiveProfile());
                return;
            }
            else if (docProject == null)
                return;

            try
            {
                GenioProjectProperties proj = PackageOperations.SavedFiles.Find(x => x.ProjectName == docProject.Name);
                GenioProjectItem item = new GenioProjectItem(Document.ProjectItem, Document.Name, Document.FullName);
                if (proj == null)
                    PackageOperations.SavedFiles.Add(new GenioProjectProperties(docProject, new List<GenioProjectItem>() { item }));
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
            PackageOperations.SavedFiles.Clear();
            String lastActive = "";

            if (PackageOperations.DTE.Solution != null
                && PackageOperations.DTE.Solution.FullName.Length != 0)
            {
                isSolution = true;
                try
                {
                    string path = System.IO.Path.GetDirectoryName(PackageOperations.DTE.Solution.FullName);
                    lastActive = PackageOperations.SearchLastActiveProfile(path);
                }
                catch (Exception)
                { }
            }

            //Updates combo box
            if (lastActive != null && lastActive.Length != 0)
                OnMenuGenioProfilesCombo(this, new OleMenuCmdEventArgs(lastActive, IntPtr.Zero));

            if (PackageOperations.ParseSolution && isSolution)
            {
                PackageOperations.SolutionProps = GenioSolutionProperties.ParseSolution(PackageOperations.DTE);
            }
            if (PackageOperations.AutoVCCTO2008Fix && isSolution)
            {
                GenioSolutionProperties.ChangeToolset2008(PackageOperations.DTE);
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
                PackageOperations.StoreLastProfile(System.IO.Path.GetDirectoryName(PackageOperations.DTE.Solution.FullName));
            PackageOperations.GetActiveProfile().GenioConfiguration.CloseConnection();
            PackageOperations.RemoveTempFiles();
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
                    Marshal.GetNativeVariantForObject(PackageOperations.GetActiveProfile() != null ? PackageOperations.GetActiveProfile().ProfileName : "", vOut);

                else if (newChoice != null)
                {
                    PackageOperations.SetProfile(newChoice);
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
            if (PackageOperations.AllProfiles.Count == 0)
                return;

            string[] dropChoices = new string[PackageOperations.AllProfiles.Count];
            for (int i = 0; i < PackageOperations.AllProfiles.Count; i++)
            {
                dropChoices[i] = PackageOperations.AllProfiles[i].ProfileName;
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

            PackageOperations.AllProfiles = PackageOperations.LoadProfiles(Properties.Settings.Default.ConnectionStrings);

            if (PackageOperations.AllProfiles.Count == 1)
                PackageOperations.SetProfile(PackageOperations.AllProfiles[0].ProfileName);
        }

        public void SaveConfig()
        {
            Settings.Default.ConnectionStrings = PackageOperations.SaveProfiles(PackageOperations.AllProfiles);
            Settings.Default.Save();

            OptionsPage.SaveSettingsToStorage();
        }

        #endregion
    }

    internal class DteInitializer : IVsShellPropertyEvents
    {
        private IVsShell shellService;
        private uint cookie;
        private Action callback;

        internal DteInitializer(IVsShell shellService, Action callback)
        {
            int hr;

            this.shellService = shellService;
            this.callback = callback;

            // Set an event handler to detect when the IDE is fully initialized
            hr = this.shellService.AdviseShellPropertyChanges(this, out this.cookie);

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
        }
        int IVsShellPropertyEvents.OnShellPropertyChange(int propid, object var)
        {
            int hr;
            bool isShellInitialized = false;

            switch (propid)
            {
                // This was for VS 2005, 2008
                //case (int) __VSSPROPID.VSSPROPID_Zombie:

                //   isShellInitialized = !(bool)var;
                //   break;

                // This is for VS 2010 and higher
                case (int)__VSSPROPID4.VSSPROPID_ShellInitialized:

                    isShellInitialized = (bool)var;
                    break;
            }

            if (isShellInitialized)
            {
                // Release the event handler to detect when the IDE is fully initialized
                hr = this.shellService.UnadviseShellPropertyChanges(this.cookie);

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

                this.cookie = 0;

                this.callback();
            }
            return VSConstants.S_OK;
        }
    }
}
