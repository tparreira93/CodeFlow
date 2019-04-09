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
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Settings;
using CodeFlowLibrary.Bridge;
using CodeFlow.Utils;
using CodeFlow.Handlers;
using CodeFlowLibrary.Package;

namespace CodeFlow.CodeUtils.Suggestions
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

        private async Task<(bool exists, IManual code)> TryGetManualAsync()
        {
            IManual manual = null;

            CodeFlowPackage package = PackageBridge.Flow as CodeFlowPackage;
            CommandHandler handler = new CommandHandler(package);
            if (PackageOptions.ContinuousAnalysis && package.Active.IsValid() && string.IsNullOrEmpty(await handler.GetCurrentSelectionAsync()))
            {
                var view = await handler.GetCurrentViewTextAsync();
                VSCodeManualMatcher vSCodeManualMatcher = new VSCodeManualMatcher(view.code, view.cursorPos, view.fullDocumentName);
                List<IManual> codeList = vSCodeManualMatcher.Match();

                if (codeList.Count == 1)
                    manual = codeList[0];
            }
            return (manual != null, manual);
        }

        private async Task<bool> ExistsManualAsync()
        {
            var (exists, code) = await TryGetManualAsync();
            return exists;
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return ExistsManualAsync();
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var (exists, code) = AsyncHelper.RunSync(async () => await TryGetManualAsync());
            if (!cancellationToken.IsCancellationRequested && exists)
            {
                List<ISuggestedAction> actions = new List<ISuggestedAction>();
                CodeFlowPackage package = PackageBridge.Flow as CodeFlowPackage;
                Profile profile = package.Active;
                CompareDBSuggestion compare = new CompareDBSuggestion(package, code, profile);
                actions.Add(compare);

                CommitSuggestion export = new CommitSuggestion(package, code, profile);
                actions.Add(export);
                
                CompareCommitBSuggestion compareExport = new CompareCommitBSuggestion(package, code, profile);
                actions.Add(compareExport);

                if (code.LocalMatch.CodeStart > 0 && code.LocalMatch.CodeLength > 0)
                {
                    UpdateSuggestion import = new UpdateSuggestion(package, code.LocalMatch.CodeStart, code.LocalMatch.CodeLength, textView, textBuffer, code.CodeId, profile);
                    actions.Add(import);
                }

                if (!String.IsNullOrEmpty(profile.GenioConfiguration.CheckoutPath) && !String.IsNullOrEmpty(profile.GenioConfiguration.SystemInitials))
                {
                    OpenSVNSuggestion openSVNSuggestion = new OpenSVNSuggestion(package, code, profile);
                    actions.Add(openSVNSuggestion);

                    BlameSVNSuggestion blameSVNSuggestion = new BlameSVNSuggestion(package, code, profile);
                    actions.Add(blameSVNSuggestion);
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
