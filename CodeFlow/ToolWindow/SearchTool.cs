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
    using CodeFlowBridge;

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
    public class SearchTool : ToolWindowPane//, IVsWindowFrameNotify3
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


        private SearchToolControl control = null;
        public static bool WindowInitialized = false;
        private string plataform = "";
        private string currentSearch = "";
        private bool caseSensitive = false;
        private bool wholeWord = false;
        private readonly object searchLock = new object();
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

            this.control = new SearchToolControl();
            Content = control;
        }


        /// <summary>
        /// This is called after our control has been created and sited.
        /// This is a good place to initialize the control with data gathered
        /// from Visual Studio services.
        /// </summary>
        public override void OnToolWindowCreated()
        {
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
                OleMenuCommand searchBoxCommand = new OleMenuCommand(new EventHandler(SearchTerm), menuCommandID)
                {
                    ParametersDescription = "$" // accept any argument string
                };
                searchBoxCommand.CommandChanged += SearchTerm;
                commandService.AddCommand(searchBoxCommand);

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
                {
                    plataform = newChoice;
                }
                else
                    throw (new ArgumentException("Invalid input and output!"));
            }
            else
                throw (new ArgumentException("Invalid combo box call!"));
        }
        private void GetPlataformList(object sender, EventArgs e)
        {
            string[] dropChoices = new string[PackageBridge.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Count + 1];
            dropChoices[0] = "All";

            for (int i = 1; i < PackageBridge.Instance.GetActiveProfile().GenioConfiguration.Plataforms.Count; i++)
            {
                dropChoices[i] = PackageBridge.Instance.GetActiveProfile().GenioConfiguration.Plataforms[i].ID;
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
            if (Monitor.TryEnter(searchLock, 2000))
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
                        Profile p = PackageBridge.Instance.GetActiveProfile();
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
                            control.RefreshteList(res, new CodeFlowLibrary.Settings.SearchOptions(p, currentSearch, wholeWord, caseSensitive));
                            cmd.Enabled = true;

                            if (error.Length != 0)
                            {
                                MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorSearch, error), CodeFlowResources.Resources.Search,
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
