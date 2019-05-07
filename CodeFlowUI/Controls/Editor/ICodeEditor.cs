using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Package;
using CodeFlowLibrary.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowUI.Controls.Editor
{
    public interface ICodeEditor
    {
        ICodeFlowPackage Package { get; }
        string GetText();
        object GetUIControl(Profile profile = null, IManual code = null, SearchOptions options = null);
        void Find(SearchOptions options);
        void Open(Profile profile, IManual code, SearchOptions options = null);
    }
}
