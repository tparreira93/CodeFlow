using CodeFlow.ManualOperations;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.CodeUtils
{
    internal class UpdateSuggestion : ISuggestedAction
    {
        private readonly Guid codmanua;
        private readonly string display;
        private readonly ITextBuffer textBuffer;
        private readonly ITextView textView;
        private readonly int begin;
        private readonly int end;

        public UpdateSuggestion(int begin, int end, ITextView textView, ITextBuffer textBuffer, Guid codmanua)
        {
            this.textBuffer = textBuffer;
            this.textView = textView;
            this.codmanua = codmanua;
            this.begin = begin;
            this.end = end;
            this.display = string.Format("Update manual code.");
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
                ManuaCode bd = ManuaCode.GetManual(PackageOperations.Instance.GetActiveProfile(), codmanua);
                if (bd == null)
                    return;
                CommandHandler.CommandHandler handler = new CommandHandler.CommandHandler();
                handler.EditCodeSegment(textView.TextBuffer, begin, end, bd.Code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.UnableToExecuteOperation, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
