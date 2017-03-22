using System.Windows;
using StackTraceExplorer.Generators;

namespace StackTraceExplorer
{
    public partial class StackTraceExplorerToolWindowControl
    {
        public StackTraceExplorerToolWindowControl()
        {
            InitializeComponent();

            TextEditor.TextArea.TextView.ElementGenerators.Add(new FileLinkElementGenerator());
            TextEditor.TextArea.TextView.ElementGenerators.Add(new MemberLinkElementGenerator());
        }

        private void ButtonWrap_OnClick(object sender, RoutedEventArgs e)
        {
            TextEditor.WordWrap = !TextEditor.WordWrap;
        }
    }
}