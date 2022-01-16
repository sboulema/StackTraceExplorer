using ICSharpCode.AvalonEdit.Document;
using Microsoft.VisualStudio.PlatformUI;
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

        public void SetStackTrace(string trace)
        {
            Document = new TextDocument { Text = FormatStackTrace(trace) };
            NotifyPropertyChanged("Document");
        }

        public void AddClickedLine(CustomLinkVisualLineText line)
            => ClickedLines.Add(line);

        public bool IsClickedLine(CustomLinkVisualLineText line)
            => ClickedLines.Any(l => l.Link.SequenceEqual(line.Link));

        private string FormatStackTrace(string trace)
        {
            if (string.IsNullOrEmpty(trace))
            {
                return string.Empty;
            }

            return trace
                .Replace("\r\n", "")
                .Replace(" at ", "\r\n   at ")
                .Replace(" --- End of inner exception stack trace --- ", "\r\n--- End of inner exception stack trace--- ");
        }
    }
}
