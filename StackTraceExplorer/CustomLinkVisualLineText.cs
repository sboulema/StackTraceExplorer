using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;

namespace StackTraceExplorer
{
    /// <summary>
    /// VisualLineElement that represents a piece of text and is a clickable link.
    /// </summary>
    public class CustomLinkVisualLineText : VisualLineText
    {
        private string[] Link { get; }
        public bool RequireControlModifierForClick { get; set; }
        public Brush ForegroundBrush { get; set; }
        public Func<string[], bool> ClickFunction { get; set; }

        /// <summary>
        /// Creates a visual line text element with the specified length.
        /// It uses the <see cref="ITextRunConstructionContext.VisualLine"/> and its
        /// <see cref="VisualLineElement.RelativeTextOffset"/> to find the actual text string.
        /// </summary>
        public CustomLinkVisualLineText(string[] theLink, VisualLine parentVisualLine, int length, 
            Brush foregroundBrush, Func<string[], bool> clickFunction, bool requireControlModifierForClick)
            : base(parentVisualLine, length)
        {
            RequireControlModifierForClick = requireControlModifierForClick;
            Link = theLink;
            ForegroundBrush = foregroundBrush;
            ClickFunction = clickFunction;         
        }


        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetForegroundBrush(ForegroundBrush);

            var lineNumber = EnvDteHelper.TextEditor.Document.GetLineByOffset(context.VisualLine.StartOffset).LineNumber;

            if (LinkIsClickable() && 
                EnvDteHelper.LineNumber == lineNumber &&
                EnvDteHelper.CurrentColumn >= RelativeTextOffset &&
                EnvDteHelper.CurrentColumn <= RelativeTextOffset + VisualLength)
            {
                TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            }

            return base.CreateTextRun(startVisualColumn, context);
        }

        private bool LinkIsClickable()
        {
            if (!Link.Any())
                return false;
            if (RequireControlModifierForClick)
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            return true;
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            if (!LinkIsClickable()) return;

            e.Handled = true;
            e.Cursor = Cursors.Hand;
                
            EnvDteHelper.SetCurrentMouseOffset(e);

            (e.Source as TextView).Redraw();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !e.Handled && LinkIsClickable())
            {
                ClickFunction(Link);
                e.Handled = true;
            }
        }

        protected override VisualLineText CreateInstance(int length)
        {
            var a = new CustomLinkVisualLineText(Link, ParentVisualLine, length, ForegroundBrush, ClickFunction, false);
            return a;
        }
    }
}
