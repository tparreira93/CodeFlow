using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using System.Linq;
using System.IO;
using CodeFlow.ManualOperations;

namespace CodeFlow
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ExportToGenio
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("657c211f-0665-4969-81bc-d3a266b0aac4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportToGenio"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ExportToGenio(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ExportToGenio Instance
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
            Instance = new ExportToGenio(package);
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
            var dte = ServiceProvider.GetService(typeof(DTE)) as _DTE;

            string code = "";
            string subCode = "";

            if (dte == null || dte.ActiveDocument == null)
                return;

            var selection = (TextSelection)dte.ActiveDocument.Selection;
            code = selection.Text;

            if (code != null && code.Length != 0)
            {
                subCode = code;
            }
            else
            {
                var textManager = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
                var componentModel = (IComponentModel)this.ServiceProvider.GetService(typeof(SComponentModel));
                var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
                IWpfTextView view = editor.GetWpfTextView(textViewCurrent);

                SnapshotPoint caretPosition = view.Caret.Position.BufferPosition;
                int pos = caretPosition.Position;
                CodeSegment segment = CodeSegment.ParseFromPosition(ManuaCode.BEGIN_MANUAL, ManuaCode.END_MANUAL, code, pos);
                if (segment.IsValid())
                    subCode = segment.CompleteTextSegment;
            }

            List<IManual> manual = ManuaCode.GetManualCode(subCode);
            manual.AddRange(CustomFunction.GetManualCode(subCode));
            ExportForm exportForm = new ExportForm(manual);
            exportForm.ShowDialog();
        }
    }
}
