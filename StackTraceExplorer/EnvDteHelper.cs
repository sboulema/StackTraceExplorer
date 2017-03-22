using EnvDTE;
using ICSharpCode.AvalonEdit;
using TextEditor = ICSharpCode.AvalonEdit.TextEditor;

namespace StackTraceExplorer
{
    public static class EnvDteHelper
    {
        public static DTE Dte;
        public static int LineNumber;
    }
}
