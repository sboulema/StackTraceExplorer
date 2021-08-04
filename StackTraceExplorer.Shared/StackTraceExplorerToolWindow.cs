using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace StackTraceExplorer
{
    public class StackTraceExplorerToolWindow : BaseToolWindow<StackTraceExplorerToolWindow>
    {
        private StackTraceExplorerToolWindowControl _control;

        public override string GetTitle(int toolWindowId) => "Stack Trace Explorer";

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            RegisterEvents();

            await Package.JoinableTaskFactory.SwitchToMainThreadAsync();

            _control = new StackTraceExplorerToolWindowControl();

            return _control;
        }

        [Guid("7648448a-48ab-4c10-968a-1b2ce0386050")]
        public class Pane : ToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.CallStackWindow;
            }
        }

        private void RegisterEvents()
        {
            VS.Events.WindowEvents.ActiveFrameChanged += WindowEvents_ActiveFrameChanged;
        }

        private void WindowEvents_ActiveFrameChanged(ActiveFrameChangeEventArgs obj)
        {
            if (obj.NewFrame.Caption.Equals("Stack Trace Explorer"))
            {
                _control.EnsureOneStackTrace();
            }
        }
    }
}
