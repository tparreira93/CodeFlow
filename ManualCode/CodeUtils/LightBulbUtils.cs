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
using CodeFlow.ManualOperations;

namespace CodeFlow.CodeUtils
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Code flow suggestions")]
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

        private bool TryGetManual(out IManual manua, out CodeSegment segment)
        {
            int pos = textView.Caret.Position.BufferPosition.Position;
            string code = textView.TextViewModel.DataBuffer.CurrentSnapshot.GetText();
            manua = null;
            segment = null;

            if (!textView.Selection.IsEmpty)
                return false;

            segment = CodeSegment.ParseFromPosition(ManuaCode.BEGIN_MANUAL, ManuaCode.END_MANUAL, code, pos);
            if (segment.IsValid())
                code = segment.CompleteTextSegment;

            List<IManual> codeList = ManuaCode.GetManualCode(code);
            if(codeList.Count == 1)
                manua = codeList[0] as ManuaCode;

            return manua != null;
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                if (PackageOperations.ContinuousAnalysis && TryGetManual(out IManual man, out CodeSegment segment))
                {
                    return true;
                }
                return false;
            });
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            if (PackageOperations.ContinuousAnalysis 
                && !cancellationToken.IsCancellationRequested 
                && TryGetManual(out IManual man, out CodeSegment segment))
            {
                List<ISuggestedAction> actions = new List<ISuggestedAction>();

                CompareDBSuggestion compare = new CompareDBSuggestion(man);
                actions.Add(compare);

                ExportToGenioSuggestion export = new ExportToGenioSuggestion(man);
                actions.Add(export);
                
                CompareExportDBSuggestion compareExport = new CompareExportDBSuggestion(man);
                actions.Add(compareExport);

                if (segment.SegmentStart > 0 && segment.SegmentLength > 0)
                {
                    ImportFromDBSuggestion import = new ImportFromDBSuggestion(segment.SegmentStart, segment.SegmentLength, textView, textBuffer, man.CodeId);
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
