using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Toarnbeike.Unions.SourceGenerator.Analyzers;

namespace Toarnbeike.Unions.Utilities;

internal static class AnalyzerTest
{
    private const string UnionCaseAttributeSource = """
        namespace Toarnbeike.Unions;

        [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
        public sealed class UnionCaseAttribute : System.Attribute
        {
            public UnionCaseAttribute(System.Type caseType) { }
        }
        """;

    public static async Task VerifyAsync(string source, params DiagnosticResult[] expectedDiagnostics)
    {
        var test = new CSharpAnalyzerTest<UnionDiagnosticAnalyzer, ShouldlyVerifier>
        {
            TestState =
            {
                Sources = { UnionCaseAttributeSource, source },
            },
        };

        test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);

        await test.RunAsync();
    }
}
