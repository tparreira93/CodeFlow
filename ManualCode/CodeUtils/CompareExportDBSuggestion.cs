using Microsoft.VisualStudio.Language.Intellisense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using System.Threading;
using System.Windows.Forms;

namespace CodeFlow.CodeUtils
{
    internal class CompareExportDBSuggestion : ISuggestedAction
    {
        private readonly IManual _manual;
        private readonly string _display;

        public CompareExportDBSuggestion(IManual manual)
        {
            _manual = manual;
            _display = string.Format("Merge and commit manual code.");
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
            IManual bd = null;
            if(_manual is ManuaCode)
            {
                try
                {
                    bd = ManuaCode.GetManual(PackageOperations.GetActiveProfile(), _manual.CodeId);
                    if (bd != null)
                    {
                        IManual result = Manual.Merge(_manual, bd);
                        if (MessageBox.Show(Properties.Resources.ExportedMerged, Properties.Resources.Export, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if(result.Update(PackageOperations.GetActiveProfile()))
                                MessageBox.Show(Properties.Resources.Submited, Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                            else
                                MessageBox.Show(Properties.Resources.NotSubmited
                                    + Environment.NewLine + Properties.Resources.VerifyProfile,
                                    Properties.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }
                    }
                }
                catch(Exception ex)
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
