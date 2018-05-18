using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using ICSharpCode.AvalonEdit;
using StackTraceExplorer.Helpers;

namespace StackTraceExplorer.Generators
{
    public class MemberLinkElementGenerator : VisualLineElementGenerator
    {
        // To use this class:
        // textEditor.TextArea.TextView.ElementGenerators.Add(new MemberLinkElementGenerator());

        private readonly TextEditor _textEditor;
        private static readonly Regex MemberRegex = new Regex(@"(\S*\.\S+\s*\(.*\))", RegexOptions.IgnoreCase);

        public MemberLinkElementGenerator(TextEditor textEditor)
        {
            _textEditor = textEditor;
        }

        private Match FindMatch(int startOffset)
        {
            // fetch the end offset of the VisualLine being generated
            var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;
            var relevantText = document.GetText(startOffset, endOffset - startOffset);
            return MemberRegex.Match(relevantText);
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
            var m = FindMatch(offset);
            // check whether there's a match exactly at offset
            if (!m.Success || m.Index != 0) return null;
            return new CustomLinkVisualLineText(
                new [] { m.Groups[1].Value }, 
                CurrentContext.VisualLine, 
                m.Length,
                ToBrush(EnvironmentColors.StartPageTextControlLinkSelectedColorKey), 
                ClickHelper.HandleFunctionLinkClicked, 
                false,
                CurrentContext.Document,
                _textEditor
            );
        }

        private static SolidColorBrush ToBrush(ThemeResourceKey key)
        {
            var color = VSColorTheme.GetThemedColor(key);
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}