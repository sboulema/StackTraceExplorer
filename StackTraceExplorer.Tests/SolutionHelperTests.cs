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
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests..ctor()")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ResolveMember(String memberName)")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ThisMethodThrows[T](String s)")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ThisMethodThrows[T,V](String s)")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ClassWithGenericTypeArgs`2")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ClassWithGenericTypeArgs`1..ctor(Boolean throwException)")]
        [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.ClassWithGenericTypeArgs`1.StaticMethod[C]()")]
        // This Doesn't work
        // [DataRow("StackTraceExplorer.Tests.SolutionHelperTests.<>c.<CreateSomeStackTraces>b__6_3()")]
        public void ResolveMember(string memberName)
        {
            ISymbol symbol = SolutionHelper.Resolve(this.Compilation, memberName);
            Assert.IsNotNull(symbol, $"Symbol {memberName} should be found");
        }

        [TestMethod]
        public void CreateSomeStackTraces()
        {
            Trace.WriteLine(GetExceptionToString(() => ClassWithGenericTypeArgs<string>.StaticMethod<int>()));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string>(throwException: true)));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string>(throwException: false).InstanceMethod<byte>()));

            Trace.WriteLine(GetExceptionToString(() => ClassWithGenericTypeArgs<string, object>.StaticMethod<int>()));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string, string>(throwException: true)));
            Trace.WriteLine(GetExceptionToString(() => new ClassWithGenericTypeArgs<string, int>(throwException: false).InstanceMethod<int>()));
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
}
