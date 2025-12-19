using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Toarnbeike.Unions.Utilities;

namespace Toarnbeike.Unions.Model;

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

    internal UnionModel(INamedTypeSymbol symbol)
    {
        var cases = GetUnionCases(symbol);
        
        Namespace = symbol.ContainingNamespace.ToDisplayString();
        Name = symbol.Name;
        CaseUsings = string.Join("\n", 
            cases.Select(c => c.ContainingNamespace.ToDisplayString())
                .Except([Namespace])
                .Distinct()
                .Select(ns => $"using {ns};"));
        Cases = [.. GetUnionCases(symbol).Select((caseSymbol, index) => new UnionCaseModel(caseSymbol, ++index))];
    }

    private static ImmutableArray<INamedTypeSymbol> GetUnionCases(INamedTypeSymbol unionType)
    {
        return [.. unionType
            .GetAttributes()
            .Where(a => a.AttributeClass?.Name is nameof(UnionCaseAttribute))
            .Select(a => a.ConstructorArguments[0].Value)
            .OfType<INamedTypeSymbol>()];
    }
}
