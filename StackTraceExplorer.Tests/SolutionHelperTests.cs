namespace StackTraceExplorer.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StackTraceExplorer.Helpers;

    [TestClass]
    public class SolutionHelperTests
    {
        [DataTestMethod]
        [DataRow("End[TAsyncResult](IAsyncResult result)", "End")]
        [DataRow("Main(String[] args)", "Main")]
        [DataRow("GenericType`1", "GenericType")]
        [DataRow("<GetSteps>d__0", "GetSteps")]
        [DataRow("get_TestProperty()", "get_TestProperty")]
        public void GetMemberName(string input, string expected)
        {
            string memberName = SolutionHelper.GetMemberName(input);
            Assert.AreEqual(expected, memberName);
        }
    }
}
