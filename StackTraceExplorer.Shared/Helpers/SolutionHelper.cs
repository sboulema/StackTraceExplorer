using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackTraceExplorer.Helpers
{
    public static class SolutionHelper
    {
        public static Solution Solution;
        public static ImmutableArray<Compilation> Compilations;

        public static async Task<ImmutableArray<Compilation>> GetCompilationsAsync(Solution solution)
        {
            var compilationTasks = new Task<Compilation>[solution.ProjectIds.Count];
            for (var i = 0; i < compilationTasks.Length; i++)
            {
                var project = solution.GetProject(solution.ProjectIds[i]);
                compilationTasks[i] = project.GetCompilationAsync();
            }

            _ = await Task.WhenAll(compilationTasks);

            Compilations = compilationTasks.Select(t => t.Result).ToImmutableArray();

            return Compilations;
        }

        public static ISymbol Resolve(string methodName)
            => Compilations.Select(c => Resolve(c, methodName)).FirstOrDefault(s => s != null);

        private static ISymbol Resolve(Compilation compilation, string methodName)
        {
            var parts = methodName.ToString().Replace(".ctor", "#ctor").Split('.');

            var currentContainer = (INamespaceOrTypeSymbol)compilation.Assembly.Modules.Single().GlobalNamespace;

            for (var i = 0; currentContainer != null && i < parts.Length - 1; i++)
            {
                ParseTypeName(parts[i], out var typeOrNamespaceName, out var typeArity);
                currentContainer = currentContainer
                    .GetMembers(typeOrNamespaceName)
                    .Where(n => typeArity == 0 || n is INamedTypeSymbol t && t.Arity == typeArity)
                    .FirstOrDefault() as INamespaceOrTypeSymbol;
            }

            if (currentContainer == null)
            {
                return null;
            }

            var name = GetMemberName(parts.Last());
            var members = currentContainer.GetMembers(name);

            if (!members.Any())
            {
                return null;
            }

            foreach (var member in members)
            {
                if (member.Kind == SymbolKind.Method)
                {
                    var methodNameAndSignature = parts.Last();
                    var methodArity = GetMethodArity(methodNameAndSignature);
                    var parameterTypes = GetMethodParameterTypes(methodNameAndSignature);

                    var method = currentContainer
                        .GetMembers(name)
                        .OfType<IMethodSymbol>()
                        .Where(m => m.Arity == methodArity)
                        .FirstOrDefault(m => IsMatch(m, parameterTypes));

                    return method;
                }
                else
                {
                    return member;
                }
            }

            return null;
        }

        private static void ParseTypeName(string typeName, out string name, out int arity)
        {
            var backtick = typeName.IndexOf('`');

            if (backtick < 0)
            {
                name = typeName;
                arity = 0;
            }
            else
            {
                name = typeName.Substring(0, backtick);
                var arityText = typeName.Substring(backtick + 1);
                arity = int.Parse(arityText);
            }
        }

        private static string GetMemberName(string memberNameAndSignature)
        {
            var bracket = memberNameAndSignature.IndexOf('[');
            var parenthesis = memberNameAndSignature.IndexOf('(');

            if (parenthesis == -1)
            {
                return memberNameAndSignature;
            }

            var nameEnd = bracket >= 0 && bracket < parenthesis
                                ? bracket
                                : parenthesis;

            var result = memberNameAndSignature.Substring(0, nameEnd);

            if (result == "#ctor")
            {
                return ".ctor";
            }

            return result;
        }

        private static int GetMethodArity(string methodNameAndSignature)
        {
            var parenthesis = methodNameAndSignature.IndexOf('(');

            var openBracket = methodNameAndSignature.IndexOf('[', 0, parenthesis);

            if (openBracket < 0)
            {
                return 0;
            }

            var closeBracket = methodNameAndSignature.IndexOf(']', 0, parenthesis);

            if (closeBracket < 0)
            {
                return 0;
            }

            var result = 1;

            for (var i = openBracket; i <= closeBracket; i++)
            {
                if (methodNameAndSignature[i] == ',')
                {
                    result++;
                }
            }

            return result;
        }

        private static IReadOnlyList<string> GetMethodParameterTypes(string methodNameAndSignature)
        {
            var openParenthesis = methodNameAndSignature.IndexOf('(');
            var closeParenthesis = methodNameAndSignature.IndexOf(')');
            var signatureStart = openParenthesis + 1;
            var signatureLength = closeParenthesis - signatureStart;
            var signature = methodNameAndSignature.Substring(signatureStart, signatureLength);
            var parameters = signature.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = parameters[i].Trim();
            }

            var result = new List<string>(parameters.Length);

            foreach (var parameter in parameters)
            {
                var space = parameter.IndexOf(' ');
                var typeName = parameter.Substring(0, space);
                result.Add(typeName);
            }

            return result;
        }

        private static bool IsMatch(IMethodSymbol method, IReadOnlyList<string> parameterTypes)
        {
            if (method.Parameters.Length != parameterTypes.Count)
            {
                return false;
            }

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                var symbolTypeName = GetTypeName(method.Parameters[i]);
                var frameTypename = parameterTypes[i];

                if (symbolTypeName != frameTypename)
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetTypeName(IParameterSymbol symbol)
        {
            var sb = new StringBuilder();

            if (symbol.Type is IArrayTypeSymbol array)
            {
                sb.Append(array.ElementType.MetadataName);
                sb.Append("[]");
            }
            else if (symbol.Type is IPointerTypeSymbol pointer)
            {
                sb.Append(pointer.PointedAtType.MetadataName);
                sb.Append('*');
            }
            else
            {
                sb.Append(symbol.Type.MetadataName);
            }

            if (symbol.RefKind != RefKind.None)
            {
                sb.Append("&");
            }

            return sb.ToString();
        }
    }
}