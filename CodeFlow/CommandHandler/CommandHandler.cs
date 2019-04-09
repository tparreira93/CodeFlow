using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Util;
using CodeFlowBridge;
using System.Threading.Tasks;
using CodeFlowLibrary.Genio;
using CodeFlowUI;
using System.Windows.Forms;

namespace CodeFlow.CommandHandler
{
    public class CommandHandler
    {
        CodeFlowPackage package;

        public CommandHandler()
        {
            package = PackageBridge.Flow as CodeFlowPackage;
        }

        public async Task<(string code, int cursorPos, IWpfTextView textView, string fullDocumentName)> GetCurrentViewTextAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));
            Assumes.Present(textManager);
            var componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
            Assumes.Present(componentModel);
            var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
            IWpfTextView textView = editor.GetWpfTextView(textViewCurrent);

            SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
            int cursorPos = caretPosition.Position;
            return (textView.TextBuffer.CurrentSnapshot.GetText(), cursorPos, textView, package.DTE.ActiveDocument.FullName);
        }
        public async Task<string> GetCurrentSelectionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = package.DTE;
            string code = "";

            if (dte != null && dte.ActiveDocument != null)
            {
                var selection = (TextSelection)dte.ActiveDocument.Selection;
                code = selection.Text;
            }
            return code;
        }
        public async Task<List<IManual>> SearchForTagsAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = package.DTE;
            List<IManual> manual = new List<IManual>();
            if (dte?.ActiveDocument == null)
                return manual;

            var code = await GetCurrentSelectionAsync();
            VSCodeManualMatcher vSCodeManualMatcher = null;
            if (string.IsNullOrEmpty(code))
            {
                var view = await GetCurrentViewTextAsync();
                vSCodeManualMatcher = new VSCodeManualMatcher(view.code, view.cursorPos, view.fullDocumentName);
            }
            else
                vSCodeManualMatcher = new VSCodeManualMatcher(code, dte.ActiveDocument.FullName);

            manual = vSCodeManualMatcher.Match();

            return manual;
        }
        public async Task<bool> ImportAndEditCurrentTagAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var view = await GetCurrentViewTextAsync();

            VSCodeManualMatcher vSCodeManualMatcher = new VSCodeManualMatcher(view.code, view.cursorPos, view.fullDocumentName);
            List<IManual> codeList = vSCodeManualMatcher.Match();
            if (codeList.Count == 1)
            {
                IManual manual = codeList[0];
                IManual bd = Manual.GetManual(manual.GetType(), manual.CodeId, PackageBridge.Instance.GetActiveProfile());
                if (bd == null)
                    return false;

                EditCodeSegment(view.textView.TextBuffer, manual.LocalMatch, bd.Code + (manual.LocalMatch.CodeLength == 0 ? Helpers.NewLine : String.Empty));
            }

            return true;
        }
        public void EditCodeSegment(ITextBuffer textBuffer, ManualMatch match, string code)
        {
            EditCodeSegment(textBuffer, match.CodeStart, match.CodeLength, code);
        }
        public void EditCodeSegment(ITextBuffer textBuffer, int begin, int length, string code)
        {
            using (var edit = textBuffer.CreateEdit())
            {
                edit.Replace(begin, length, code);
                edit.Apply();
            }
        }
        public void InsertCreatedCode(IManual man)
        {
           Utils.AsyncHelper.RunSyncUI(() =>
           {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
               string ext = Path.GetExtension(package.DTE.ActiveDocument.ProjectItem.Name);
               var selection = (TextSelection)package.DTE.ActiveDocument.Selection;
               selection.Insert(man.FormatCode(ext));
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
           });
        }
        public async Task<string> GetActiveDocumentNameAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return package.DTE.ActiveDocument.FullName;
        }
    }
}
