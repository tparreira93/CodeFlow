namespace CodeFlow.ToolWindow
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.ComponentModel.Design;
    using System.Collections.Generic;
    using System.Windows.Threading;
    using System.Windows;
    using System.Threading;
    using CodeFlowUI.Controls;
    using CodeFlowLibrary.GenioCode;
    using CodeFlowLibrary.Genio;
    using CodeFlowLibrary.Bridge;
    using CodeFlowLibrary.Settings;
    using System.Linq;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using System.Windows.Forms;

    //using CommandHandler;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("92310e84-1d2c-4801-b3e5-63ba1f5f2d5c")]
    public class SearchTool : ToolWindowPane, IOleCommandTarget//, IVsWindowFrameNotify3
    {
        private const string toolWindowSet = "4f609967-bec4-4036-9038-1a779d23cc7e";
        private const int cmdidSearchToolbar = 0x101;
        private const int cmdidSearchToolbarGroup = 0x1001;
        private const int cmdidSearchToolbarGroup2 = 0x1002;
        private const int cmdidSearchManualCode = 0x2003;
        private const int cmdIdSearchBox = 0x105;
        private const int cmdIdWholeWord = 0x2004;
        private const int cmdIdCaseSensitive = 0x2005;
        private const int cmdIdPlataformCombo = 0x2006;
        private const int cmdIdPlataformComboGetList = 0x2007;
        private const int cmdExecuteSearch = 0x2008;

        private SearchToolControl control = null;
        public static bool WindowInitialized = false;
        private string plataform = "";
        private string currentSearch = "";
        private bool caseSensitive = false;
        private bool wholeWord = false;
        private readonly object searchLock = new object();

        // Hook preview Window
        private string previewFile;
        IComponentModel _componentModel;
        IVsInvisibleEditorManager _invisibleEditorManager;
        //This adapter allows us to convert between Visual Studio 2010 editor components and
        //the legacy components from Visual Studio 2008 and earlier.
        IVsEditorAdaptersFactoryService _editorAdapter;
        ITextEditorFactoryService _editorFactoryService;
        IVsTextView _currentlyFocusedTextView;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTool"/> class.
        /// </summary>
        public SearchTool() : base(null)
        {
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            ToolBar = new CommandID(new Guid(toolWindowSet), cmdidSearchToolbar);
            // Specify that we want the toolbar at the top of the window
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
            previewFile = PackageBridge.Flow.SearchPreviewFile;

            _componentModel = (IComponentModel)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel));
            _invisibleEditorManager = (IVsInvisibleEditorManager)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsInvisibleEditorManager));
            _editorAdapter = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
            _editorFactoryService = _componentModel.GetService<ITextEditorFactoryService>();
            PackageBridge.Flow.OpenFileAsync(previewFile);
        }


        #region SidePreview


        /// <summary>
        /// Creates an invisible editor for a given filePath. 
        /// If you're frequently creating projection buffers, it may be worth caching
        /// these editors as they're somewhat expensive to create.
        /// </summary>
        private IVsInvisibleEditor GetInvisibleEditor(string filePath)
        {
            IVsInvisibleEditor invisibleEditor;
            ErrorHandler.ThrowOnFailure(this._invisibleEditorManager.RegisterInvisibleEditor(
                filePath
                , pProject: null
                , dwFlags: (uint)_EDITORREGFLAGS.RIEF_ENABLECACHING
                , pFactory: null
                , ppEditor: out invisibleEditor));
            RegisterDocument(filePath);
            return invisibleEditor;
        }

        uint RegisterDocument(string targetFile)
        {
            //Then when creating the IVsInvisibleEditor, find and lock the document 
            uint itemID;
            IntPtr docData;
            uint docCookie;
            IVsHierarchy hierarchy;
            var runningDocTable = (IVsRunningDocumentTable)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsRunningDocumentTable)); 
            

            ErrorHandler.ThrowOnFailure(runningDocTable.FindAndLockDocument(
                dwRDTLockType: (uint)_VSRDTFLAGS.RDT_EditLock,
                pszMkDocument: targetFile,
                ppHier: out hierarchy,
                pitemid: out itemID,
                ppunkDocData: out docData,
                pdwCookie: out docCookie));

            return docCookie;
        }

        public IWpfTextViewHost CreateEditor(string filePath, int start = 0, int end = 0, bool createProjectedEditor = false)
        {
            //IVsInvisibleEditors are in-memory represenations of typical Visual Studio editors.
            //Language services, highlighting and error squiggles are hooked up to these editors
            //for us once we convert them to WpfTextViews. 
            var invisibleEditor = GetInvisibleEditor(filePath);

            var docDataPointer = IntPtr.Zero;
            Guid guidIVsTextLines = typeof(IVsTextLines).GUID;

            ErrorHandler.ThrowOnFailure(invisibleEditor.GetDocData(
                fEnsureWritable: 1
                , riid: ref guidIVsTextLines
                , ppDocData: out docDataPointer));

            IVsTextLines docData = (IVsTextLines)Marshal.GetObjectForIUnknown(docDataPointer);

            // This will actually be defined as _codewindowbehaviorflags2.CWB_DISABLEDIFF once the latest version of
            // Microsoft.VisualStudio.TextManager.Interop.16.0.DesignTime is published. Setting the flag will have no effect
            // on releases prior to d16.0.
            const _codewindowbehaviorflags CWB_DISABLEDIFF = (_codewindowbehaviorflags)0x04;

            //var serviceProvider = (PackageBridge.Flow as CodeFlowPackage).OleServiceProvider;
            var serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider));
            //Create a code window adapter
            var codeWindow = _editorAdapter.CreateVsCodeWindowAdapter(serviceProvider);

            // You need to disable the dropdown, splitter and -- for d16.0 -- diff since you are extracting the code window's TextViewHost and using it.
            ((IVsCodeWindowEx)codeWindow).Initialize((uint)_codewindowbehaviorflags.CWB_DISABLESPLITTER | (uint)_codewindowbehaviorflags.CWB_DISABLEDROPDOWNBAR | (uint)CWB_DISABLEDIFF,
                                                     VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter,
                                                     string.Empty,
                                                     string.Empty,
                                                     0,
                                                     new INITVIEW[1]);

            ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(docData));

            //Get a text view for our editor which we will then use to get the WPF control for that editor.
            IVsTextView textView;
            ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));

            if (createProjectedEditor)
            {
                //We add our own role to this text view. Later this will allow us to selectively modify
                //this editor without getting in the way of Visual Studio's normal editors.
                var roles = _editorFactoryService.DefaultRoles.Concat(new string[] { "CustomProjectionRole" });

                var vsTextBuffer = docData as IVsTextBuffer;
                var textBuffer = _editorAdapter.GetDataBuffer(vsTextBuffer);

                textBuffer.Properties.AddProperty("StartPosition", start);
                textBuffer.Properties.AddProperty("EndPosition", end);
                var guid = VSConstants.VsTextBufferUserDataGuid.VsTextViewRoles_guid;
                ((IVsUserData)codeWindow).SetData(ref guid, _editorFactoryService.CreateTextViewRoleSet(roles).ToString());
            }

            _currentlyFocusedTextView = textView;
            var textViewHost = _editorAdapter.GetWpfTextViewHost(textView);
            return textViewHost;
        }
        
        protected override bool PreProcessMessage(ref Message m)
        {
            if (_currentlyFocusedTextView != null)
            {
                // copy the Message into a MSG[] array, so we can pass
                // it along to the active core editor's IVsWindowPane.TranslateAccelerator
                var pMsg = new MSG[1];
                pMsg[0].hwnd = m.HWnd;
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;

                var vsWindowPane = (IVsWindowPane)_currentlyFocusedTextView;
                return vsWindowPane.TranslateAccelerator(pMsg) == 0;
            }
            return base.PreProcessMessage(ref m);
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_currentlyFocusedTextView != null)
            {
                var cmdTarget = (IOleCommandTarget)_currentlyFocusedTextView;
                hr = cmdTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            }
            return hr;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            var hr =
              (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;

            if (_currentlyFocusedTextView != null)
            {
                var cmdTarget = (IOleCommandTarget)_currentlyFocusedTextView;
                hr = cmdTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            }
            return hr;
        }


        private IWpfTextViewHost _completeTextViewHost;
        public IWpfTextViewHost CompleteTextViewHost
        {
            get
            {
                if (_completeTextViewHost == null)
                {
                    _completeTextViewHost = CreateEditor(previewFile);
                }
                return _completeTextViewHost;
            }
        }

        public override object Content
        {
            get
            {
                if (control == null)
                {
                    control = new SearchToolControl();
                    control.HookPreview(CompleteTextViewHost?.HostControl); // Don't rely on the IWpfTextViewHost implementation being a FrameworkElement
                }
                return control;
            }
        }

        #endregion


        /// <summary>
        /// This is called after our control has been created and sited.
        /// This is a good place to initialize the control with data gathered
        /// from Visual Studio services.
        /// </summary>
        public override void OnToolWindowCreated()
        {
            //We need to set up the tool window to respond to key bindings
            //They're passed to the tool window and its buffers via Query() and Exec()
            var windowFrame = (IVsWindowFrame)Frame;
            var cmdUi = VSConstants.GUID_TextEditorFactory;
            windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);

            base.OnToolWindowCreated();

            // Set the text that will appear in the title bar of the tool window.
            // Note that because we need access to the package for localization,
            // we have to wait to do this here. If we used a constant string,
            // we could do this in the consturctor.
            this.Caption = CodeFlowResources.Resources.CodeSearch;

            if (GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(new Guid(toolWindowSet), cmdidSearchManualCode);
                var command = new MenuCommand(this.SearchManualCode, menuCommandID);
                commandService.AddCommand(command);

                menuCommandID = new CommandID(new Guid(toolWindowSet), cmdIdSearchBox);
                var searchBoxCommand = new OleMenuCommand(new EventHandler(SearchTerm), menuCommandID)
                {
                    ParametersDescription = "$" // accept any argument string
                };
                searchBoxCommand.CommandChanged += SearchTerm;
                commandService.AddCommand(searchBoxCommand);

                menuCommandID = new CommandID(new Guid(toolWindowSet), cmdExecuteSearch);
                command = new MenuCommand(this.SearchManualCode, menuCommandID);
                commandService.AddCommand(command);

                menuCommandID = new CommandID(new Guid(toolWindowSet), cmdIdCaseSensitive);
                command = new MenuCommand(this.CheckCaseSensitive, menuCommandID);
                commandService.AddCommand(command);

                menuCommandID = new CommandID(new Guid(toolWindowSet), cmdIdWholeWord);
                command = new MenuCommand(this.CheckWholeWord, menuCommandID);
                commandService.AddCommand(command);

                menuCommandID = new CommandID(new Guid(toolWindowSet), cmdIdPlataformCombo);
                OleMenuCommand menuMyDropDownComboCommand = new OleMenuCommand(new EventHandler(SetPlataform), menuCommandID);
                commandService.AddCommand(menuMyDropDownComboCommand);

                menuCommandID = new CommandID(new Guid(toolWindowSet), cmdIdPlataformComboGetList);
                MenuCommand menuMyDropDownComboGetListCommand = new OleMenuCommand(new EventHandler(GetPlataformList), menuCommandID);
                commandService.AddCommand(menuMyDropDownComboGetListCommand);
            }

            // Ensure the control's handle has been created; otherwise, BeginInvoke cannot be called.
            // Note that during runtime this should have no effect when running inside Visual Studio,
            // as the control's handle should already be created, but unit tests can end up calling
            // this method without the control being created.
            control.InitializeComponent();

            SetPlataform(this, new OleMenuCmdEventArgs("All", new IntPtr()));
        }

        public void UpdateSearchPreview(IManual code, SearchOptions options)
        {
            Profile p = PackageBridge.Flow.Active;
            string ext = code.GetCodeExtension(p);
            var extensionList = p.GenioConfiguration.Plataforms.SelectMany(x => x.TipoRotina.Select(t => t.ProgrammingLanguage)).Distinct();
            int size = CompleteTextViewHost.TextView.TextSnapshot.Length;
            var edit = CompleteTextViewHost.TextView.TextBuffer.CreateEdit();
            edit.Replace(0, size, code.ToString());
        }

        private void SearchTerm(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
                return;


            if (e is OleMenuCmdEventArgs eventArgs)
            {
                object input = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero && input != null)
                    throw (new ArgumentException("Ilegal input and output parameters!"));

                else if (vOut != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(currentSearch, vOut);

                else if (input != null)
                {
                    string newChoice = input.ToString();

                    if (!string.IsNullOrEmpty(newChoice))
                    {
                        currentSearch = newChoice;
                        SearchManualCode(sender, e);
                    }
                }
                else
                    throw (new ArgumentException("Invalid input and output!"));
            }
            else
                throw (new ArgumentException("Invalid combo box call!"));
        }

        internal void Search(string searchTerms)
        {
            OleMenuCommand cmd = null;
            if (GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                cmd = commandService?.FindCommand(new CommandID(new Guid(toolWindowSet), cmdIdSearchBox)) as OleMenuCommand;
                cmd?.Invoke(searchTerms, new IntPtr(), Microsoft.VisualStudio.OLE.Interop.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER);
            }
        }

        private void SetPlataform(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
            {
                plataform = String.Empty;
                return;
            }
            if (e is OleMenuCmdEventArgs eventArgs)
            {
                string newChoice = eventArgs.InValue as string;
                IntPtr vOut = eventArgs.OutValue;
                if (vOut != IntPtr.Zero && newChoice != null)
                    throw (new ArgumentException("Ilegal input and output parameters!"));

                else if (vOut != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(plataform, vOut);

                else if (newChoice != null)
                    plataform = newChoice;
                else
                    throw (new ArgumentException("Invalid input and output!"));
            }
            else
                throw (new ArgumentException("Invalid combo box call!"));
        }
        private void GetPlataformList(object sender, EventArgs e)
        {
            Profile p = PackageBridge.Flow.Active;
            string[] dropChoices = new string[p.GenioConfiguration.Plataforms.Count + 1];
            dropChoices[0] = "All";

            for (int i = 1; i < p.GenioConfiguration.Plataforms.Count; i++)
            {
                dropChoices[i] = p.GenioConfiguration.Plataforms[i].ID;
            }

            if (e is OleMenuCmdEventArgs eventArgs)
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
        private void SearchManualCode(object sender, EventArgs e)
        {
            // Only one search at time
            Profile p = PackageBridge.Flow.Active;
            if (p.IsValid() && Monitor.TryEnter(searchLock, 2000))
            {
                control.Clear();
                OleMenuCommand cmd = null;
                if (GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                {
                    cmd = commandService?.FindCommand(new CommandID(new Guid(toolWindowSet), cmdIdSearchBox)) as OleMenuCommand;
                    //var cmdSearch = commandService.FindCommand(new CommandID(new Guid(toolWindowSet), cmdidSearchManualCode));
                }
                if (cmd == null /*|| cmdSearch == null*/)
                    return;

                try
                {
                    if (String.IsNullOrEmpty(currentSearch))
                    {
                        currentSearch = cmd.Text;
                        if (String.IsNullOrEmpty(currentSearch))
                            return;
                    }
                    string searchPlat = plataform;
                    if (plataform.Equals("All"))
                        searchPlat = "";
                    cmd.Enabled = false;

#pragma warning disable VSTHRD110 // Observe result of async calls
#pragma warning disable VSTHRD105 // Avoid method overloads that assume TaskScheduler.Current
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
#pragma warning restore VSTHRD105 // Avoid method overloads that assume TaskScheduler.Current
#pragma warning restore VSTHRD110 // Observe result of async calls
                    {
                        string error = "";
                        List<IManual> res = new List<IManual>();
                        try
                        {
                            res.AddRange(Manual.SearchDatabase(p, currentSearch, caseSensitive, wholeWord, searchPlat));
                        }
                        catch (Exception ex)
                        {
                            error = ex.Message;
                        }

                        // Update UI 
                        Utils.AsyncHelper.RunSyncUI(() =>
                        {
                            control.RefreshteList(res, new SearchOptions(p, currentSearch, wholeWord, caseSensitive));
                            cmd.Enabled = true;

                            if (error.Length != 0)
                            {
                                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, error), CodeFlowResources.Resources.Search,
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                    });
                }
                finally
                {
                    Monitor.Exit(searchLock);
                }
            }
        }

        private void CheckWholeWord(object sender, EventArgs e)
        {
            var command = sender as MenuCommand;
            command.Checked = !command.Checked;
            wholeWord = command.Checked;
        }

        private void CheckCaseSensitive(object sender, EventArgs e)
        {
            var command = sender as MenuCommand;
            command.Checked = !command.Checked;
            caseSensitive = command.Checked;
        }
    }
}
