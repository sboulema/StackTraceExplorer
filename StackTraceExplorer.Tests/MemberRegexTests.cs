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
        [DataRow(
            @" at StackTraceExplorer.TestClass.End[T,V](String s) in D:\StackTraceExplorer\TestClass.cs:line 38",
            "StackTraceExplorer.TestClass.End[T,V](String s)",
            new[] { "StackTraceExplorer.", "TestClass." },
            "End[T,V](String s)")]
        [DataRow(
            "  at SolutionHelperTests.ClassWithGenericTypeArgs`2.StaticMethod[V]()",
            "SolutionHelperTests.ClassWithGenericTypeArgs`2.StaticMethod[V]()",
            new[] { "SolutionHelperTests.", "ClassWithGenericTypeArgs`2.", },
            "StaticMethod[V]()")]
        [DataRow(
            "  at StackTraceExplorer.Tests.SolutionHelperTests.<>c.<CreateSomeStackTraces>b__6_3() in D:\\SolutionHelperTests.cs:line 52",
            "StackTraceExplorer.Tests.SolutionHelperTests.<>c.<CreateSomeStackTraces>b__6_3()",
            new[] { "StackTraceExplorer.", "Tests.", "SolutionHelperTests.", "<>c." },
            "<CreateSomeStackTraces>b__6_3()")]
        [DataRow("at Sample.ClassWithGenericTypeArgs`1..ctor(Boolean throwException)",
            "Sample.ClassWithGenericTypeArgs`1..ctor(Boolean throwException)",
            new[] { "Sample.", "ClassWithGenericTypeArgs`1.", },
            ".ctor(Boolean throwException)")]
        [DataRow(@"StackTraceExplorer.Tests.SolutionHelperTests.ClassWithGenericTypeArgs`1.StaticMethod[C]() in D:\StackTraceExplorer.Tests\SolutionHelperTests.cs:line 114",
            "StackTraceExplorer.Tests.SolutionHelperTests.ClassWithGenericTypeArgs`1.StaticMethod[C]()",
            new[] { "StackTraceExplorer.", "Tests.", "SolutionHelperTests.", "ClassWithGenericTypeArgs`1." },
            "StaticMethod[C]()")]
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

        [DataTestMethod]
        [DataRow("whatever[0]")]
        [DataRow("Normal sentence (pretty much)")]
        [DataRow("Normal sentence [pretty much]")]
        public void ShouldNotMatch(string input)
        {
            var match = MemberLinkElementGenerator.MemberRegex.Match(input);
            Assert.IsFalse(match.Success, $"Input {input} should not match.");
        }
    }
}
