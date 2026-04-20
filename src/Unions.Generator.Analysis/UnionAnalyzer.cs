using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Toarnbeike.SourceGeneration.Semantic.Attributes;

namespace Toarnbeike.Unions.Generator.Analysis;

public static class UnionAnalyzer
{
    public static ImmutableArray<Diagnostic> Analyze(INamedTypeSymbol unionSymbol, INamedTypeSymbol unionCaseSymbol)
    {
        var diagnostics = new List<Diagnostic?>();

        var attributes = unionSymbol.GetAttributes(unionCaseSymbol).ToList();
        var caseTypes = attributes.Select(attr => attr.GetTypeArgument()).ToList();
        
        diagnostics.Add(CheckT001_TooFewCases(unionSymbol, attributes));
        diagnostics.AddRange(CheckT002_DuplicateCases(unionSymbol, caseTypes));
        diagnostics.AddRange(CheckT003_InvalidCaseType(unionSymbol, caseTypes));
        //diagnostics.Add(CheckT004_UnionMustBePartial(unionSymbol));
        diagnostics.AddRange(CheckT005_UnionCaseShouldBeRecord(unionSymbol, caseTypes));
        diagnostics.AddRange(CheckT006_UnionCaseMustBeConcrete(unionSymbol, caseTypes));
        diagnostics.AddRange(CheckT007_UnionCaseMustBeNonGeneric(unionSymbol, caseTypes));
        diagnostics.AddRange(CheckT008_UnionCaseMustBeNonNested(unionSymbol, caseTypes));

        return [..diagnostics.OfType<Diagnostic>()];

    }

    private static Diagnostic? CheckT001_TooFewCases(INamedTypeSymbol unionSymbol, List<AttributeData> cases)
    {
        return cases.Count >= 2 
            ? null 
            : Diagnostic.Create(Diagnostics.TooFewCases, unionSymbol.GetSymbolLocation(), unionSymbol.Name);
    }

    private static IEnumerable<Diagnostic> CheckT002_DuplicateCases(INamedTypeSymbol unionSymbol, List<INamedTypeSymbol?> caseTypes)
    {
        var duplicates = caseTypes
            .GroupBy(t => t, SymbolEqualityComparer.Default)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var dup in duplicates)
        {
                yield return Diagnostic.Create(
                    Diagnostics.DuplicateCase,
                    unionSymbol.GetSymbolLocation(),
                    dup.Key?.Name);
        }
    }

    private static IEnumerable<Diagnostic> CheckT003_InvalidCaseType(INamedTypeSymbol unionSymbol, List<INamedTypeSymbol?> caseTypes)
    {
        return caseTypes.Where(type => type is null || type.SpecialType is SpecialType.System_Object)
            .Select(type => Diagnostic.Create(
                Diagnostics.InvalidCaseType,
                unionSymbol.GetSymbolLocation(),
                type?.Name ?? "null")
            );
    }

    private static Diagnostic? CheckT004_UnionMustBePartial(INamedTypeSymbol unionSymbol)
    {
        var isPartial = unionSymbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .Any(c => c.Modifiers.Any(m => m.Text == "partial"));

        if (isPartial) return null;

        return Diagnostic.Create(
                Diagnostics.UnionMustBePartial,
                unionSymbol.GetSymbolLocation(),
                unionSymbol.Name);
        }
    
    private static IEnumerable<Diagnostic> CheckT005_UnionCaseShouldBeRecord(INamedTypeSymbol unionSymbol, List<INamedTypeSymbol?> caseTypes)
    {
        return caseTypes.Where(caseTypeSymbol => !caseTypeSymbol!.IsRecord).Select(caseTypeSymbol => 
            Diagnostic.Create(
                Diagnostics.UnionCaseMustBeRecord,
                unionSymbol.GetSymbolLocation(),
                caseTypeSymbol?.Name)
            );
    }

    private static IEnumerable<Diagnostic> CheckT006_UnionCaseMustBeConcrete(INamedTypeSymbol unionSymbol, List<INamedTypeSymbol?> caseTypes)
    {
        return 
            from caseTypeSymbol in caseTypes 
            where caseTypeSymbol!.IsAbstract || caseTypeSymbol.TypeKind == TypeKind.Interface 
            select Diagnostic.Create(
                Diagnostics.UnionCaseMustBeConcrete,
                unionSymbol.GetSymbolLocation(),
                caseTypeSymbol.Name);
    }

    private static IEnumerable<Diagnostic> CheckT007_UnionCaseMustBeNonGeneric(INamedTypeSymbol unionSymbol, List<INamedTypeSymbol?> caseTypes)
    {
        return caseTypes.Where(caseTypeSymbol => caseTypeSymbol!.IsGenericType)
            .Select(caseTypeSymbol => Diagnostic.Create(
                Diagnostics.UnionCaseMustBeNonGeneric,
                unionSymbol.GetSymbolLocation(),
                caseTypeSymbol?.Name)
            );
    }

    private static IEnumerable<Diagnostic> CheckT008_UnionCaseMustBeNonNested(INamedTypeSymbol unionSymbol, List<INamedTypeSymbol?> caseTypes)
    {
        return caseTypes.Where(caseTypeSymbol => caseTypeSymbol!.ContainingType is not null)
            .Select(caseTypeSymbol => Diagnostic.Create(
                Diagnostics.UnionCaseMustBeUnnested,
                unionSymbol.GetSymbolLocation(),
                caseTypeSymbol?.Name)
            );
    }

    private static Location GetSymbolLocation(this INamedTypeSymbol unionSymbol) =>
        unionSymbol.Locations.FirstOrDefault() ?? Location.None;
}
