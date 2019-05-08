using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CodeFlowUI.Controls.Editor
{
    public interface ICodeEditorAdapter
    {
        ICodeFlowPackage Package { get; }

        bool PreProcessMessage(ref Message m);

        bool Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut, out int result);

        bool QueryStatus(ref Guid pguidCmdGroup, uint cCmds, Microsoft.VisualStudio.OLE.Interop.OLECMD[] prgCmds, IntPtr pCmdText, out int result);

        void Open(string fileName, IManual code);

        void Close();

        string GetText();

        IVsFindTarget GetFindTarget();

        IVsFindTarget2 GetFindTarget2();

        UIElement GetUIControl();
    }
}
