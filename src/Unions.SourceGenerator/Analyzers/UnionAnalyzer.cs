using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Toarnbeike.Unions.SourceGenerator.Analyzers;

internal static class UnionAnalyzer
{
    public static bool ErrorsDetected;

    /// <summary>
    /// Analyse if the union definition is valid.
    /// Checks all rules defined in <see cref="Diagnostics"/>.
    /// Reports diagnostics to the provided <see cref="SourceProductionContext"/>.
    /// </summary>
    /// <param name="unionSymbol">The union symbol to validate.</param>
    /// <param name="context">The SourceProductionContext used to report diagnostics.</param>
    public static void ValidateUnion(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context)
    {
        ErrorsDetected = false;
        CheckT004_UnionMustBePartial(unionSymbol, context);

        var cases = unionSymbol.GetAttributes()
            .Where(a => a.AttributeClass?.Name == nameof(UnionCaseAttribute))
            .ToList();

        CheckT001_TooFewCases(unionSymbol, context, cases);

        var caseTypes = cases
            .Select(a => a.ConstructorArguments[0].Value as INamedTypeSymbol)
            .ToList();

        CheckT002_DuplicateCases(unionSymbol, context, caseTypes);
        CheckT003_InvalidCaseType(unionSymbol, context, caseTypes);

        if (ErrorsDetected) return;
        CheckT006_UnionCaseMustBeConcrete(unionSymbol, context, caseTypes);
        CheckT007_UnionCaseMustBeNonGeneric(unionSymbol, context, caseTypes);
        CheckT008_UnionCaseMustBeNonNested(unionSymbol, context, caseTypes);

        if (ErrorsDetected) return;
        CheckT005_UnionCaseShouldBeRecord(unionSymbol, context, caseTypes);
    }

    private static void CheckT001_TooFewCases(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<AttributeData> cases)
    {
        if (cases.Count < 2)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.TooFewCases,
                    unionSymbol.Locations.First(),
                    unionSymbol.Name));

            ErrorsDetected = true;
        }
    }

    private static void CheckT002_DuplicateCases(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<INamedTypeSymbol?> caseTypes)
    {
        var duplicates = caseTypes
            .GroupBy(t => t, SymbolEqualityComparer.Default)
            .Where(g => g.Count() > 1);

        foreach (var dup in duplicates)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.DuplicateCase,
                    unionSymbol.Locations.First(),
                    dup.Key?.Name));

            ErrorsDetected = true;
        }
    }

    private static void CheckT003_InvalidCaseType(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<INamedTypeSymbol?> caseTypes)
    {
        foreach (var type in caseTypes.Where(type => type is null || type.SpecialType is SpecialType.System_Object))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.InvalidCaseType,
                    unionSymbol.Locations.First(),
                    type?.Name ?? "null"));

            ErrorsDetected = true;
        }
    }

    private static void CheckT004_UnionMustBePartial(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context)
    {
        if (!unionSymbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .Any(c => c.Modifiers.Any(m => m.Text == "partial")))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.UnionMustBePartial,
                    unionSymbol.Locations.First(),
                    unionSymbol.Name));

            ErrorsDetected = true;
        }
    }

    private static void CheckT005_UnionCaseShouldBeRecord(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<INamedTypeSymbol?> caseTypes)
    {
        foreach (var caseTypeSymbol in caseTypes.Where(caseTypeSymbol => !caseTypeSymbol!.IsRecord))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.UnionCaseMustBeRecord,
                    unionSymbol.Locations.First(),
                    caseTypeSymbol?.Name));

            ErrorsDetected = true;
        }
    }

    private static void CheckT006_UnionCaseMustBeConcrete(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<INamedTypeSymbol?> caseTypes)
    {
        foreach (var caseTypeSymbol in caseTypes)
        {
            if (caseTypeSymbol!.IsAbstract || caseTypeSymbol.TypeKind == TypeKind.Interface)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Diagnostics.UnionCaseMustBeConcrete,
                        unionSymbol.Locations.First(),
                        caseTypeSymbol.Name));

                ErrorsDetected = true;
            }
        }
    }

    private static void CheckT007_UnionCaseMustBeNonGeneric(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<INamedTypeSymbol?> caseTypes)
    {
        foreach (var caseTypeSymbol in caseTypes.Where(caseTypeSymbol => caseTypeSymbol!.IsGenericType))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics.UnionCaseMustBeNonGeneric,
                    unionSymbol.Locations.First(),
                    caseTypeSymbol?.Name));

            ErrorsDetected = true;
        }
    }

    private static void CheckT008_UnionCaseMustBeNonNested(INamedTypeSymbol unionSymbol, SymbolAnalysisContext context, List<INamedTypeSymbol?> caseTypes)
    {
        foreach (var caseTypeSymbol in caseTypes)
        {
            if (caseTypeSymbol!.ContainingType is not null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        Diagnostics.UnionCaseMustBeUnnested,
                        unionSymbol.Locations.First(),
                        caseTypeSymbol.Name));

                ErrorsDetected = true;
            }
        }
    }
}
