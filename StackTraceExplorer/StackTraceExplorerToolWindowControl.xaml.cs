using Microsoft.VisualStudio.LanguageServices;
using StackTraceExplorer.Helpers;
using StackTraceExplorer.Models;
using System.IO;
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
            Drop += StackTraceExplorerToolWindowControl_Drop;

            ViewModel = new StackTracesViewModel();
            DataContext = ViewModel;

            EnsureOneStackTrace();
        }

        /// <summary>
        /// Always have one tab available in the toolwindow
        /// </summary>
        public void EnsureOneStackTrace()
        {
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

        /// <summary>
        /// Add a tab to the toolwindow after presenting an open file dialog
        /// </summary>
        public void AddStackTraceFromFile()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddStackTraceFromPath(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Add a tab to the toolwindow with file contents
        /// </summary>
        /// <param name="path">path to the file</param>
        public void AddStackTraceFromPath(string path)
        {
            if (File.Exists(path))
            {
                AddStackTrace(File.ReadAllText(path));
            }
        }

        #region Events
        private void ButtonPaste_OnClick(object sender, RoutedEventArgs e) => ViewModel.SetStackTrace(Clipboard.GetText());

        private void ButtonPasteAsNew_OnClick(object sender, RoutedEventArgs e) => AddStackTrace(Clipboard.GetText());

        private void ButtonOpenFile_OnClick(object sender, RoutedEventArgs e) => AddStackTraceFromFile();

        private async void TextEditor_TextChanged(object sender, System.EventArgs e)
        {
            var workspace = EnvDteHelper.ComponentModel.GetService<VisualStudioWorkspace>();
            SolutionHelper.Solution = workspace.CurrentSolution;
            await SolutionHelper.GetCompilationsAsync(workspace.CurrentSolution);
        }

        private void StackTraceExplorerToolWindowControl_Drop(object sender, DragEventArgs e)
        {
            var dropped = ((string[])e.Data.GetData(DataFormats.FileDrop));
            var files = dropped.ToList();

            if (!files.Any()) return;

            foreach (var file in files)
            {
                AddStackTraceFromPath(file);            
            }

            e.Handled = true;
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (StackTraceTabs.SelectedIndex >= 0)
            {
                ViewModel.StackTraces.RemoveAt(StackTraceTabs.SelectedIndex);
            }

            EnsureOneStackTrace();
        }

        private void StackTraceExplorerToolWindowControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                AddStackTrace(Clipboard.GetText());
            }
            base.OnKeyDown(e);
        }
        #endregion
    }
}