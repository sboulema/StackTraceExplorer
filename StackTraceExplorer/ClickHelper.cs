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
            EnvDteHelper.Dte.ExecuteCommand("File.OpenFile", File.Exists(input[0]) ? input[0] : Find(input[0]));

            (EnvDteHelper.Dte.ActiveDocument.Selection as TextSelection)?.GotoLine(int.Parse(input[1]));
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
                if (element.FullName.Equals(name))
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

        private static string Find(string path)
        {
            var filename = Path.GetFileName(path);
            var dir = Path.GetDirectoryName(path);
            var dirParts = dir.Split(Path.DirectorySeparatorChar);

            for (var i = 0; i < dirParts.Length; i++)
            {
                var partialPath = Path.Combine(string.Join(Path.DirectorySeparatorChar.ToString(), dirParts.Skip(i)), filename);
                var file = EnvDteHelper.Dte.Solution.FindProjectItem(partialPath);
                if (file != null)
                {
                    return file.FileNames[0];
                }
            }

            return filename;
        }

        public static void TestStackTrace()
        {
            throw new Exception("Testing stack trace explorer");
        }
    }
}
