using CodeFlow.ManualOperations;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;

namespace CodeFlow.CommandHandlers
{
    internal static class CommandHandler
    {
        public static string GetCurrentViewText(IServiceProvider serviceProvider, out int cursorPos, out IWpfTextView textView)
        {
            var textManager = (IVsTextManager)serviceProvider.GetService(typeof(SVsTextManager));
            var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));
            var editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            textManager.GetActiveView(1, null, out IVsTextView textViewCurrent);
            textView = editor.GetWpfTextView(textViewCurrent);

            SnapshotPoint caretPosition = textView.Caret.Position.BufferPosition;
            cursorPos = caretPosition.Position;
            return textView.TextBuffer.CurrentSnapshot.GetText();
        }
        public static string GetCurrentSelection(IServiceProvider serviceProvider)
        {
            var dte = PackageOperations.DTE;
            string code = "";

            if (dte != null && dte.ActiveDocument != null)
            {
                var selection = (EnvDTE.TextSelection)dte.ActiveDocument.Selection;
                code = selection.Text;
            }
            return code;
        }
        public static List<IManual> SearchForTags(IServiceProvider serviceProvider)
        {
            var dte = PackageOperations.DTE;
            List<IManual> manual = new List<IManual>();
            string code = "";
            string subCode = "";

            if (dte == null || dte.ActiveDocument == null)
                return manual;

            code = GetCurrentSelection(serviceProvider);

            if (code != null && code.Length != 0)
            {
                manual.AddRange(ManuaCode.GetManualCode(code, dte.ActiveDocument.Name).ToArray());
                manual.AddRange(CustomFunction.GetManualCode(code, dte.ActiveDocument.Name).ToArray());
            }
            else
            {
                code = GetCurrentViewText(serviceProvider, out int pos, out IWpfTextView textView);

                CodeSegment segment = CodeSegment.ParseFromPosition(ManuaCode.BEGIN_MANUAL, ManuaCode.END_MANUAL, code, pos);
                if (segment.IsValid())
                {
                    subCode = segment.CompleteTextSegment;
                    manual.AddRange(ManuaCode.GetManualCode(subCode, dte.ActiveDocument.Name).ToArray());
                }

                segment = CodeSegment.ParseFromPosition(CustomFunction.BEGIN_MANUAL, CustomFunction.END_MANUAL, code, pos);
                if (segment.IsValid())
                {
                    subCode = segment.CompleteTextSegment;
                    manual.AddRange(CustomFunction.GetManualCode(subCode, dte.ActiveDocument.Name).ToArray());
                }
            }

            return manual;
        }
        public static bool ImportAndEditCurrentTag(IServiceProvider serviceProvider)
        {
            string code = GetCurrentViewText(serviceProvider, out int pos, out IWpfTextView textView);
            string subCode = "";
            CodeSegment segment = CodeSegment.ParseFromPosition(ManuaCode.BEGIN_MANUAL, ManuaCode.END_MANUAL, code, pos);
            if (segment.IsValid())
                subCode = segment.CompleteTextSegment;

            List<IManual> codeList = ManuaCode.GetManualCode(subCode);
            if (codeList.Count == 1 && codeList[0] is ManuaCode)
            {
                ManuaCode bd = ManuaCode.GetManual(PackageOperations.GetActiveProfile(), codeList[0].CodeId);
                if (bd == null)
                    return false;

                EditCodeSegment(textView.TextBuffer, segment, bd.Code);
            }

            return true;
        }
        public static void EditCodeSegment(ITextBuffer textBuffer, CodeSegment segment, string code)
        {
            EditCodeSegment(textBuffer, segment.SegmentStart, segment.SegmentLength, code);
        }
        public static void EditCodeSegment(ITextBuffer textBuffer, int begin, int length, string code)
        {
            using (var edit = textBuffer.CreateEdit())
            {
                edit.Replace(begin, length, code);
                edit.Apply();
            }
        }
    }
}
