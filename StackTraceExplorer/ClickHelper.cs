using System;
using EnvDTE;

namespace StackTraceExplorer
{
    public static class ClickHelper
    {
        public static bool HandleFileLinkClicked(string[] input)
        {
            EnvDteHelper.Dte.ExecuteCommand("File.OpenFile", input[0]);
            (EnvDteHelper.Dte.ActiveDocument.Selection as TextSelection).GotoLine(int.Parse(input[1]));
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
            (EnvDteHelper.Dte.ActiveDocument.Selection as TextSelection).GotoLine(needle.StartPoint.Line);

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

        public static void TestStackTrace()
        {
            throw new Exception("Testing stack trace explorer");
        }
    }
}
