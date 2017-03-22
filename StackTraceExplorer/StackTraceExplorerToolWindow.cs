using System.Windows.Input;

namespace StackTraceExplorer
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    [Guid("7648448a-48ab-4c10-968a-1b2ce0386050")]
    public class StackTraceExplorerToolWindow : ToolWindowPane
    {
        public StackTraceExplorerToolWindow() : base(null)
        {
            Caption = "Stack Trace Explorer";
            Content = new StackTraceExplorerToolWindowControl { DataContext = this };
        }
    }
}
