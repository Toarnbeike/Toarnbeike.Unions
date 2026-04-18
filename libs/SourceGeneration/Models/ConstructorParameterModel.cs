using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Toarnbeike.SourceGeneration.XmlDocs;

namespace Toarnbeike.SourceGeneration.Models;

public sealed record ConstructorParameterModel(string TypeName, string Name, string? XmlDescription);

public sealed class ConstructorParameterModelsFactory
{
    public static ImmutableArray<ConstructorParameterModel> Create(INamedTypeSymbol symbol)
    {
        var constructor = SelectConstructor(symbol);
        if (constructor is null)
        {
            return [];
        }

        var documentation = constructor.GetParameterDocumentation();

        return constructor.Parameters
            .Select(p => ConstructorParameterModelFactory
                .Create(p, documentation.TryGetValue(p.Name, out var docs) ? docs : null))
            .ToImmutableArray();
    }

    private static IMethodSymbol? SelectConstructor(INamedTypeSymbol symbol)
    {
        var constructors = symbol.InstanceConstructors
            .Where(c => c.DeclaredAccessibility == Accessibility.Public).ToImmutableArray();

        return constructors.FirstOrDefault(c => !c.IsImplicitlyDeclared && c.Parameters.Length > 0)
               ?? constructors.FirstOrDefault(c => c.Parameters.Length > 0);
    }
}

public sealed class ConstructorParameterModelFactory
{
    public static ConstructorParameterModel Create(IParameterSymbol parameterSymbol, string? docs)
    {
        var typeName = parameterSymbol.Type.ToDisplayString(SymbolDisplayFormats.FullyQualifiedType);
        return new ConstructorParameterModel(typeName, parameterSymbol.Name, docs);
    }
}