﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging.Interop;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.Language.Intellisense;
using CodeFlowLibrary.CodeControl.Operations;
using CodeFlowLibrary.Bridge;
using CodeFlowLibrary.CodeControl.Analyzer;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Changes;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.Package;

namespace CodeFlow.CodeUtils.Suggestions
{
    internal class CompareCommitBSuggestion : ISuggestedAction
    {
        private readonly CodeFlowPackage package;
        private readonly IManual local;
        private readonly string _display;
        private readonly Profile _profile;

        public CompareCommitBSuggestion(CodeFlowPackage package, IManual manual, Profile profile)
        {
            this.package = package;
            local = manual;
            _display = string.Format("Merge and commit manual code");
            _profile = profile;
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
                analyzer.CheckForDifferences(local, _profile);
                if (analyzer.Modifications.AsList.Count == 1)
                {
                    IChange change = analyzer.Modifications.AsList[0];
                    change = change.Merge();
                    IOperation operation = change.GetOperation();
                    if(operation != null)
                    {
                        string message = CodeFlowResources.Resources.ExportedMerged;
                        if (operation is DeleteOperation)
                        {
                            message = CodeFlowResources.Resources.ConfirmDelete;
                        }
                        if (MessageBox.Show(message, CodeFlowResources.Resources.Export,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (package.ExecuteOperation(operation))
                                MessageBox.Show(CodeFlowResources.Resources.Submited, CodeFlowResources.Resources.Export,
                                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                            else
                                MessageBox.Show(CodeFlowResources.Resources.NotSubmited
                                    + Environment.NewLine + CodeFlowResources.Resources.VerifyProfile,
                                    CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        }

                    }
                }
                else
                    MessageBox.Show(CodeFlowResources.Resources.NoChanges, CodeFlowResources.Resources.Export, 
                        MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format(CodeFlowResources.Resources.ErrorUpdating, ex.Message),
                    CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
