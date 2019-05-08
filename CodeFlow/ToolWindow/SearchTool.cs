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
    using CodeFlowUI.Controls.Editor;
    using CodeFlow.Editor;
    using CodeFlowLibrary.Solution;

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
    public class SearchTool : ToolWindowPane, IVsWindowFrameNotify3, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget, IVsFindTarget, IVsFindTarget2
        //ToolWindowPane//, IOleCommandTarget//, IVsWindowFrameNotify3
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


        private bool closed = true;
        private SearchToolControl control = null;
        public static bool WindowInitialized = false;
        private string plataform = "";
        private string currentSearch = "";
        private bool caseSensitive = false;
        private bool wholeWord = false;
        private readonly object searchLock = new object();
        private ICodeEditor editor;

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
            editor = GetCodeEditor(PackageOptions.ShowPreview, PackageOptions.SearchPreviewOption);
            control = new SearchToolControl(PackageOptions.ShowPreview, editor);
            Content = control;

            PackageOptions.OnPreviewOptionChanged += OnPreviewChanged;
            closed = editor == null;
        }

        private ICodeEditor GetCodeEditor(bool searchPreview, PreviewOption option)
        {
            ICodeEditor editor = null;
            if (searchPreview)
            {
                if (option == PreviewOption.Simple)
                    editor = new AvalonCodeEditor(PackageBridge.Flow);
                else
                    editor = new VisualStudioCodeEditor(PackageBridge.Flow);
            }

            return editor;
        }

        private void OnPreviewChanged(bool searchPreview, PreviewOption option)
        {
            editor = GetCodeEditor(searchPreview, option);
            closed = editor == null;
            control.UpdateOptions(searchPreview, editor);
        }


        /// <summary>
        /// This is called after our control has been created and sited.
        /// This is a good place to initialize the control with data gathered
        /// from Visual Studio services.
        /// </summary>
        public override void OnToolWindowCreated()
        {
            ////We need to set up the tool window to respond to key bindings
            ////They're passed to the tool window and its buffers via Query() and Exec()
            //var windowFrame = (IVsWindowFrame)Frame;
            //var cmdUi = VSConstants.GUID_TextEditorFactory;
            //windowFrame.SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref cmdUi);

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

        #region Test


        public int OnShow(int fShow)
        {
            if (fShow == 7 && !closed)
                closed = true;
            return 0;
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
            return 0;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            return 0;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            return 0;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            return 0;
        }

        public int Find(
          string pszSearch,
          uint grfOptions,
          int fResetStartPoint,
          IVsFindHelper pHelper,
          out uint pResult)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.Find(pszSearch, grfOptions, fResetStartPoint, pHelper, out pResult);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            pResult = 7U;
            return 1;
        }

        public int GetCapabilities(bool[] pfImage, uint[] pgrfOptions)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.GetCapabilities(pfImage, pgrfOptions);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int GetCurrentSpan(TextSpan[] pts)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.GetCurrentSpan(pts);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int GetFindState(out object ppunk)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.GetFindState(out ppunk);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ppunk = (object)null;
            return 1;
        }

        public int GetMatchRect(RECT[] prc)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.GetMatchRect(prc);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int GetProperty(uint propid, out object pvar)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.GetProperty(propid, out pvar);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            pvar = (object)null;
            return 1;
        }

        public int GetSearchImage(
          uint grfOptions,
          IVsTextSpanSet[] ppSpans,
          out IVsTextImage ppTextImage)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.GetSearchImage(grfOptions, ppSpans, out ppTextImage);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ppTextImage = (IVsTextImage)null;
            return 1;
        }

        public int MarkSpan(TextSpan[] pts)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.MarkSpan(pts);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int NavigateTo(TextSpan[] pts)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.NavigateTo(pts);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int NotifyFindTarget(uint notification)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.NotifyFindTarget(notification);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int Replace(
          string pszSearch,
          string pszReplace,
          uint grfOptions,
          int fResetStartPoint,
          IVsFindHelper pHelper,
          out int pfReplaced)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.Replace(pszSearch, pszReplace, grfOptions, fResetStartPoint, pHelper, out pfReplaced);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            pfReplaced = 0;
            return 1;
        }

        public int SetFindState(object pUnk)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget findTarget = editor.CodeAdapter.GetFindTarget();
                    if (findTarget != null)
                        return findTarget.SetFindState(pUnk);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        public int NavigateTo2(IVsTextSpanSet pSpans, TextSelMode iSelMode)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    IVsFindTarget2 findTarget2 = editor.CodeAdapter.GetFindTarget2();
                    if (findTarget2 != null)
                        return findTarget2.NavigateTo2(pSpans, iSelMode);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return 1;
        }

        protected override bool PreProcessMessage(ref Message m)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    if (editor.CodeAdapter.PreProcessMessage(ref m))
                        return true;
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return base.PreProcessMessage(ref m);
        }

        int IOleCommandTarget.Exec(
          ref Guid pguidCmdGroup,
          uint nCmdID,
          uint nCmdexecopt,
          IntPtr pvaIn,
          IntPtr pvaOut)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    if (editor.CodeAdapter.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut, out int result))
                        return result;
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return -2147221248;
        }

        int IOleCommandTarget.QueryStatus(
          ref Guid pguidCmdGroup,
          uint cCmds,
          OLECMD[] prgCmds,
          IntPtr pCmdText)
        {
            try
            {
                if (!this.closed && editor.CodeAdapter != null)
                {
                    if (editor.CodeAdapter.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText, out int result))
                        return result;
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, e.Message), CodeFlowResources.Resources.Search,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return -2147221248;
        }

        #endregion
    }
}
