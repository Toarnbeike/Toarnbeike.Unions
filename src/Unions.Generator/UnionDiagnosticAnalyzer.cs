using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Toarnbeike.SourceGeneration.Semantic.Attributes;
using Toarnbeike.Unions.Generator.Analysis;

namespace Toarnbeike.Unions.SourceGenerator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionDiagnosticAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => Diagnostics.All;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedType) return;
        if (namedType.TypeKind != TypeKind.Class) return;

        var unionCaseAttribute = context.Compilation.GetTypeByMetadataName(typeof(UnionCaseAttribute).FullName!)!;

        if (!namedType.HasAttribute(unionCaseAttribute)) return;

        var diagnostics = UnionAnalyzer.Analyze(namedType, unionCaseAttribute);

        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }
}
