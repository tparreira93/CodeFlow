using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Package;
using CodeFlowUI.Controls.Editor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace CodeFlow.Editor
{
    public class VisualStudioCodeAdapter : ICodeEditorAdapter
    {
        public ICodeFlowPackage Package { get; private set; }

        IComponentModel _componentModel;
        private IVsInvisibleEditor invisibleEditor;
        private IVsCodeWindow codeWindow;
        private IVsTextView textView;
        private IOleCommandTarget editorCommandTarget;
        private UIElement textViewHostControl;
        private IVsTextLines textLines;

        string fileName;

        public VisualStudioCodeAdapter(ICodeFlowPackage package)
        {
            Package = package;
        }


        private static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetOleServiceProvider()
        {
            ServiceProvider globalProvider = ServiceProvider.GlobalProvider;
            Guid guid = typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider).GUID;
            ref Guid local1 = ref guid;
            IntPtr pUnk = IntPtr.Zero;
            ref IntPtr local2 = ref pUnk;
            ((IObjectWithSite)globalProvider).GetSite(ref local1, out local2);
            try
            {
                if (pUnk != IntPtr.Zero)
                    return (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)Marshal.GetObjectForIUnknown(pUnk);
            }
            finally
            {
                if (pUnk != IntPtr.Zero)
                    Marshal.Release(pUnk);
            }
            return null;
        }


        private static void InitWindow(IVsCodeWindow codeWindow)
        {
            ((IVsCodeWindowEx)codeWindow).Initialize(2U, VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter, "", "", 0U, new INITVIEW[1]);
        }


        public void Open(string fileName, IManual code)
        {
            Utils.AsyncHelper.RunSyncUI(() =>
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var tmp = (IVsInvisibleEditorManager)Package.GetService(typeof(SVsInvisibleEditorManager));
                ErrorHandler.ThrowOnFailure(tmp.RegisterInvisibleEditor(fileName, null, 1U, null, out invisibleEditor));
                IntPtr docData = IntPtr.Zero;
                Guid guid = typeof(IVsTextLines).GUID;

                uint itemID;
                uint docCookie;
                IVsHierarchy hierarchy;
                var runningDocTable = (IVsRunningDocumentTable)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsRunningDocumentTable));

                ErrorHandler.ThrowOnFailure(runningDocTable.FindAndLockDocument(
                    dwRDTLockType: (uint)_VSRDTFLAGS.RDT_EditLock,
                    pszMkDocument: fileName,
                    ppHier: out hierarchy,
                    pitemid: out itemID,
                    ppunkDocData: out _,
                    pdwCookie: out docCookie));
                
                ErrorHandler.ThrowOnFailure(this.invisibleEditor.GetDocData(1, ref guid, out docData));

                try
                {
                    IVsTextLines objectForIunknown = (IVsTextLines)Marshal.GetObjectForIUnknown(docData);
                    IComponentModel componentModel = (IComponentModel)Package.GetService(typeof(SComponentModel));
                    IVsEditorAdaptersFactoryService adapterFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

                    codeWindow = adapterFactory.CreateVsCodeWindowAdapter(GetOleServiceProvider());
                    InitWindow(codeWindow);

                    this.textLines = objectForIunknown;

                    ErrorHandler.ThrowOnFailure(codeWindow.SetBuffer(textLines));
                    ErrorHandler.ThrowOnFailure(codeWindow.GetPrimaryView(out textView));

                    editorCommandTarget = textView as IOleCommandTarget;

                    IWpfTextViewHost wpfTextViewHost = adapterFactory.GetWpfTextViewHost(textView);

                    if (wpfTextViewHost.TextView.Options.IsOptionDefined("TextViewHost/FileHealthIndicator", false))
                        wpfTextViewHost.TextView.Options.SetOptionValue("TextViewHost/FileHealthIndicator", (object)false);

                    textViewHostControl = VS2019HostControl.GetTopParent((UIElement)wpfTextViewHost.HostControl);
                    BackgroundInvoke((Action)(() => VS2019HostControl.HideNavigationBar(this.textViewHostControl)));
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    if (docData != IntPtr.Zero)
                        Marshal.Release(docData);
                }
            });
        }

        private void BackgroundInvoke(Action action)
        {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, action);
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
        }

        public void Close()
        {
            if (codeWindow != null)
            {
                codeWindow.Close();
                codeWindow = null;
            }
            if (textView != null)
            {
                textView.CloseView();
                textView = null;
            }
        }

        public bool Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut, out int result)
        {
            if (textViewHostControl != null && textViewHostControl.IsKeyboardFocusWithin && this.editorCommandTarget != null)
            {
                result = editorCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                return true;
            }
            result = 0;
            return false;
        }

        public IVsFindTarget GetFindTarget()
        {
            return textView as IVsFindTarget;
        }

        public IVsFindTarget2 GetFindTarget2()
        {
            return textView as IVsFindTarget2;
        }

        public bool PreProcessMessage(ref Message m)
        {
            if (m.Msg < 256 || m.Msg > 265 || textViewHostControl == null || !textViewHostControl.IsKeyboardFocusWithin)
                return false;
            Guid pguidCmd;
            uint pdwCmd;
            int fCmdTranslated;
            int pfKeyComboStartsChord;
            var msg = new MSG[1] { new MSG() { hwnd = m.HWnd, lParam = m.LParam, wParam = m.WParam, message = (uint)m.Msg } };
            var service = (IVsFilterKeys2) Package.GetService(typeof(SVsFilterKeys));
            if (service.TranslateAcceleratorEx(msg, 4U, 0U, new Guid[0], out pguidCmd, out pdwCmd, out fCmdTranslated, out pfKeyComboStartsChord) != 0)
                return (uint)pfKeyComboStartsChord > 0U;
            return true;
        }

        public bool QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText, out int result)
        {
            if (textViewHostControl != null
                && textViewHostControl.IsKeyboardFocusWithin && editorCommandTarget != null)
            {
                result = editorCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
                return true;
            }
            result = 0;
            return false;
        }
        public UIElement GetUIControl()
        {
            return textViewHostControl;
        }

        public string GetText()
        {
            if (textView == null)
                return string.Empty;

            IVsTextLines ppBuffer;
            ErrorHandler.ThrowOnFailure(textView.GetBuffer(out ppBuffer));
            int piLine;
            int piIndex;
            ErrorHandler.ThrowOnFailure(ppBuffer.GetLastLineIndex(out piLine, out piIndex));
            string pbstrBuf;
            ppBuffer.GetLineText(0, 0, piLine, piIndex, out pbstrBuf);
            return pbstrBuf;
        }
    }
}
