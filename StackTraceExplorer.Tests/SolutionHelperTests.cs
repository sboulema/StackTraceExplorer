namespace StackTraceExplorer.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StackTraceExplorer.Helpers;

    [TestClass]
    public class SolutionHelperTests
    {
        public SolutionHelperTests()
        {
            this.Compilation = CreateCompilationForCurrentFile();
        }

        CSharpCompilation Compilation { get; }

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

        [DataTestMethod]
        [DataRow("StackTraceExplorer.Tests." + nameof(SolutionHelperTests), "StackTraceExplorer.Tests.SolutionHelperTests")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests..ctor()", "StackTraceExplorer.Tests.SolutionHelperTests.SolutionHelperTests()")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ResolveMember(String memberName, String expectedMatch)", "StackTraceExplorer.Tests.SolutionHelperTests.ResolveMember(string, string)")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ThisMethodThrows[T](String s)", "StackTraceExplorer.Tests.SolutionHelperTests.ThisMethodThrows<T>(string)")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ThisMethodThrows[T,V](String s)", "StackTraceExplorer.Tests.SolutionHelperTests.ThisMethodThrows<T, V>(string)")]
        [DataRow("StackTraceExplorer.Tests.ClassWithGenericTypeArgs`1", "StackTraceExplorer.Tests.ClassWithGenericTypeArgs<A>")]
        [DataRow("StackTraceExplorer.Tests." + nameof(ClassWithGenericTypeArgs<int, int>) + "`2", "StackTraceExplorer.Tests.ClassWithGenericTypeArgs<A, B>")]
        [DataRow("StackTraceExplorer.Tests.ClassWithGenericTypeArgs`1..ctor(Boolean throwException)", "StackTraceExplorer.Tests.ClassWithGenericTypeArgs<A>.ClassWithGenericTypeArgs(bool)")]
        [DataRow("StackTraceExplorer.Tests.ClassWithGenericTypeArgs`1.StaticMethod[C]()", "StackTraceExplorer.Tests.ClassWithGenericTypeArgs<A>.StaticMethod<C>()")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.<>c.<" + nameof(GenerateStackTracesForTesting) + ">b__6_3()", "StackTraceExplorer.Tests.SolutionHelperTests.GenerateStackTracesForTesting()")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.<>c__DisplayClass6_0.<" + nameof(GenerateStackTracesForTesting) + ">b__7(String s)", "StackTraceExplorer.Tests.SolutionHelperTests.GenerateStackTracesForTesting()")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.GetExceptionToString[T](Action`1 action, T value)", "StackTraceExplorer.Tests.SolutionHelperTests.GetExceptionToString<T>(Action<T>, T)")]
        public void ResolveMember(string memberName, string expectedMatch)
        {
            ISymbol symbol = SolutionHelper.Resolve(this.Compilation, memberName);
            Assert.IsNotNull(symbol, $"Symbol {memberName} should be found");
            Trace.WriteLine($"Found: {symbol}");
            Assert.AreEqual(expectedMatch, symbol.ToString(), "Resolved member was not correct");
        }

        [TestMethod]
        public void GenerateStackTracesForTesting()
        {
            // This isn't a test, per se, but it is useful for creating sample text to try pasting into the StackTrace window.
            Trace.WriteLine(GetExceptionToString(() => ClassWithGenericTypeArgs<string>.StaticMethod<int>()));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string>(throwException: true)));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string>(throwException: false).InstanceMethod<byte>()));

            Trace.WriteLine(GetExceptionToString(() => ClassWithGenericTypeArgs<string, object>.StaticMethod<int>()));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string, string>(throwException: true)));
            Trace.WriteLine(GetExceptionToString((s) => new ClassWithGenericTypeArgs<string, int>(throwException: false).InstanceMethod<int>(), "someString"));

            // Create some funny compiler generated names
            Trace.WriteLine(GetExceptionToString(() => throw new InvalidOperationException(this.ToString())));
            foreach (string value in new[] { "Test String" })
            {
                Trace.WriteLine(GetExceptionToString((s) => throw new InvalidOperationException(value), value));
            }
        }

        public static CSharpCompilation CreateCompilationForCurrentFile([CallerFilePath] string fileName = "")
        {
            var compilation = CSharpCompilation.Create("TempAssembly");
            string sourceText = File.ReadAllText(fileName);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceText);
            compilation = compilation.AddSyntaxTrees(tree);
            return compilation;
        }

        public static string GetExceptionToString(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return string.Empty;
        }

        public static string GetExceptionToString<T>(Action<T> action, T value)
        {
            try
            {
                action(value);
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return string.Empty;
        }

        // Used in Test DataRows
        static void ThisMethodThrows<T>(string s)
        {
            throw new NotImplementedException("foo");
        }

        // Used in Test DataRows
        static void ThisMethodThrows<T, V>(string s)
        {
            throw new NotImplementedException("foo");
        }
    }

    class ClassWithGenericTypeArgs<A>
    {
        public ClassWithGenericTypeArgs(bool throwException)
        {
            if (throwException)
            {
                throw new NotImplementedException();
            }
        }

        public string InstanceMethod<B>()
        {
            throw new NotImplementedException();
        }

        public static string StaticMethod<C>()
        {
            throw new NotImplementedException();
        }
    }

    class ClassWithGenericTypeArgs<A,B>
    {
        public ClassWithGenericTypeArgs(bool throwException)
        {
            if (throwException)
            {
                throw new NotImplementedException();
            }
        }

        public string InstanceMethod<C>()
        {
            throw new NotImplementedException();
        }

        public static string StaticMethod<D>()
        {
            throw new NotImplementedException();
        }
    }
}
