using StackTraceExplorer.Models;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            if (!ViewModel.StackTraces.Any())
            {
               AddStackTrace();
            }          

            DataContext = ViewModel;
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

        private void AddStackTrace(string trace = "")
        {
            ViewModel.AddStackTrace(trace);
            StackTraceTabs.SelectedIndex = StackTraceTabs.Items.Count - 1;
        }
    }
}