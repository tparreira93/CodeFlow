﻿using CodeFlow.Utils;
using CodeFlowLibrary.Bridge;
using CodeFlowLibrary.CodeControl.Analyzer;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowUI;
using CodeFlowUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeFlow.Handlers
{
    public class VsCommander
    {
        public static void Create(CodeFlowPackage package)
        {
#pragma warning disable VSTHRD110 // Observe result of async calls
            Utils.AsyncHelper.RunSyncUI(async delegate
#pragma warning restore VSTHRD110 // Observe result of async calls
            {
                try
                {
                    CommandHandler handler = new CommandHandler(package);
                    string code = await handler.GetCurrentSelectionAsync();

                    if (!string.IsNullOrEmpty(code))
                    {
                        Profile profile = package.Active;
                        ManuaCode man = new ManuaCode(code);
                        ManualMatch manualMatch = new ManualMatch();
                        manualMatch.FullFileName = await handler.GetActiveDocumentNameAsync();
                        man.LocalMatch = manualMatch;
                        CreateInGenioForm genioForm = new CreateInGenioForm(profile, man);
                        genioForm.ShowDialog();
                        if (genioForm.DialogResult == DialogResult.OK)
                            handler.InsertCreatedCode(man);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, ex.Message),
                        CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                }
            });
        }

        public static void Update(CodeFlowPackage package)
        {
            try
            {
                CommandHandler handler = new CommandHandler(package);
                if (!AsyncHelper.RunSync(() => handler.ImportAndEditCurrentTagAsync()))
                {
                    MessageBox.Show(CodeFlowResources.Resources.VerifyProfile, CodeFlowResources.Resources.Import, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, ex.Message),
                    CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public static void Commit(CodeFlowPackage package)
        {
            try
            {
                CommandHandler handler = new CommandHandler(package);
                List<IManual> manual = AsyncHelper.RunSync(() => handler.SearchForTagsAsync());
                Profile profile = package.Active;
                ChangeAnalyzer diffs = new ChangeAnalyzer();
                diffs.CheckForDifferences(manual, profile);
                CommitForm commitForm = new CommitForm(package, profile, diffs);
                CodeFlowUIManager.Open(commitForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, ex.Message),
                    CodeFlowResources.Resources.Export, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
        }

        public static void ViewVersions(CodeFlowPackage package)
        {
            CodeFlowChangesForm changesForm = new CodeFlowChangesForm(PackageBridge.Flow.PackageUpdates, PackageBridge.Flow.Settings.ToolVersion, PackageBridge.Flow.Settings.OldVersion);
            CodeFlowUIManager.Open(changesForm);
        }

        public static void ManageProfiles(CodeFlowPackage package)
        {
            ProfilesForm form = new ProfilesForm(package, package.Active);
            form.ShowDialog();
        }
    }
}