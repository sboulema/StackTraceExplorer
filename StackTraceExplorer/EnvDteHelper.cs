using System.Windows.Input;
using EnvDTE;
using TextEditor = ICSharpCode.AvalonEdit.TextEditor;

namespace StackTraceExplorer
{
    public static class EnvDteHelper
    {
        public static DTE Dte;
        public static int LineNumber;
        public static TextEditor TextEditor;
        public static int CurrentMouseOffset;
        public static int CurrentColumn;

        public static void TestStackTrace()
        {
            ClickHelper.TestStackTrace();
        }

        public static void SetCurrentMouseOffset(QueryCursorEventArgs e)
        {
            var pos = TextEditor.GetPositionFromPoint(e.GetPosition(TextEditor));
            if (pos == null) return;

            LineNumber = pos.Value.Line;
            CurrentColumn = pos.Value.Column;
            CurrentMouseOffset = TextEditor.Document.GetOffset(LineNumber, CurrentColumn);
        }
    }
}
