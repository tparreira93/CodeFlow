﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.Package;
using CodeFlowLibrary.Settings;
using CodeFlowUI.Controls.Editor;

namespace CodeFlow.Editor
{
    public class VisualStudioCodeEditor : ICodeEditor
    {
        private ICodeFlowPackage _package;
        public ICodeFlowPackage Package => _package;
        public VisualStudioCodeAdapter CodeAdapter { get; private set; }
        public Dictionary<string, string> LanguageFiles { get; private set; }

        public VisualStudioCodeEditor(ICodeFlowPackage package)
        {
            CodeAdapter = new VisualStudioCodeAdapter(package);
            LanguageFiles = new Dictionary<string, string>();
            _package = package;
        }

        public void Open(Profile profile, IManual code, SearchOptions options = null)
        {
            var extension = code.GetCodeExtension(profile);
            string filePath;

            if (LanguageFiles.ContainsKey(extension))
                filePath = LanguageFiles[extension];
            else
                filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.{extension}");

            File.WriteAllText(filePath, code.FormatCode(extension), Encoding.UTF8);

            CodeAdapter.Open(filePath, code);
        }

        public void Find(SearchOptions options)
        {
            
        }

        public string GetText()
        {
            return CodeAdapter.GetText();
        }

        public object GetUIControl(Profile profile, IManual code, SearchOptions options = null)
        {
            Border border = new Border();
            border.BorderBrush = (Brush)new SolidColorBrush(Color.FromRgb((byte)160, (byte)160, (byte)160));
            border.BorderThickness = new Thickness(1.0);

            UIElement element;
            if (profile == null || code == null || options == null)
                element = new AvalonCodeEditor(Package).Editor;
            else
            {
                Open(profile, code, options);
                element = CodeAdapter.GetUIControl();
            }
            border.Child = element;

            return border;
        }
    }
}