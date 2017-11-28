﻿using CodeFlow.CommandHandlers;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFlow.CodeUtils
{
    internal class ImportFromDBSuggestion : ISuggestedAction
    {
        private readonly Guid codmanua;
        private readonly string display;
        private readonly ITextBuffer textBuffer;
        private readonly ITextView textView;
        private readonly int begin;
        private readonly int end;

        public ImportFromDBSuggestion(int begin, int end, ITextView textView, ITextBuffer textBuffer, Guid codmanua)
        {
            this.textBuffer = textBuffer;
            this.textView = textView;
            this.codmanua = codmanua;
            this.begin = begin;
            this.end = end;
            this.display = string.Format("Import manual code from database.");
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
                ManuaCode bd = ManuaCode.GetManual(PackageOperations.GetActiveProfile(), codmanua);
                if (bd == null)
                    return;
                
                CommandHandler.EditCodeSegment(textView.TextBuffer, begin, end, bd.Code);
            }
            catch (Exception)
            {}
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
