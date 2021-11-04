using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using ICSharpCode.AvalonEdit;
using StackTraceExplorer.Generators;

namespace StackTraceExplorer
{
    public class StackTraceEditor : TextEditor
    {
        OutputWindowPane outputWindowPane;

        public StackTraceEditor()
        {
            TextArea.TextView.ElementGenerators.Add(new FileLinkElementGenerator(this));
            TextArea.TextView.ElementGenerators.Add(new MemberLinkElementGenerator(this));
        }

        public async Task<OutputWindowPane> EnsureOutputWindowPaneAsync()
        {
            const string OutputWindowName = nameof(StackTraceExplorer);
            if (this.outputWindowPane == null)
            {
                this.outputWindowPane = await VS.Windows.CreateOutputWindowPaneAsync(OutputWindowName);
            }

            return this.outputWindowPane;
        }
    }
}
