using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Bridge;

namespace CodeFlow.CodeUtils.Suggestions
{
    internal class UpdateSuggestion : ISuggestedAction
    {
        private readonly Guid codmanua;
        private readonly string display;
        private readonly ITextBuffer textBuffer;
        private readonly ITextView textView;
        private readonly CodeFlowPackage package;
        private readonly int begin;
        private readonly int end;
        private readonly Profile _profile;

        public UpdateSuggestion(CodeFlowPackage package, int begin, int end, ITextView textView, ITextBuffer textBuffer, Guid codmanua, Profile profile)
        {
            this.textBuffer = textBuffer;
            this.textView = textView;
            this.codmanua = codmanua;
            this.package = package;
            this.begin = begin;
            this.end = end;
            this.display = string.Format("Update manual code.");
            _profile = profile;
        }

        public string DisplayText
        {
            get
            {
                return display;
            }
        }

        public string IconAutomationText
        {
            get
            {
                return null;
            }
        }

        ImageMoniker ISuggestedAction.IconMoniker
        {
            get
            {
                return default(ImageMoniker);
            }
        }

        public string InputGestureText
        {
            get
            {
                return null;
            }
        }

        public bool HasActionSets
        {
            get
            {
                return false;
            }
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public bool HasPreview
        {
            get
            {
                return false;
            }
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                ManuaCode bd = ManuaCode.GetManual(_profile, codmanua);
                if (bd == null)
                    return;
                Handlers.CommandHandler handler = new Handlers.CommandHandler(package);
                handler.EditCodeSegment(textView.TextBuffer, begin, end, bd.Code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, ex.Message),
                    CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
