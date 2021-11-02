using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackTraceExplorer.Generators;

namespace StackTraceExplorer.Tests
{
    [TestClass]
    public class FileRegexTests
    {
        [DataTestMethod]
        [DataRow(
            @"at Bla.Program.InnerClass.InnerMain() in C:\repos\Bla.Program\InnerClass.cs:line 168",
            @"C:\repos\Bla.Program\InnerClass.cs:line 168",
            @"C:\repos\Bla.Program\InnerClass.cs",
            "168")]
        [DataRow(
            @"at Bla.Program.InnerClass.InnerMain() in D:\repos\Bla.Program\Dash-File.cs:line 3005",
            @"D:\repos\Bla.Program\Dash-File.cs:line 3005",
            @"D:\repos\Bla.Program\Dash-File.cs",
            "3005")]
        [DataRow(
            @"at Bla.Program.InnerClass.InnerMain() in C:\repos\Bla.Program\Dot.File.cs:line 3",
            @"C:\repos\Bla.Program\Dot.File.cs:line 3",
            @"C:\repos\Bla.Program\Dot.File.cs",
            "3")]
        [DataRow(
            @"at Program.ApplicationMdi.<Button_Click>b__59_0() in E:\Repos\Underscore_File.cs:line 375",
            @"E:\Repos\Underscore_File.cs:line 375",
            @"E:\Repos\Underscore_File.cs",
            "375")]
        public void ShouldMatch(string input, string expectedMatch, string expectedFile, string expectedLine)
        {
            var match = FileLinkElementGenerator.FilePathRegex.Match(input);

            Assert.IsTrue(match.Success, "Match was not a success!");
            Assert.AreEqual(expectedMatch, match.Value, nameof(expectedMatch));

            var captures = match.Groups[1].Captures.Cast<Capture>().Select(c => c.Value).ToArray();
            Assert.AreEqual(1, captures.Length, "captures.Length did not match expected");
            Assert.AreEqual(expectedFile, captures[0], nameof(expectedFile));

            Assert.AreEqual(expectedLine, match.Groups[2].Value, nameof(expectedLine));
        }
    }
}
