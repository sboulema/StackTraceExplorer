using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using Community.VisualStudio.Toolkit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using StackTraceExplorer.Helpers;

namespace StackTraceExplorer
{
    /// <summary>
    /// VisualLineElement that represents a piece of text and is a clickable link.
    /// </summary>
    public class CustomLinkVisualLineText : VisualLineText
    {
        public string[] Link { get; }

        public bool RequireControlModifierForClick { get; set; }

        public Brush ForegroundBrush { get; set; }

        public Action<string[], StackTraceEditor> ClickFunction { get; set; }

        public TextDocument TextDocument { get; set; }

        public StackTraceEditor TextEditor { get; set; }

        OutputWindowPane OutputWindowPane { get; }

        /// <summary>
        /// Creates a visual line text element with the specified length.
        /// It uses the <see cref="ITextRunConstructionContext.VisualLine"/> and its
        /// <see cref="VisualLineElement.RelativeTextOffset"/> to find the actual text string.
        /// </summary>
        public CustomLinkVisualLineText(string[] theLink, VisualLine parentVisualLine, int length, 
            Brush foregroundBrush, Action<string[], StackTraceEditor> clickFunction, bool requireControlModifierForClick,
            TextDocument textDocument, StackTraceEditor textEditor)
            : base(parentVisualLine, length)
        {
            RequireControlModifierForClick = requireControlModifierForClick;
            Link = theLink;
            ForegroundBrush = foregroundBrush;
            ClickFunction = clickFunction;
            TextDocument = textDocument;
            TextEditor = textEditor;
        }

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetForegroundBrush(ForegroundBrush);

            var lineNumber = TextDocument.GetLineByOffset(context.VisualLine.StartOffset).LineNumber;

            if (LinkIsClickable() &&
                TraceHelper.LineNumber == lineNumber &&
                TraceHelper.CurrentColumn >= RelativeTextOffset &&
                TraceHelper.CurrentColumn <= RelativeTextOffset + VisualLength)
            {
                TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            }

            return base.CreateTextRun(startVisualColumn, context);
        }

        private bool LinkIsClickable()
        {
            if (!Link.Any())
            {
                return false;
            }

            return RequireControlModifierForClick
                ? (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                : true;
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            if (!LinkIsClickable())
            {
                return;
            }

            e.Handled = true;
            e.Cursor = Cursors.Hand;

            TraceHelper.TextEditor = TextEditor;
            TraceHelper.SetCurrentMouseOffset(e);

            (e.Source as TextView).Redraw();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !e.Handled && LinkIsClickable())
            {
                e.Handled = true;

                ClickFunction(Link, this.TextEditor);
                TraceHelper.ViewModel.AddClickedLine(this);

                (e.Source as TextView).Redraw();
            }
        }

        protected override VisualLineText CreateInstance(int length)
            => new CustomLinkVisualLineText(Link, ParentVisualLine, length,
                ForegroundBrush, ClickFunction, false, TextDocument, TextEditor);
    }
}
