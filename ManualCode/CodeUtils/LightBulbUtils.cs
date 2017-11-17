using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Threading;
using CodeFlow.SolutionOperations;

namespace CodeFlow.CodeUtils
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Test Suggested Actions")]
    [ContentType("text")]
    internal class CodeFlowSuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            if (textBuffer == null && textView == null)
            {
                return null;
            }
            return new CodeFlowSuggestedActionsSource(this, textView, textBuffer);
        }
    }

    internal class CodeFlowSuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly CodeFlowSuggestedActionsSourceProvider factory;
        private readonly ITextBuffer textBuffer;
        private readonly ITextView textView;
#pragma warning disable CS0067 // #warning directive
        public event EventHandler<EventArgs> SuggestedActionsChanged;
#pragma warning restore CS0067 // #warning directive

        public CodeFlowSuggestedActionsSource(CodeFlowSuggestedActionsSourceProvider testSuggestedActionsSourceProvider, ITextView textView, ITextBuffer textBuffer)
        {
            factory = testSuggestedActionsSourceProvider;
            this.textBuffer = textBuffer;
            this.textView = textView;
        }
        private bool TryGetWordUnderCaret(out TextExtent wordExtent)
        {
            ITextCaret caret = textView.Caret;
            SnapshotPoint point;

            if (caret.Position.BufferPosition > 0)
            {
                point = caret.Position.BufferPosition - 1;
            }
            else
            {
                wordExtent = default(TextExtent);
                return false;
            }
            ITextStructureNavigator navigator = factory.NavigatorService.GetTextStructureNavigator(textBuffer);

            wordExtent = navigator.GetExtentOfWord(point);
            return true;
        }

        private bool TryGetManual(out ManuaCode manua, out int begin, out int length)
        {
            int pos = textView.Caret.Position.BufferPosition.Position;
            string subCode = "";
            string code = textView.TextViewModel.DataBuffer.CurrentSnapshot.GetText();
            begin = -1;
            length = -1;
            int end = 0;
            int platBegin = -1;
            int platEnd = -1;

            if (code != null && !code.Equals("") && code.Length >= ManuaCode.BEGIN_MANUAL.Length)
            {
                begin = code.LastIndexOf(ManuaCode.BEGIN_MANUAL, pos, pos + 1);
                end = code.IndexOf(ManuaCode.END_MANUAL, pos) + ManuaCode.END_MANUAL.Length;
            }

            if (begin != -1 && begin <= pos && end > begin)
            {
                platBegin = code.LastIndexOf(Utils.Util.NewLine, begin);
                platEnd = code.LastIndexOf(Utils.Util.NewLine, platBegin);
                length = end - begin;
                subCode = code.Substring(begin, length);
            }

            List<IManual> codeList = ManuaCode.GetManualCode(subCode);
            if(codeList.Count == 1)
            {
                int dif = code.IndexOf(Utils.Util.NewLine, begin) - begin + Utils.Util.NewLine.Length;
                begin = code.IndexOf(Utils.Util.NewLine, begin) + Utils.Util.NewLine.Length;
                length -= dif;

                length = code.LastIndexOf(Utils.Util.NewLine, end, Math.Abs(begin - length)) - begin;

                manua = codeList[0] as ManuaCode;
                return true;
            }
            manua = null;
            return false;
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (PackageOperations.ContinuousAnalysis && TryGetManual(out ManuaCode man, out int begin, out int end))
                {
                    // don't display the action if the extent has whitespace  
                    return true;
                }
                return false;
            });
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            if (PackageOperations.ContinuousAnalysis 
                && !cancellationToken.IsCancellationRequested 
                && TryGetManual(out ManuaCode man, out int begin, out int end))
            {
                List<ISuggestedAction> actions = new List<ISuggestedAction>();
                CompareDBSuggestion compare = new CompareDBSuggestion(man);
                actions.Add(compare);

                if (begin > 0 && end > 0)
                {
                    ImportFromDBSuggestion import = new ImportFromDBSuggestion(begin, end, textView, textBuffer, man.CodeId);
                    actions.Add(import);
                }

                if (PackageOperations.SolutionProps != null
                    && PackageOperations.ActiveProfile.GenioConfiguration.CheckoutPath.Length > 0
                    && PackageOperations.SolutionProps.ClientInfo.System.Length > 0)
                {
                    OpenSVNSuggestion openSVNSuggestion = new OpenSVNSuggestion(man, PackageOperations.ActiveProfile, PackageOperations.SolutionProps.ClientInfo.System);
                    actions.Add(openSVNSuggestion);
                }
                return new SuggestedActionSet[] { new SuggestedActionSet(actions.ToArray()) };
            }
            return Enumerable.Empty<SuggestedActionSet>();
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry  
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
