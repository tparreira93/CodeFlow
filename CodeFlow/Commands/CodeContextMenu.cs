using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeFlowUI.Manager;
using CodeFlowLibrary.GenioCode;
using CodeFlowUI;
using CodeFlowLibrary.CodeControl.Analyzer;
using CodeFlowLibrary.Bridge;
using CodeFlowLibrary.Genio;
using System.Threading.Tasks;
using CodeFlow.Utils;
using CodeFlow.Handlers;

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
        private readonly CodeFlowPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenu"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ContextMenuCommand(CodeFlowPackage package, OleMenuCommandService commandService)
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
        public static async Task InitializeAsync(CodeFlowPackage package)
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
            VsCommander.Commit(package);
        }

        private void ImportGenio(object sender, EventArgs e)
        {
            VsCommander.Update(package);
        }

        private void CreateGenio(object sender, EventArgs e)
        {
            VsCommander.Create(package);
        }
    }
}