using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.CodeUtils
{
    internal class ExportToGenioSuggestion : ISuggestedAction
    {
        private readonly IManual _manual;
        private readonly string _display;

        public ExportToGenioSuggestion(IManual manual)
        {
            _manual = manual;
            _display = string.Format("Submit current manual code.");
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
                    if (MessageBox.Show(Properties.Resources.ConfirmExportDirect,
                        Properties.Resources.Export, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        if (_manual.Update(PackageOperations.GetActiveProfile()))
                            MessageBox.Show(Properties.Resources.Submited, Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                        else
                            MessageBox.Show(Properties.Resources.NotSubmited
                                + Environment.NewLine + Properties.Resources.VerifyProfile,
                                Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorUpdating, ex.Message),
                       Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
