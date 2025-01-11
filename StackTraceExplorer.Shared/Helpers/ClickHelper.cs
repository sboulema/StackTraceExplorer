using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using File = System.IO.File;
using Path = System.IO.Path;
using Task = System.Threading.Tasks.Task;

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
                var stopwatch = Stopwatch.StartNew();
                OutputWindowPane outputWindow = await stackTraceEditor.EnsureOutputWindowPaneAsync();
                try
                {
                    var path = await Find(input[0], outputWindow, stopwatch);

                    if (!File.Exists(path))
                    {
                        await WriteOutputAsync(outputWindow, $"FileLinkClicked: {input[0]}: Unable to resolve file ({stopwatch.Elapsed})", true);
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

                    await WriteOutputAsync(outputWindow, $"FileLinkClicked: {input[0]} finished ({stopwatch.Elapsed})", true);
                }
                catch (Exception e)
                {
                    await WriteOutputAsync(outputWindow, $"FileLinkClicked: {input[0]} error ({stopwatch.Elapsed}):\r\n{e}", true);
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
                var stopwatch = Stopwatch.StartNew();
                string typeOrMemberName = GetTypeOrMemberName(input);
                OutputWindowPane outputWindow = await stackTraceEditor.EnsureOutputWindowPaneAsync();
                try
                {
                    var member = SolutionHelper.Resolve(typeOrMemberName);
                    if (member == null)
                    {
                        await WriteOutputAsync(outputWindow, $"MemberLinkClicked: {typeOrMemberName}: unable to resolve member ({stopwatch.Elapsed})", true);
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

                    await WriteOutputAsync(outputWindow, $"MemberLinkClicked: {typeOrMemberName} finished ({stopwatch.Elapsed})", true);
                }
                catch (Exception e)
                {
                    await WriteOutputAsync(outputWindow, $"MemberLinkClicked: {typeOrMemberName} error:\r\n{e}", true);
                }
            });
        }

        static async Task WriteOutputAsync(OutputWindowPane outputWindow, string message, bool showInStatusBar = false)
        {
            await outputWindow.WriteLineAsync(message);
            if (showInStatusBar)
            {
                await VS.StatusBar.ShowMessageAsync(message);
            }
        }

        /// <summary>
        /// Given a path to a file, try to find a project item that closely matches the file path, 
        /// but is not an exact match
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> Find(string path, OutputWindowPane outputWindow, Stopwatch stopwatch)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            if (File.Exists(path))
            {
                await WriteOutputAsync(outputWindow, $"FindFile: '{path}': full path exists ({stopwatch.Elapsed})");
                return path;
            }

            string fileNameOnly = Path.GetFileName(path);

            var solution = await VS.Solutions.GetCurrentSolutionAsync();
            if (solution == null)
            {
                await WriteOutputAsync(outputWindow, $"FindFile: '{path}': No solution ({stopwatch.Elapsed})");
                return string.Empty;
            }

            var solutionDir = new DirectoryInfo(Path.GetDirectoryName(solution.FullPath)).Parent;
            try
            {
                await outputWindow.WriteLineAsync($"FindFile: '{path}' looking for '{fileNameOnly}' in '{solutionDir.FullName}'");
                FileInfo[] fileInfos = solutionDir.GetFiles($"{fileNameOnly}", SearchOption.AllDirectories);
                string[] files = fileInfos.Select(fi => fi.FullName).ToArray();
                await outputWindow.WriteLineAsync($"FindFile: '{path}' found {files.Length} potential matches ({stopwatch.Elapsed})");
                if (files.Length == 0)
                {
                    return string.Empty;
                }

                var candidates = new List<string>();
                candidates.AddRange(files.Select(f => f.Replace('\\', '/')));
                candidates.AddRange(files.Select(f => f.Replace('/', '\\')));

                string file = StringHelper.FindLongestMatchingSuffix(path, candidates.ToArray(), StringComparison.OrdinalIgnoreCase);

                if (file != null)
                {
                    await outputWindow.WriteLineAsync($"FindFile: Returning file: '{file}' ({stopwatch.Elapsed})");
                    return file;
                }
            }
            catch (Exception e)
            {
                await outputWindow.WriteLineAsync($"FindFile: '{path}' ({stopwatch.Elapsed}) error:\r\n{e}");
            }

            await outputWindow.WriteLineAsync($"FindFile: '{path}' returning original path! Is this bad? ({stopwatch.Elapsed})'");
            return path;
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
