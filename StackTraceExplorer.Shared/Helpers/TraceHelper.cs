using System.Windows.Input;
using Microsoft.VisualStudio.ComponentModelHost;
using StackTraceExplorer.Models;
using TextEditor = ICSharpCode.AvalonEdit.TextEditor;

namespace StackTraceExplorer.Helpers
{
    public static class TraceHelper
    {
        public static int LineNumber;
        public static TextEditor TextEditor;
        public static int CurrentColumn;
        public static IComponentModel ComponentModel;
        public static StackTracesViewModel ViewModel;

        public static void SetCurrentMouseOffset(QueryCursorEventArgs e)
        {
            var pos = TextEditor.GetPositionFromPoint(e.GetPosition(TextEditor));

            if (pos == null)
            {
                return;
            }

            LineNumber = pos.Value.Line;
            CurrentColumn = pos.Value.Column;
        }
    }
}
