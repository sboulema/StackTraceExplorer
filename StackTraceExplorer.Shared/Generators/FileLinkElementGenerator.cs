﻿using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using StackTraceExplorer.Helpers;

namespace StackTraceExplorer.Generators
{
    public class FileLinkElementGenerator : VisualLineElementGenerator
    {
        // To use this class:
        // textEditor.TextArea.TextView.ElementGenerators.Add(new FileLinkElementGenerator());

        private readonly StackTraceEditor _textEditor;
        public static readonly Regex FilePathRegex = new Regex(@"((?:[A-Za-z]\:|\\|/)(?:[\\/a-zA-Z_\-\s0-9\.\(\)]+)+):(?:line|Zeile|строка|ligne)?\s?(\d+)", RegexOptions.IgnoreCase);

        public FileLinkElementGenerator(StackTraceEditor textEditor)
        {
            _textEditor = textEditor;
        }

        private Match FindMatch(int startOffset)
        {
            // fetch the end offset of the VisualLine being generated
            var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;
            var relevantText = document.GetText(startOffset, endOffset - startOffset);
            return FilePathRegex.Match(relevantText);
        }

        /// Gets the first offset >= startOffset where the generator wants to construct
        /// an element.
        /// Return -1 to signal no interest.
        public override int GetFirstInterestedOffset(int startOffset)
        {
            var m = FindMatch(startOffset);
            return m.Success ? startOffset + m.Index : -1;
        }

        /// Constructs an element at the specified offset.
        /// May return null if no element should be constructed.
        public override VisualLineElement ConstructElement(int offset)
        {
            var match = FindMatch(offset);

            // check whether there's a match exactly at offset
            if (!match.Success || match.Index != 0)
            {
                return null;
            }

            var line = new CustomLinkVisualLineText(
                new[] { match.Groups[1].Value, match.Groups[2].Value },
                CurrentContext.VisualLine,
                match.Groups[0].Length,
                ToBrush(EnvironmentColors.ControlLinkTextBrushKey),
                ClickHelper.HandleFileLinkClicked,
                false,
                CurrentContext.Document,
                _textEditor
            );

            if (TraceHelper.ViewModel.IsClickedLine(line))
            {
                line.ForegroundBrush = ToBrush(EnvironmentColors.ControlLinkTextBrushKey);
            }

            return line;
        }

        private static SolidColorBrush ToBrush(ThemeResourceKey key)
        {
            var color = VSColorTheme.GetThemedColor(key);
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}