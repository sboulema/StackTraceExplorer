using System;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;

namespace StackTraceExplorer
{
    public static class ClickHelper
    {
        public static bool HandleFileLinkClicked(string[] input)
        {
            try
            {
                var path = Find(input[0]);
                if (File.Exists(path))
                {
                    EnvDteHelper.Dte.ExecuteCommand("File.OpenFile", path);

                    try
                    {
                        (EnvDteHelper.Dte.ActiveDocument?.Selection as TextSelection)?.GotoLine(int.Parse(input[1]));
                    }
                    catch (Exception)
                    {
                        // Cannot go to the requested line in the file
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                LogHelper.Log(e);
                return false;
            }
        }

        public static bool HandleFunctionLinkClicked(string[] input)
        {
            try
            {
                var workspace = MSBuildWorkspace.Create();
                var solutionFilePath = EnvDteHelper.Dte.Solution.FileName;

                if (!File.Exists(solutionFilePath)) return false;

                var solution = workspace.OpenSolutionAsync(solutionFilePath).Result;

                Location location = null;

                foreach (var project in solution.Projects)
                {
                    var symbols = SymbolFinder.FindDeclarationsAsync(project, input[0].Split('.').Last(), true).Result;

                    foreach (var symbol in symbols)
                    {
                        var fullName = symbol.ToDisplayString().Split('(')[0];
                        if (fullName.Equals(input[0]))
                        {
                            location = symbol.Locations.FirstOrDefault();
                            break;
                        }
                    }
                }

                if (location == null) return false;

                EnvDteHelper.Dte.ExecuteCommand("File.OpenFile", location.SourceTree.FilePath);
                (EnvDteHelper.Dte.ActiveDocument.Selection as TextSelection)?.GotoLine(location.GetLineSpan().StartLinePosition.Line + 1);

                return true;
            }
            catch (Exception e)
            {
                LogHelper.Log(e);
                return false;
            }
        }

        /// <summary>
        /// Given a path to a file, try to find a project item that closely matches the file path, 
        /// but is not an exact match
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Find(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            if (File.Exists(path)) return path;

            var pathParts = path.Split(Path.DirectorySeparatorChar);

            for (var i = 0; i < pathParts.Length; i++)
            {
                var partialPath = string.Join(Path.DirectorySeparatorChar.ToString(), pathParts.Skip(i));
                var file = EnvDteHelper.Dte.Solution.FindProjectItem(partialPath);
                if (file != null)
                {
                    return file.FileNames[0];
                }
            }

            return path;
        }
    }
}
