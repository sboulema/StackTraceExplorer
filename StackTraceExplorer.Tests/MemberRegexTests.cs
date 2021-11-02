using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackTraceExplorer.Generators;

namespace StackTraceExplorer.Tests
{
    [TestClass]
    public class MemberRegexTests
    {
        [DataTestMethod]
        [DataRow("Bla.Program.Main(String[] args)", "Bla.Program.Main(String[] args)", new [] { "Bla.", "Program." }, "Main(String[] args)")]
        [DataRow(
            @"at Bla.Program.InnerClass.InnerMain() in C:\repos\Bla.Program\InnerClass.cs:line 168",
            "Bla.Program.InnerClass.InnerMain()",
            new [] { "Bla.", "Program.", "InnerClass." },
            "InnerMain()")]
        [DataRow(
            @"at Bla.Program.ApplicationMdi.<Button_Click>b__59_0() in C:\Repos\Bla.Program\Views\ApplicationMdi.cs:line 375",
            "Bla.Program.ApplicationMdi.<Button_Click>b__59_0()",
            new [] { "Bla.", "Program.", "ApplicationMdi." },
            "<Button_Click>b__59_0()")]
        [DataRow(
            @"at Bla.Program.ApplicationMdi.<>c.<Button_Click>b__59_0() in C:\Repos\Bla.Program\Views\ApplicationMdi.cs:line 375",
            "Bla.Program.ApplicationMdi.<>c.<Button_Click>b__59_0()",
            new [] { "Bla.", "Program.", "ApplicationMdi.", "<>c." },
            "<Button_Click>b__59_0()")]
        [DataRow(
            "at Company.Common.AsyncResult.End(IAsyncResult result)",
            "Company.Common.AsyncResult.End(IAsyncResult result)",
            new[] { "Company.", "Common.", "AsyncResult." },
            "End(IAsyncResult result)")]
        [DataRow(
            "at Company.Common.AsyncResult.End[TAsyncResult](IAsyncResult result)",
            "Company.Common.AsyncResult.End[TAsyncResult](IAsyncResult result)",
            new[] { "Company.", "Common.", "AsyncResult." },
            "End[TAsyncResult](IAsyncResult result)")]
        [DataRow(
            "Company.SomeType`1.StepCallback(IAsyncResult result)",
            "Company.SomeType`1.StepCallback(IAsyncResult result)",
            new[] { "Company.", "SomeType`1." },
            "StepCallback(IAsyncResult result)")]
        public void ShouldMatch(string input, string expectedMatch, string[] expectedCaptures, string expectedMethod)
        {
            var match = MemberLinkElementGenerator.MemberRegex.Match(input);

            Assert.IsTrue(match.Success, "Match was not a success!");
            Assert.AreEqual(expectedMatch, match.Value, nameof(expectedMatch));

            var captures = match.Groups[1].Captures.Cast<Capture>().Select(c => c.Value).ToArray();
            Assert.AreEqual(expectedCaptures.Length, captures.Length, "captures.Length did not match expected");
            for (int i = 0; i < expectedCaptures.Length; i++)
            {
                Assert.AreEqual(expectedCaptures[i], captures[i], $"Capture at index {i} did not match");
            }

            Assert.AreEqual(expectedMethod, match.Groups[2].Value, nameof(expectedMethod));
        }
    }
}
