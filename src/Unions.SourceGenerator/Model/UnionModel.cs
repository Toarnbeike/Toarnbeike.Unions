using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.SourceGeneration.Attributes;
using Toarnbeike.SourceGeneration.Generation;
using Toarnbeike.SourceGeneration.Naming;
#pragma warning disable IDE0305 // Personal preference, prefer ToImmutableArray() over collection initialisation.

namespace Toarnbeike.Unions.SourceGenerator.Model;

internal sealed record UnionModel
{
    public string Namespace { get; }
    public string Name { get; }
    public string CaseUsings { get; }
    public ImmutableArray<UnionCaseModel> Cases { get; }

    public string BackingFieldName { get; } = "_value";

    public int Arity => Cases.Length;

    public string GenericParameterList => string.Join(", ", Cases.Select(c => c.Name));

    public string BackingUnionType => $"Union<{GenericParameterList}>";

    public string ArgumentName => Name.ToCamelCase();

    public IEnumerable<string> CaseNames => Cases.Select(c => c.Name);

    internal UnionModel(INamedTypeSymbol symbol, Compilation compilation)
    {
        var cases = GetUnionCases(symbol, compilation);

        Namespace = symbol.ContainingNamespace.ToDisplayString();
        Name = symbol.Name;
        CaseUsings = cases.CreateUsingStatements(exclude: Namespace);
        Cases = cases.Select((caseSymbol, index) => new UnionCaseModel(caseSymbol, compilation, ++index)).ToImmutableArray();
    }

    private static ImmutableArray<INamedTypeSymbol> GetUnionCases(INamedTypeSymbol unionType, Compilation compilation)
    {
        var unionCaseAttributeSymbol = compilation.GetTypeByMetadataName(typeof(UnionCaseAttribute).FullName!)!;

        return unionType
            .GetAttributes(unionCaseAttributeSymbol)
            .Select(a => a.GetConstructorTypeArgument())
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();
    }
}
