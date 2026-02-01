using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Toarnbeike.Unions.SourceGenerator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => Diagnostics.All;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(
            AnalyzeNamedType,
            SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        // Filter named types
        if (context.Symbol is not INamedTypeSymbol namedType) return;

        // Filter just classes
        if (namedType.TypeKind != TypeKind.Class) return;

        // Filter [UnionCase]
        if (!HasUnionCaseAttribute(namedType)) return;

        UnionAnalyzer.ValidateUnion(namedType, context);
    }

    private static bool HasUnionCaseAttribute(INamedTypeSymbol symbol)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.Name is nameof(UnionCaseAttribute)) return true;
        }

        return false;
    }
}
