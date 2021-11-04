using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using File = System.IO.File;
using Path = System.IO.Path;

namespace StackTraceExplorer.Helpers
{
    public static class ClickHelper
    {
        /// <summary>
        /// Handle click event on a file path in a stacktrace
        /// </summary>
        /// <param name="input">File path</param>
        /// <returns>File found</returns>
        public static void HandleFileLinkClicked(string[] input, StackTraceEditor stackTraceEditor)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var start = DateTime.UtcNow;
                OutputWindowPane outputWindow = await stackTraceEditor.EnsureOutputWindowPaneAsync();
                try
                {
                    var path = await Find(input[0], stackTraceEditor);

                    if (!File.Exists(path))
                    {
                        string message = $"FileLinkClicked: {input[0]}: Unable to resolve file.";
                        await VS.StatusBar.ShowMessageAsync(message);
                        await outputWindow.WriteLineAsync(message);
                        return;
                    }

                    var documentView = await VS.Documents.OpenAsync(path);

                    ITextSnapshotLine line = documentView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(int.Parse(input[1]) - 1);
                    documentView?.TextView?.ViewScroller.EnsureSpanVisible(line.Extent);
                    var selectionBroker = documentView.TextView?.GetMultiSelectionBroker();
                    if (selectionBroker != null)
                    {
                        selectionBroker.SetSelection(new Microsoft.VisualStudio.Text.Selection(line.Extent));
                    }

                    await outputWindow.WriteLineAsync($"FileLinkClicked: {input[0]} finished in {DateTime.UtcNow.Subtract(start)}");
                }
                catch (Exception e)
                {
                    await outputWindow.WriteLineAsync($"FileLinkClicked: {input[0]} error:\r\n{e}");
                }
            });
        }

        /// <summary>
        /// Handle click event on a member in a stacktrace
        /// </summary>
        /// <param name="input">Function name</param>
        public static void HandleMemberLinkClicked(string[] input, StackTraceEditor stackTraceEditor)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var start = DateTime.UtcNow;
                string typeOrMemberName = GetTypeOrMemberName(input);
                OutputWindowPane outputWindow = await stackTraceEditor.EnsureOutputWindowPaneAsync();
                try
                {
                    var member = SolutionHelper.Resolve(typeOrMemberName);

                    if (member == null)
                    {
                        string message = $"MemberLinkClicked: {typeOrMemberName}: unable to resolve member.";
                        await VS.StatusBar.ShowMessageAsync(message);
                        await outputWindow.WriteLineAsync(message);
                        return;
                    }

                    var location = member.Locations.FirstOrDefault();

                    var documentView = await VS.Documents.OpenAsync(location.SourceTree.FilePath);
                    var line = documentView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(location.GetLineSpan().StartLinePosition.Line);
                    documentView?.TextView?.ViewScroller.EnsureSpanVisible(line.Extent);
                    var selectionBroker = documentView.TextView?.GetMultiSelectionBroker();
                    if (selectionBroker != null)
                    {
                        var snapshotSpan = new SnapshotSpan(documentView.TextView.TextSnapshot, location.SourceSpan.Start, location.SourceSpan.Length);
                        selectionBroker.SetSelection(new Microsoft.VisualStudio.Text.Selection(snapshotSpan));
                    }

                    await outputWindow.WriteLineAsync($"MemberLinkClicked: {typeOrMemberName} finished in {DateTime.UtcNow.Subtract(start)}");
                }
                catch (Exception e)
                {
                    await outputWindow.WriteLineAsync($"MemberLinkClicked: {typeOrMemberName} error:\r\n{e}");
                }
            });
        }

        /// <summary>
        /// Given a path to a file, try to find a project item that closely matches the file path, 
        /// but is not an exact match
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> Find(string path, StackTraceEditor stackTraceEditor)
        {
            var outputWindow = await stackTraceEditor.EnsureOutputWindowPaneAsync();
            DateTime start = DateTime.UtcNow;
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    await outputWindow.WriteLineAsync($"FindFile: '{path}' is null or empty");
                    return string.Empty;
                }

                if (File.Exists(path))
                {
                    await outputWindow.WriteLineAsync($"FindFile: '{path}' File Exists");
                    return path;
                }

                string fileNameOnly = Path.GetFileName(path);

                var solution = await VS.Solutions.GetCurrentSolutionAsync();
                if (solution == null)
                {
                    return string.Empty;
                }

                var solutionDir = new DirectoryInfo(Path.GetDirectoryName(solution.FullPath));
                try
                {
                    await outputWindow.WriteLineAsync($"FindFile: '{path}' looking for '{fileNameOnly}' in '{solutionDir.FullName}'");
                    FileInfo[] fileInfos = solutionDir.GetFiles($"{fileNameOnly}", SearchOption.AllDirectories);
                    string[] files = fileInfos.Select(fi => fi.FullName).ToArray();
                    await outputWindow.WriteLineAsync($"FindFile: '{path}' found {files.Length} potential matches");
                    if (files.Length == 0)
                    {
                        return string.Empty;
                    }

                    string file = StringHelper.FindLongestMatchingSuffix(path, files, StringComparison.OrdinalIgnoreCase);

                    if (file != null)
                    {
                        await outputWindow.WriteLineAsync($"FindFile: '{path}' returning file: '{file}'");
                        return file;
                    }
                }
                catch (Exception e)
                {
                    await outputWindow.WriteLineAsync($"FindFile: '{path}' error:\r\n{e}");
                }

                await outputWindow.WriteLineAsync($"FindFile: '{path}' returning original path! Is this bad?'");
                return path;
            }
            finally
            {
                var elapsed = DateTime.UtcNow - start;
                await outputWindow.WriteLineAsync($"FindFile: '{path}' completed in {elapsed}");
            }
        }

        /// <summary>
        /// Construct the member identification
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetTypeOrMemberName(string[] input)
        {
            var needle = input[1].TrimEnd('.');
            var index = input[0].IndexOf(needle);

            var result = input[0].Substring(0, index + needle.Length);

            return result;
        }
    }
}
