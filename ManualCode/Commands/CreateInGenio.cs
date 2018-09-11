using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.IO;
using CodeFlow.ManualOperations;
using CodeFlow.Forms;
using Task = System.Threading.Tasks.Task;

namespace CodeFlow.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreateInGenio
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4130;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("657c211f-0665-4969-81bc-d3a266b0aac4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInGenio"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CreateInGenio(AsyncPackage package, OleMenuCommandService commandService)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CreateInGenio Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
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
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new CreateInGenio(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
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
                if(genioForm.DialogResult == System.Windows.Forms.DialogResult.OK)
                    handler.InsertCreatedCode(man);
            }
        }
    }
}
