﻿using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE;
using CodeFlow.Properties;
using CodeFlow.SolutionOperations;
using System.Collections.Generic;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

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
    [ProvideToolWindow(typeof(FindInManualCode))]
    [ProvideOptionPage(typeof(OptionsPageGrid), "Genio", "CodeFlow properties", 0, 0, true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class CodeFlowPackage : Package, IVsSolutionEvents
    {
        /// <summary>
        /// InvokeCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "23ac2f2d-5778-45dd-b5b2-5186260c958c";

        private DocumentEvents documentEnvents;
        private Events dteEvents;
        private DteInitializer dteInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportToGenio"/> class.
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
            ExportToGenio.Initialize(this);
            base.Initialize();
            ImportFromGenio.Initialize(this);
            CreateInGenio.Initialize(this);
            IVsSolution solution = GetService(typeof(SVsSolution)) as IVsSolution;
            uint cookie = 0;
            solution.AdviseSolutionEvents(this, out cookie);
            FindInManualCodeCommand.Initialize(this);
            ManageProfiles.Initialize(this);
            ExportSolution.Initialize(this);
            InitializeDTE();

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


        private void InitializeDTE()
        {
            IVsShell shellService;

            PackageOperations.DTE = this.GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE)) as EnvDTE80.DTE2;

            if (PackageOperations.DTE == null) // The IDE is not yet fully initialized
            {
                shellService = this.GetService(typeof(SVsShell)) as IVsShell;
                this.dteInitializer = new DteInitializer(shellService, this.InitializeDTE);
            }
            else
            {
                this.dteInitializer = null;
                SetupEvents();
            }
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
            if (docProject == null)
                return;

            List<GenioProjectProperties> savedFiles = GenioSolutionProperties.SavedFiles;

            GenioProjectProperties proj = savedFiles.Find(x => x.ProjectName == docProject.Name);
            GenioProjectItem item = new GenioProjectItem(Document.ProjectItem);
            if (proj == null)
                proj = new GenioProjectProperties(docProject, new List<GenioProjectItem>() { item });
            else
            {
                GenioProjectItem tmp = proj.ProjectFiles.Find(x => x.ItemName == item.ItemName);
                if (tmp == null)
                    proj.ProjectFiles.Add(item);
            }
        }
        #endregion

        #region SolutionEvents
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            if (PackageOperations.ParseSolution)
            {
                PackageOperations.SolutionProps = GenioSolutionProperties.ParseSolution(PackageOperations.DTE);
            }
            if(PackageOperations.AutoVCCTO2008Fix)
            {
                GenioSolutionProperties.ChangeToolset2008(PackageOperations.DTE);
            }
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
            PackageOperations.ActiveProfile.GenioConfiguration.CloseConnection();
            PackageOperations.RemoveTempFiles();
            SaveConfig();
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        /*private async System.Threading.Tasks.Task doInitializeAsync()
        {
            await VsTaskLibraryHelper.CreateAndStartTask(
            VsTaskLibraryHelper.ServiceInstance,
            VsTaskRunContext.UIThreadIdlePriority,
                    () =>
                    {
                        if (PackageOperations.AllProfiles.Count == 0)
                            LoadConfig();
                        if(PackageOperations.ParseSolution)
                            PackageOperations.SolutionProps = GenioSolutionProperties.ParseSolution(PackageOperations.DTE);
                    }
                );
        }*/
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
                    Marshal.GetNativeVariantForObject(PackageOperations.ActiveProfile != null ? PackageOperations.ActiveProfile.ProfileName : "", vOut);

                else if (newChoice != null)
                    PackageOperations.SetProfile(newChoice);
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
            string lastActive = Settings.Default.LastActiveProfile;

            Profile p = PackageOperations.AllProfiles.Find(x => x.ProfileName.Equals(lastActive));

            if (p == null && PackageOperations.AllProfiles.Count == 1)
                PackageOperations.SetProfile(PackageOperations.AllProfiles[0].ProfileName);
            else if(p != null)
                PackageOperations.SetProfile(p.ProfileName);
        }

        public void SaveConfig()
        {
            Settings.Default.ConnectionStrings = PackageOperations.SaveProfiles(PackageOperations.AllProfiles);
            Settings.Default.LastActiveProfile = PackageOperations.ActiveProfile.ProfileName;
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
            bool isZombie;

            if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
            {
                isZombie = (bool)var;

                if (!isZombie)
                {
                    // Release the event handler to detect when the IDE is fully initialized
                    hr = this.shellService.UnadviseShellPropertyChanges(this.cookie);

                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);

                    this.cookie = 0;

                    this.callback();
                }
            }
            return VSConstants.S_OK;
        }
    }
}
