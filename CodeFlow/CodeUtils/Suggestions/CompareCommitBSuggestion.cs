using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using System.Threading;
using System.Windows.Forms;
using CodeFlow.CodeControl;
using CodeFlow.CodeControl.Analyzer;
using CodeFlow.GenioManual;
using CodeFlow.ManualOperations;
using Microsoft.VisualStudio.Language.Intellisense;

namespace CodeFlow.CodeUtils.Suggestions
{
    internal class CompareCommitBSuggestion : ISuggestedAction
    {
        private readonly IManual local;
        private readonly string _display;

        public CompareCommitBSuggestion(IManual manual)
        {
            local = manual;
            _display = string.Format("Merge and commit manual code");
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

            try
            {
                ChangeAnalyzer analyzer = new ChangeAnalyzer();
                analyzer.CheckForDifferences(local, PackageOperations.Instance.GetActiveProfile());
                if (analyzer.Modifications.AsList.Count == 1)
                {
                    IChange change = analyzer.Modifications.AsList[0];
                    change = change.Merge();
                    IOperation operation = change.GetOperation();
                    if(operation != null)
                    {
                        string message = Properties.Resources.ExportedMerged;
                        if (operation is DeleteOperation)
                        {
                            message = Properties.Resources.ConfirmDelete;
                        }
                        if (MessageBox.Show(message, Properties.Resources.Export,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (PackageOperations.Instance.ExecuteOperation(operation))
                                MessageBox.Show(Properties.Resources.Submited, Properties.Resources.Export,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                            else
                                MessageBox.Show(Properties.Resources.NotSubmited
                                    + Environment.NewLine + Properties.Resources.VerifyProfile,
                                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }

                    }
                }
                else
                    MessageBox.Show(Properties.Resources.NoChanges, Properties.Resources.Export, 
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorUpdating, ex.Message),
                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
