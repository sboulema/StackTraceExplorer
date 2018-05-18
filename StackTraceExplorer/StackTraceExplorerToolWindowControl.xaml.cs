using StackTraceExplorer.Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StackTraceExplorer
{
    public partial class StackTraceExplorerToolWindowControl
    {
        public StackTracesViewModel ViewModel { get; set; }

        public StackTraceExplorerToolWindowControl()
        {
            InitializeComponent();

            KeyDown += StackTraceExplorerToolWindowControl_KeyDown;

            ViewModel = new StackTracesViewModel();
            DataContext = ViewModel;

            if (!ViewModel.StackTraces.Any())
            {
                AddStackTrace();
            }
        }

        private void StackTraceExplorerToolWindowControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                AddStackTrace(Clipboard.GetText());
            }
            base.OnKeyDown(e);
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (StackTraceTabs.SelectedIndex >= 0)
            {
                ViewModel.StackTraces.RemoveAt(StackTraceTabs.SelectedIndex);
            }        

            if (!ViewModel.StackTraces.Any())
            {
                AddStackTrace();
            }
        }

        /// <summary>
        /// Add a tab to the toolwindow with the pasted stacktrace
        /// </summary>
        /// <param name="trace">stack trace</param>
        public void AddStackTrace(string trace = "")
        {
            ViewModel.AddStackTrace(trace);
            StackTraceTabs.SelectedIndex = StackTraceTabs.Items.Count - 1;
        }
    }
}