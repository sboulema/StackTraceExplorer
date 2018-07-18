namespace StackTraceExplorer
{
    using System.Runtime.InteropServices;
    using EnvDTE;
    using global::StackTraceExplorer.Helpers;
    using Microsoft.VisualStudio.Shell;

    [Guid("7648448a-48ab-4c10-968a-1b2ce0386050")]
    public class StackTraceExplorerToolWindow : ToolWindowPane
    {
        private const string _caption = "Stack Trace Explorer";
        private WindowEvents _windowEvents;

        public StackTraceExplorerToolWindow() : base(null)
        {
            Caption = _caption;
            Content = new StackTraceExplorerToolWindowControl();

            _windowEvents = EnvDteHelper.Dte.Events.WindowEvents;
            _windowEvents.WindowActivated += _windowEvents_WindowActivated;
        }

        private void _windowEvents_WindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.Caption.Equals(_caption))
            {
                var control = Content as StackTraceExplorerToolWindowControl;
                control.EnsureOneStackTrace();
            }
        }
    }
}
