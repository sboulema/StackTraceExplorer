namespace StackTraceExplorer.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StackTraceExplorer.Helpers;

    [TestClass]
    public class LongestSuffixTests
    {
        [DataTestMethod]
        [DataRow(
            @"D:\bldserver01\src\product\Project1\Sample.cs",
            new[] { @"C:\Sample.cs", @"C:\src\Project1\Sample.cs", @"D:\src\product\Project1\Sample.cs" },
            @"D:\src\product\Project1\Sample.cs")]
        [DataRow(
            @"D:\bldserver01\src\product\Project1\Sample.cs",
            new[] { @"C:\Sample.cs", @"C:\src\Project1\Sample.cs", @"C:\product\Project1\Sample.cs" },
            @"C:\product\Project1\Sample.cs")]
        [DataRow(
            @"D:\test\product\Project1\Sample.cs",
            new[] { @"C:\test\product\Project1\Sample.cs", @"D:\test\product\Project1\Sample.cs" },
            @"D:\test\product\Project1\Sample.cs")]
        public void LongestSuffixMatch(string path, string[] candidates, string expected)
        {
            string longestMatched = StringHelper.FindLongestMatchingSuffix(path, candidates, StringComparison.OrdinalIgnoreCase);
            Assert.AreEqual(expected, longestMatched);
        }

        [DataTestMethod]
        [DataRow(
            @"D:\src\Project1\DifferentFileName.cs",
            new[] { @"C:\DifferentFileName2.cs", @"C:\src\Project1\DifferentFileName2.cs", },
            "None of the candidates match")]
        [DataRow(
            @"C:\CandidateFileNameMatchingWithSuffix.cs",
            new[] { @"C:\OtherCandidateFileNameMatchingWithSuffix.cs", },
            "None of the candidates match")]
        public void LongestSuffixMatchNoMatches(string path, string[] candidates, string exceptionText)
        {
            var exception = Assert.ThrowsException<ArgumentException>(() => StringHelper.FindLongestMatchingSuffix(path, candidates, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(exception.Message.Contains(exceptionText), $"Exception.Message should contain '{exceptionText}' (actual:{exception.Message})");
        }
    }
}
