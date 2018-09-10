using CodeFlow.CodeControl;
using CodeFlow.ManualOperations;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeFlow.CodeControl.Analyzer;
using CodeFlow.GenioManual;
using CodeFlow.Forms;

namespace CodeFlow.CodeUtils.Suggestions
{
    internal class CommitSuggestion : ISuggestedAction
    {
        private readonly IManual _manual;
        private readonly string _display;

        public CommitSuggestion(IManual manual)
        {
            _manual = manual;
            _display = string.Format("Commit manual code");
        }

        public string DisplayText
        {
            get
            {
                return _display;
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
            if (_manual is ManuaCode)
            {
                try
                {
                    ChangeAnalyzer diffs = new ChangeAnalyzer();
                    diffs.CheckForDifferences(_manual, PackageOperations.Instance.GetActiveProfile());
                    CommitForm commitForm = new CommitForm(diffs);
                    commitForm.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.UnableToExecuteOperation, ex.Message),
                        Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            }
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
