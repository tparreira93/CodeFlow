using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using CodeFlowLibrary.Bridge;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Package;
using CodeFlowLibrary.Settings;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;

namespace CodeFlowUI.Controls.Editor
{
    public class AvalonCodeEditor : ICodeEditor
    {
        public TextEditor Editor { get; private set; }
        public string SyntaxHighlighting { get; set; }
        public Dictionary<string, IHighlightingDefinition> HighlightDefinitions { get; private set; }
        public List<string> Highlighters { get; private set; }
        public SearchPanel EditorSearch { get; private set; }
        public ICodeFlowPackage Package { get; private set; }
        public ICodeEditorAdapter CodeAdapter => null;
        private UIElement element { get; set; }
        private IManual _current;

        public AvalonCodeEditor(ICodeFlowPackage package)
        {
            Editor = new TextEditor();
            Editor.FontSize = 12;
            Editor.FontFamily = new FontFamily("Consolas");
            Editor.ShowLineNumbers = true;
            EditorSearch = SearchPanel.Install(Editor.TextArea);
            _current = null;

            HighlightDefinitions = new Dictionary<string, IHighlightingDefinition>();
            Highlighters = new List<string>();
            foreach (var hl in HighlightingManager.Instance.HighlightingDefinitions)
            {
                HighlightDefinitions[hl.Name] = hl;
                Highlighters.Add(hl.Name);
            }
            Highlighters.Add("SQL");

            HighlightDefinitions["SQL"] = LoadHighlightingDefinition("sqlmode.xshd");
            Highlighters.Sort();
            Highlighters.Insert(0, "None");
            Package = package;
        }

        private IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        {
            var sql = CodeFlowResources.Resources.sqlmode;
            using (MemoryStream stream = new MemoryStream(sql, 0, sql.Length))
            using (var reader = new XmlTextReader(stream))
                return ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }

        public void Find(SearchOptions options)
        {
            var res = EditorSearch.FindName(options.SearchTerm);
            EditorSearch.Open();

            int nIndex = Editor.Text.IndexOf(options.SearchTerm, 0);
            if (nIndex != -1)
            {
                int line = Editor.TextArea.Document.GetLineByOffset(nIndex).LineNumber;
                Editor.Select(nIndex, options.SearchTerm.Length);
                EditorSearch.FindNext();
            }
        }

        public string GetText()
        {
            return Editor.Text;
        }

        public void Open(Profile profile, IManual code, SearchOptions options = null)
        {
            var typeConverter = new HighlightingDefinitionTypeConverter();
            string ext = code.GetCodeExtension(profile);
            var extensionList = profile.GenioConfiguration.Plataforms.SelectMany(x => x.TipoRotina.Select(t => t.ProgrammingLanguage)).Distinct();
            string lang = GetLanguage(ext);
            IHighlightingDefinition highlight = null;

            if (HighlightDefinitions.ContainsKey(lang))
                highlight = HighlightDefinitions[lang];
            else
                highlight = HighlightingManager.Instance.GetDefinitionByExtension("txt");

            Editor.SyntaxHighlighting = highlight;
            Editor.Document = new ICSharpCode.AvalonEdit.Document.TextDocument(code.FormatCode(ext));
        }

        public string GetLanguage(string extension)
        {
            switch (extension.ToUpper())
            {
                case "CS":
                    return "C#";
                case "CPP":
                    return "C++";
                default:
                    return extension.ToUpper();
            }
        }

        public object GetUIControl(Profile profile, IManual code, SearchOptions options = null)
        {
            if (element == null)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(160, 160, 160));
                border.BorderThickness = new Thickness(1.0);
                border.Child = Editor;

                element = border;
            }
            if (_current != code)
                Open(profile, code, options);

            return element;
        }
    }
}
