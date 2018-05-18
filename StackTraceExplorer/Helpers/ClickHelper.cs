﻿using System;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.LanguageServices;

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

        /// <summary>
        /// Handle click event on a function in a stacktrace
        /// </summary>
        /// <param name="input">Function name</param>
        /// <returns>Function found</returns>
        public static bool HandleFunctionLinkClicked(string[] input)
        {
            try
            {
                var workspace = EnvDteHelper.ComponentModel.GetService<VisualStudioWorkspace>();
                SolutionHelper.Solution = workspace.CurrentSolution;
                var compilations = SolutionHelper.GetCompilationsAsync(workspace.CurrentSolution).Result;

                var method = SolutionHelper.Resolve(input.FirstOrDefault());

                if (method == null) return false;

                var location = method.Locations.FirstOrDefault();

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