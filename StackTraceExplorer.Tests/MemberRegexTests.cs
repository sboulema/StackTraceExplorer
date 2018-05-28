using NUnit.Framework;
using StackTraceExplorer.Generators;
using System.Linq;
using System.Text.RegularExpressions;

namespace StackTraceExplorer.Tests
{
    [TestFixture]
    public class MemberRegexTests
    {
        [TestCase("Bla.Program.Main(String[] args)", 
            "Bla.Program.Main(String[] args)", new string[]{ "Bla.", "Program." }, "Main(String[] args)")]
        [TestCase(@"at Bla.Program.InnerClass.InnerMain() in C:\repos\Bla.Program\InnerClass.cs:line 168",
            "Bla.Program.InnerClass.InnerMain()", new string[] { "Bla.", "Program.", "InnerClass." }, "InnerMain()")]
        [TestCase(@"at Bla.Program.ApplicationMdi.<Button_Click>b__59_0() in C:\Repos\Bla.Program\Views\ApplicationMdi.cs:line 375",
            "Bla.Program.ApplicationMdi.<Button_Click>b__59_0()", new string[] { "Bla.", "Program.", "ApplicationMdi." }, "<Button_Click>b__59_0()")]
        [TestCase(@"at Bla.Program.ApplicationMdi.<>c.<Button_Click>b__59_0() in C:\Repos\Bla.Program\Views\ApplicationMdi.cs:line 375",
            "Bla.Program.ApplicationMdi.<>c.<Button_Click>b__59_0()", new string[] { "Bla.", "Program.", "ApplicationMdi.", "<>c." }, "<Button_Click>b__59_0()")]
        public void ShouldMatch(string input, string expectedMatch, string[] expectedCaptures, string expectedMethod)
        {
            var match = MemberLinkElementGenerator.MemberRegex.Match(input);

            Assert.IsTrue(match.Success);
            Assert.AreEqual(expectedMatch, match.Value);

            var captures = match.Groups[1].Captures.Cast<Capture>().Select(c => c.Value).ToArray();
            Assert.AreEqual(expectedCaptures, captures);

            Assert.AreEqual(expectedMethod, match.Groups[2].Value);
        }
    }
}
