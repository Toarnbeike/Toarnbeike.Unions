using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.SourceGeneration.Attributes;
using Toarnbeike.Unions.SourceGenerator.Models;

namespace Toarnbeike.Unions.SourceGenerator.ModelFactories;

internal static class UnionModelFactory
{
    public static UnionModel Create(
        INamedTypeSymbol symbol,
        INamedTypeSymbol unionCaseAttributeSymbol)
    {
        var cases = GetUnionCases(symbol, unionCaseAttributeSymbol);

        return new UnionModel(
            Namespace: symbol.ContainingNamespace.ToDisplayString(),
            Name: symbol.Name,
            Cases: cases
                .Select((caseSymbol, index) =>
                    UnionCaseModelFactory.Create(caseSymbol, index + 1))
                .ToImmutableArray());
    }

    private static ImmutableArray<INamedTypeSymbol> GetUnionCases(
        INamedTypeSymbol unionType,
        INamedTypeSymbol unionCaseAttributeSymbol)
    {
        return unionType
            .GetAttributes(unionCaseAttributeSymbol)
            .Select(a => a.GetConstructorTypeArgument())
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();
    }
}