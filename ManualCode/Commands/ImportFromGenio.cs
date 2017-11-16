using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using System.Windows.Forms;

namespace CodeFlow
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ImportFromGenio
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("657c211f-0665-4969-81bc-d3a266b0aac4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportFromGenio"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ImportFromGenio(Package package)
        {
            this.package = package ?? throw new ArgumentNullException("package");

            if (this.ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ImportFromGenio Instance
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
            Instance = new ImportFromGenio(package);
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
            var dte = this.ServiceProvider.GetService(typeof(DTE)) as _DTE;
            string code = "", subCode = "";

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
                int begin = -1, length = -1;
                int end = 0;

                code = view.TextViewModel.DataBuffer.CurrentSnapshot.GetText();

                if (code != null)
                {
                    begin = code.LastIndexOf(ManuaCode.BEGIN_MANUAL, pos, pos + 1);
                    end = code.IndexOf(ManuaCode.END_MANUAL, pos) + ManuaCode.END_MANUAL.Length;
                }

                if (begin != -1 && begin <= pos && end > begin)
                {
                    length = end - begin;
                    subCode = code.Substring(begin, length);
                }

                List<IManual> codeList = ManuaCode.GetManualCode(subCode);
                if (codeList.Count == 1 && codeList[0] is ManuaCode)
                {
                    int dif = code.IndexOf(Utils.Util.NewLine, begin) - begin + Utils.Util.NewLine.Length;
                    begin = code.IndexOf(Utils.Util.NewLine, begin) + Utils.Util.NewLine.Length;
                    length -= dif;

                    length = code.LastIndexOf(Utils.Util.NewLine, end, Math.Abs(begin - length)) - begin;

                    ManuaCode bd = ManuaCode.GetManual(codeList[0].CodeId, PackageOperations.ActiveProfile);
                    if (bd == null)
                    {
                        MessageBox.Show(Properties.Resources.VerifyProfile, "Conflict resolve", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var point = view.Caret.Position.BufferPosition;
                    int position = point.Position;

                    using (var edit = view.TextBuffer.CreateEdit())
                    {
                        edit.Replace(begin, length, bd.Code);
                        edit.Apply();
                    }
                }
            }
        }
    }
}
