using Microsoft.CodeAnalysis;

namespace Toarnbeike.SourceGeneration.Generation;

/// <summary>
/// Generation methods to make creation of usings easier
/// </summary>
public static class UsingStatementExtensions
{

    public static string CreateUsingStatements(this IEnumerable<INamedTypeSymbol> symbols, params IEnumerable<string> exclude)
    {
        return string.Join("\n",
            symbols.Select(c => c.ContainingNamespace.ToDisplayString())
                .Except(exclude)
                .Distinct()
                .Select(ns => $"using {ns};"));
    }
}
