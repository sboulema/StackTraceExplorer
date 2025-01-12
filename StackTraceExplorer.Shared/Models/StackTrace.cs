using ICSharpCode.AvalonEdit.Document;
using Microsoft.VisualStudio.PlatformUI;
using StackTraceExplorer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StackTraceExplorer.Shared.Models
{
    public class StackTrace : ObservableObject
    {
        public TextDocument Document { get; set; }

        public List<CustomLinkVisualLineText> ClickedLines { get; set; } = new List<CustomLinkVisualLineText>();

        public StackTrace(string trace = null)
            => SetStackTrace(trace);

        private bool _wordWrap;

        public bool WordWrap
        {
            get => _wordWrap;
            set => SetProperty(ref _wordWrap, value);
        }

        public void SetStackTrace(string trace)
        {
            Document = new TextDocument { Text = WrapStackTrace(trace) };
            ClickHelper.ClearCache();
            NotifyPropertyChanged("Document");
        }

        public void AddClickedLine(CustomLinkVisualLineText line)
            => ClickedLines.Add(line);

        public bool IsClickedLine(CustomLinkVisualLineText line)
            => ClickedLines.Any(l => l.Link.SequenceEqual(line.Link));

        private string WrapStackTrace(string trace)
        {
            if (string.IsNullOrEmpty(trace))
            {
                return string.Empty;
            }

            if (trace.Contains(Environment.NewLine))
            {
                return trace;
            }

            var lines = Regex
                .Split(trace, @"(?=\s+(at|в)\s+)")
                .Where(line => !string.IsNullOrEmpty(line))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Where(line => line != "в");

            return string.Join(Environment.NewLine, lines);
        }
    }
}
