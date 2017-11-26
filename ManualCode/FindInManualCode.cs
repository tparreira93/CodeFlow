namespace CodeFlow
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Shell.Interop;

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
    [Guid("93a46544-0f88-491b-b820-06fddb518d20")]
    public class FindInManualCode : ToolWindowPane
    {
        private const int cmdidSearchToolbar = 4136;
        private const int cmdidSearchToolbarGroup = 4137;
        private const int cmdidSearchManualCode = 4138;
        private const int cmdIdSearchBox = 4139;
        private const int cmdIdCaseSensitive = 4140;
        private const int cmdIdWholeWord = 4141;

        public static bool WindowInitialized = false;
        private string currentSearch = "";
        private bool caseSensitive = false;
        private bool wholeWord = false;


        // Control that will be hosted in the tool window
        private FindManwinControl control = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="FindInManualCode"/> class.
        /// </summary>
        public FindInManualCode() : base(null)
        {
            this.Caption = "Find manual code";

            //BitmapImageMoniker = Microsoft.VisualStudio.Imaging.KnownMonikers.Search;
            //ToolBar = new CommandID(new Guid(CodeFlowPackage.PackageGuidString), cmdidSearchToolbar);
            // Specify that we want the toolbar at the top of the window
            //ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            control = new FindManwinControl();
            Content = control;
            WindowInitialized = true;

        }
        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            OleMenuCommandService commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {

                var menuCommandID = new CommandID(new Guid(CodeFlowPackage.PackageGuidString), cmdidSearchManualCode);
                var command = new MenuCommand(this.SearchManualCode, menuCommandID);
                commandService.AddCommand(command);

                menuCommandID = new CommandID(new Guid(CodeFlowPackage.PackageGuidString), cmdIdSearchBox);
                command = new MenuCommand(this.SearchTerm, menuCommandID);
                commandService.AddCommand(command);

                /*var menuCommandID = new CommandID(new Guid(CodeFlowPackage.PackageGuidString), cmdIdCaseSensitive);
                var command = new MenuCommand(this.CheckCaseSensitive, menuCommandID);
                commandService.AddCommand(command);

                menuCommandID = new CommandID(new Guid(CodeFlowPackage.PackageGuidString), cmdIdWholeWord);
                command = new MenuCommand(this.CheckWholeWord, menuCommandID);
                commandService.AddCommand(command);*/
            }
            control.InitializeComponent();
        }
        public override void OnToolBarAdded()
        {
            base.OnToolBarAdded();

            // In general it is not useful to override this method,
            // but it is useful when the tool window hosts a toolbar
            // with a drop-down (combo box) that needs to be initialized.
            // If that were the case, the initalization would happen here.
        }
        private void SearchManualCode(object sender, EventArgs e)
        {
            if (currentSearch == null)
                return;

            PackageOperations.CurrentSearch = currentSearch;
            PackageOperations.WholeWordSearch = wholeWord;
            PackageOperations.CaseSensitive = caseSensitive;
        }
        private void SearchTerm(object sender, EventArgs e)
        {
            if (e == EventArgs.Empty)
                throw (new ArgumentException("No event args"));

            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
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
                        currentSearch = newChoice;

                    else
                        throw (new ArgumentException("Empty string is not accepted"));
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
        public void SetComboData(List<string> data)
        {
            if (Content is null)
                return;
            FindManwinControl control = Content as FindManwinControl;
            control.SetComboData(data);
        }
    }
}
