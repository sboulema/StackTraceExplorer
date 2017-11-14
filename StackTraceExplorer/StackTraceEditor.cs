using ICSharpCode.AvalonEdit;
using StackTraceExplorer.Generators;

namespace StackTraceExplorer
{
    public class StackTraceEditor : TextEditor
    {
        public StackTraceEditor()
        {
            TextArea.TextView.ElementGenerators.Add(new FileLinkElementGenerator(this));
            TextArea.TextView.ElementGenerators.Add(new MemberLinkElementGenerator(this));
        }
    }
}
