using System;
using System.IO;
using System.Linq;
using EnvDTE;

namespace StackTraceExplorer
{
    public static class ClickHelper
    {
        public static bool HandleFileLinkClicked(string[] input)
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

        public static bool HandleFunctionLinkClicked(string[] input)
        {
            var projects = EnvDteHelper.Dte.Solution.Projects;
            CodeElement needle = null;

            foreach (Project project in projects)
            {
                if (project.CodeModel == null) continue;
                needle = Find(project.CodeModel.CodeElements, input[0]);              
            }

            if (needle == null) return false;

            EnvDteHelper.Dte.ExecuteCommand("File.OpenFile", needle.ProjectItem.FileNames[0]);
            (EnvDteHelper.Dte.ActiveDocument.Selection as TextSelection)?.GotoLine(needle.StartPoint.Line);

            return true;
        }

        private static CodeElement Find(CodeElements elements, string name)
        {
            foreach (CodeElement element in elements)
            {
                try
                {
                    if (element.FullName.Equals(name))
                    {
                        return element;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Log(e, element);
                    return null;
                }

                if (element.Name.Equals(name))
                {
                    return element;
                }
                if (element is CodeNamespace && (element as CodeNamespace).Members.Count > 0)
                {
                    var needle = Find((element as CodeNamespace).Members, name);
                    if (needle != null) return needle;
                }
                if (element is CodeClass && (element as CodeClass).Members.Count > 0)
                {
                    var needle = Find((element as CodeClass).Members, name);
                    if (needle != null) return needle;
                }
            }
            return null;
        }

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

        public static void TestStackTrace()
        {
            throw new Exception("Testing stack trace explorer");
        }
    }
}
