using Microsoft.CodeAnalysis;
using Toarnbeike.SourceGeneration.Models.Factories;
using Toarnbeike.SourceGeneration.Semantic.Display;

namespace Toarnbeike.Unions.SourceGenerator.Models.Factories;

internal static class UnionCaseModelFactory
{
    public static UnionCaseModel Create(INamedTypeSymbol symbol, int index)
    {
        return new UnionCaseModel(
            Index: index,
            Name: symbol.Name,
            TypeName: symbol.ToDisplayString(SymbolDisplayFormats.FullyQualifiedType),
            ConstructorParameters: ConstructorParameterModelsFactory.Create(symbol));
    }
}