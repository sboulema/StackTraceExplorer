using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
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
        public static bool HandleFileLinkClicked(string[] input)
        {
            try
            {
                var path = Find(input[0]).Result;

                if (!File.Exists(path))
                {
                    return true;
                }

                var documentView = VS.Documents.OpenAsync(path).Result;

                var line = documentView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(int.Parse(input[1]));
                documentView?.TextView?.ViewScroller.EnsureSpanVisible(line.Extent);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Handle click event on a member in a stacktrace
        /// </summary>
        /// <param name="input">Function name</param>
        /// <returns>Function found</returns>
        public static bool HandleMemberLinkClicked(string[] input)
        {
            try
            {
                var member = SolutionHelper.Resolve(GetInput(input));

                if (member == null)
                {
                    return false;
                }

                var location = member.Locations.FirstOrDefault();

                var documentView = VS.Documents.OpenAsync(location.SourceTree.FilePath).Result;

                var line = documentView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(location.GetLineSpan().StartLinePosition.Line + 1);
                documentView?.TextView?.ViewScroller.EnsureSpanVisible(line.Extent);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Given a path to a file, try to find a project item that closely matches the file path, 
        /// but is not an exact match
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<string> Find(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            if (File.Exists(path))
            {
                return path;
            }

            var pathParts = path.Split(new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);

            var solution = await VS.Solutions.GetCurrentSolutionAsync();
            var solutionDir = new DirectoryInfo(Path.GetDirectoryName(solution.FullPath));

            for (var i = 0; i < pathParts.Length; i++)
            {
                var partialPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts.Skip(i));

                try
                {
                    var file = solutionDir.GetFiles($"{partialPath}*", SearchOption.AllDirectories).FirstOrDefault();

                    if (file != null)
                    {
                        return file.FullName;
                    }
                }
                catch
                {
                }
            }

            return path;
        }

        /// <summary>
        /// Construct the member identification
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetInput(string[] input)
        {
            var needle = input[1].TrimEnd('.');
            var index = input[0].IndexOf(needle);

            var result = input[0].Substring(0, index + needle.Length);

            return result;
        }
    }
}
