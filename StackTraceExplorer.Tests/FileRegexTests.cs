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
        [DataRow(
            @"at CodeNav.Helpers.HistoryHelper.AddItemToHistory(CodeNav.VS2019, Version= 8.8.28.0, Culture= neutral, PublicKeyToken= null: D:\a\CodeNav\CodeNav\CodeNav.Shared\Helpers\HistoryHelper.cs:21)",
            @"D:\a\CodeNav\CodeNav\CodeNav.Shared\Helpers\HistoryHelper.cs:21",
            @"D:\a\CodeNav\CodeNav\CodeNav.Shared\Helpers\HistoryHelper.cs",
            "21")]
        [DataRow(
            @"at Dapper.SqlMapper+<ExecuteScalarImplAsync>d__69`1.MoveNext (Dapper, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null: /_/Dapper/SqlMapper.Async.cs:1241)",
            @"/_/Dapper/SqlMapper.Async.cs:1241",
            @"/_/Dapper/SqlMapper.Async.cs",
            "1241")]
        public void ShouldMatch(string input, string expectedPlace, string expectedFile, string expectedLine)
        {
            var match = FileLinkElementGenerator.FilePathRegex.Match(input);

            Assert.IsTrue(match.Success, "Match was not a success!");
            Assert.AreEqual(expectedPlace, match.Groups["place"].Value, nameof(expectedPlace));
            Assert.AreEqual(expectedFile, match.Groups["path"].Value, nameof(expectedFile));
            Assert.AreEqual(expectedLine, match.Groups["line"].Value, nameof(expectedLine));
        }
    }
}
