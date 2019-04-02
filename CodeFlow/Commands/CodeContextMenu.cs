using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using CodeFlow.ToolWindow;
using Task = System.Threading.Tasks.Task;
using CodeFlow.GenioManual;
using System.Collections.Generic;
using CodeFlow.CodeControl.Analyzer;
using CodeFlow.Forms;
using System.Windows.Forms;
using CodeFlow.ManualOperations;
using CodeFlowLibrary.GenioCode;

namespace CodeFlow.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ContextMenuCommand
    {
        /// <summary>
        /// Submit Command ID.
        /// </summary>
        public const int SubmitCommandId = 5130;
        /// <summary>
        /// Import Command ID.
        /// </summary>
        public const int ImportCommandId = 5131;
        /// <summary>
        /// Create Command ID.
        /// </summary>
        public const int CreateCommandId = 5132;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid ContextMenuSet = new Guid("7547ebce-1079-4c06-8b74-bfe595d61e7e");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenu"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ContextMenuCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;
            
            if (commandService != null)
            {
                var commitCommand = new CommandID(ContextMenuSet, SubmitCommandId);
                var commitMenuItem = new MenuCommand(this.SubmitGenio, commitCommand);
                commandService.AddCommand(commitMenuItem);


                var updateCommand = new CommandID(ContextMenuSet, ImportCommandId);
                var updateMenuItem = new MenuCommand(this.ImportGenio, updateCommand);
                commandService.AddCommand(updateMenuItem);


                var createCommand = new CommandID(ContextMenuSet, CreateCommandId);
                var createMenuItem = new MenuCommand(this.CreateGenio, createCommand);
                commandService.AddCommand(createMenuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ContextMenuCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.Interop.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ContextMenuCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new ContextMenuCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void SubmitGenio(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                CommandHandler.CommandHandler handler = new CommandHandler.CommandHandler();
                List<IManual> manual = handler.SearchForTags();
                ChangeAnalyzer diffs = new ChangeAnalyzer();
                diffs.CheckForDifferences(manual, PackageOperations.Instance.GetActiveProfile());
                CommitForm commitForm = new CommitForm(diffs);
                CodeFlowFormManager.Open(commitForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.UnableToExecuteOperation, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void ImportGenio(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                CommandHandler.CommandHandler handler = new CommandHandler.CommandHandler();
                if (!handler.ImportAndEditCurrentTag())
                {
                    MessageBox.Show(Properties.Resources.VerifyProfile, Properties.Resources.Import, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.UnableToExecuteOperation, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        private void CreateGenio(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                CommandHandler.CommandHandler handler = new CommandHandler.CommandHandler();
                string code = handler.GetCurrentSelection();

                if (!string.IsNullOrEmpty(code))
                {
                    ManuaCode man = new ManuaCode(code);
                    ManualMatch manualMatch = new ManualMatch();
                    manualMatch.FullFileName = PackageOperations.Instance.DTE.ActiveDocument.FullName;
                    man.LocalMatch = manualMatch;
                    CreateInGenioForm genioForm = new CreateInGenioForm(man);
                    genioForm.ShowDialog();
                    if (genioForm.DialogResult == DialogResult.OK)
                        handler.InsertCreatedCode(man);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.UnableToExecuteOperation, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
