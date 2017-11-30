using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using CodeFlow.CommandHandlers;
using System.Windows.Forms;
using System.IO;

namespace CodeFlow.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ContextMenu
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
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextMenu"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ContextMenu(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var submitCommand = new CommandID(ContextMenuSet, SubmitCommandId);
                var submitMenuItem = new MenuCommand(this.SubmitGenio, submitCommand);
                commandService.AddCommand(submitMenuItem);


                var importCommand = new CommandID(ContextMenuSet, ImportCommandId);
                var importMenuItem = new MenuCommand(this.ImportGenio, importCommand);
                commandService.AddCommand(importMenuItem);


                var createCommand = new CommandID(ContextMenuSet, CreateCommandId);
                var createMenuItem = new MenuCommand(this.CreateGenio, createCommand);
                commandService.AddCommand(createMenuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ContextMenu Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
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
        public static void Initialize(Package package)
        {
            Instance = new ContextMenu(package);
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
            List<IManual> manual = CommandHandler.SearchForTags(ServiceProvider);

            CommitForm exportForm = new CommitForm(manual);
            exportForm.ShowDialog();
        }

        private void ImportGenio(object sender, EventArgs e)
        {
            if (!CommandHandlers.CommandHandler.ImportAndEditCurrentTag(ServiceProvider))
            {
                MessageBox.Show(Properties.Resources.VerifyProfile, Properties.Resources.Import, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CreateGenio(object sender, EventArgs e)
        {
            string code = CommandHandlers.CommandHandler.GetCurrentSelection(ServiceProvider);

            if (code != null && code.Length != 0)
            {
                ManuaCode man = new ManuaCode(code);
                CreateInGenioForm genioForm = new CreateInGenioForm(man);
                genioForm.ShowDialog();
                if(genioForm.DialogResult == DialogResult.OK)
                {
                    string ext = Path.GetExtension(PackageOperations.DTE.ActiveDocument.ProjectItem.Name);
                    var selection = (EnvDTE.TextSelection)PackageOperations.DTE.ActiveDocument.Selection;
                    selection.Insert(man.FormatCode(ext));
                }
            }
        }
    }
}
