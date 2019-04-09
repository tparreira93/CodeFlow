using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using CodeFlow.SolutionOperations;
using Task = System.Threading.Tasks.Task;
using CodeFlowLibrary.Bridge;
using CodeFlow.SolutionAnalyzer;
using CodeFlowLibrary.Package;

namespace CodeFlow.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RefreshSolution
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4135;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("657c211f-0665-4969-81bc-d3a266b0aac4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly CodeFlowPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshSolution"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private RefreshSolution(CodeFlowPackage package, OleMenuCommandService commandService)
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
        public static RefreshSolution Instance
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
        public static async Task InitializeAsync(CodeFlowPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RefreshSolution's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new RefreshSolution(package, commandService);
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
            SolutionParser parser = new SolutionParser(package as ICodeFlowPackage);
#pragma warning disable VSTHRD110 // Observe result of async calls
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            parser.ParseAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning restore VSTHRD110 // Observe result of async calls
        }
    }
}
