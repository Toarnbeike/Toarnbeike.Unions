using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.SourceGeneration.Rendering.Usings;
using Toarnbeike.SourceGeneration.Semantic.Attributes;
using Toarnbeike.Unions.SourceGenerator.Models;

namespace Toarnbeike.Unions.SourceGenerator.ModelFactories;

internal static class UnionModelFactory
{
    public static UnionModel Create(
        INamedTypeSymbol symbol,
        INamedTypeSymbol unionCaseAttributeSymbol)
    {
        var cases = GetUnionCases(symbol, unionCaseAttributeSymbol);
        var modelNamespace = symbol.ContainingNamespace.ToDisplayString();

        return new UnionModel(
            Namespace: modelNamespace,
            Name: symbol.Name,
            Cases: cases
                .Select((caseSymbol, index) =>
                    UnionCaseModelFactory.Create(caseSymbol, index + 1))
                .ToImmutableArray(),
            CaseUsings: UsingBuilder.FromTypes(cases, modelNamespace));
    }

    private static ImmutableArray<INamedTypeSymbol> GetUnionCases(
        INamedTypeSymbol unionType,
        INamedTypeSymbol unionCaseAttributeSymbol)
    {
        return unionType
            .GetAttributes(unionCaseAttributeSymbol)
            .Select(a => a.GetTypeArgument())
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();
    }
}