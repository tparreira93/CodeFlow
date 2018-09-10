using System;
using System.Collections.Generic;
using System.IO;
using CodeFlow.GenioManual;
using CodeFlow.ManualOperations;
using CodeFlow.Utils;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CodeFlow.CommandHandler
{
    public class CommandHandler
    {
        public string GetCurrentViewText(out int cursorPos, out IWpfTextView textView)
        {
            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));
            var componentModel = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));
            var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
            textView = editor.GetWpfTextView(textViewCurrent);

            SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
            cursorPos = caretPosition.Position;
            return textView.TextBuffer.CurrentSnapshot.GetText();
        }
        public string GetCurrentSelection()
        {
            var dte = PackageOperations.Instance.DTE;
            string code = "";

            if (dte != null && dte.ActiveDocument != null)
            {
                var selection = (EnvDTE.TextSelection)dte.ActiveDocument.Selection;
                code = selection.Text;
            }
            return code;
        }
        public List<IManual> SearchForTags()
        {
            var dte = PackageOperations.Instance.DTE;
            List<IManual> manual = new List<IManual>();
            if (dte?.ActiveDocument == null)
                return manual;

            var code = GetCurrentSelection();
            VSCodeManualMatcher vSCodeManualMatcher = null;
            if (string.IsNullOrEmpty(code))
            {
                var pos = -1;
                code = GetCurrentViewText(out pos, out IWpfTextView _);
                vSCodeManualMatcher = new VSCodeManualMatcher(code, pos, dte.ActiveDocument.FullName);
            }
            else
                vSCodeManualMatcher = new VSCodeManualMatcher(code, dte.ActiveDocument.FullName);

            manual = vSCodeManualMatcher.Match();

            return manual;
        }
        public bool ImportAndEditCurrentTag()
        {
            string code = GetCurrentViewText(out int pos, out IWpfTextView textView);
            VSCodeManualMatcher vSCodeManualMatcher = new VSCodeManualMatcher(code, pos, PackageOperations.Instance.DTE.ActiveDocument.FullName);
            List<IManual> codeList = vSCodeManualMatcher.Match();
            if (codeList.Count == 1)
            {
                IManual manual = codeList[0];
                IManual bd = Manual.GetManual(manual.GetType(), manual.CodeId, PackageOperations.Instance.GetActiveProfile());
                if (bd == null)
                    return false;

                EditCodeSegment(textView.TextBuffer, manual.LocalMatch, bd.Code + (manual.LocalMatch.CodeLength == 0 ? Util.NewLine : String.Empty));
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
            string ext = Path.GetExtension(PackageOperations.Instance.DTE.ActiveDocument.ProjectItem.Name);
            var selection = (EnvDTE.TextSelection)PackageOperations.Instance.DTE.ActiveDocument.Selection;
            selection.Insert(man.FormatCode(ext));
        }
    }
}
