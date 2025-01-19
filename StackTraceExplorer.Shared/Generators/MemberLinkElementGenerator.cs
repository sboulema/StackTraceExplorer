using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using StackTraceExplorer.Helpers;
using System.Linq;

namespace StackTraceExplorer.Generators
{
    public class MemberLinkElementGenerator : VisualLineElementGenerator
    {
        // To use this class:
        // textEditor.TextArea.TextView.ElementGenerators.Add(new MemberLinkElementGenerator());

        private readonly StackTraceEditor _textEditor;
        private static readonly Regex MemberVisualStudioRegex = new Regex(@"(?<member>(?<namespace>[A-Za-z0-9<>_`+]+\.)*(?<method>(.ctor|[A-Za-z0-9<>_\[,\]|+])+\(.*?\)))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex MemberAppInsightsRegex = new Regex(@"(?<member>(?<namespace>[A-Za-z0-9<>_`+]+\.)*(?<method>(.ctor|[A-Za-z0-9<>_\[,\]|+])+)) \(.+:\d+\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex MemberDemystifiedRegex = new Regex(@"(async )?([A-Za-z0-9<>_`+]+ )(?<member>(?<namespace>[A-Za-z0-9<>_`+]+\.)*(?<method>(.ctor|[A-Za-z0-9<>_\[,\]|+])+\(.*?\))) ", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private string _fullMatchText;

        public MemberLinkElementGenerator(StackTraceEditor textEditor)
        {
            _textEditor = textEditor;
        }

        private Match FindMatch(int startOffset)
        {
            // fetch the end offset of the VisualLine being generated
            var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;
            var relevantText = document.GetText(startOffset, endOffset - startOffset);
            return FindMatch(relevantText);
        }

        public static Match FindMatch(string text)
        {
            var match = MemberDemystifiedRegex.Match(text);
            if (match.Success)
                return match;
            match = MemberAppInsightsRegex.Match(text);
            if (match.Success)
                return match;
            return MemberVisualStudioRegex.Match(text);
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
            if (!match.Success || match.Index != 0) return null;

            // The first match returns the full method definition
            if (string.IsNullOrEmpty(_fullMatchText))
            {
                _fullMatchText = match.Groups["member"].Value;
            }

            var captures = match.Groups["namespace"].Captures.Cast<Capture>().Select(c => c.Value).ToList();
            captures.Add(match.Groups["method"].Value);

            var firstCapture = captures[0];
            var lineElement = new CustomLinkVisualLineText(
                new [] { _fullMatchText, firstCapture }, 
                CurrentContext.VisualLine,
                firstCapture.TrimEnd('.').Length,
                ToBrush(EnvironmentColors.ControlLinkTextBrushKey), 
                ClickHelper.HandleMemberLinkClicked, 
                false,
                CurrentContext.Document,
                _textEditor
            );

            // If we have created elements for the entire definition, reset. 
            // So we can create elements for more definitions
            if (_fullMatchText.EndsWith(firstCapture))
            {
                _fullMatchText = null;
            }

            if (TraceHelper.ViewModel.IsClickedLine(lineElement))
            {
                lineElement.ForegroundBrush = ToBrush(EnvironmentColors.ControlLinkTextBrushKey);
            }

            return lineElement;
        }

        private static SolidColorBrush ToBrush(ThemeResourceKey key)
        {
            var color = VSColorTheme.GetThemedColor(key);
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}