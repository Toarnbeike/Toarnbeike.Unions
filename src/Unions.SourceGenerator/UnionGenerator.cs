using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Toarnbeike.Unions;

[Generator]
public sealed class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. Find classes with [UnionCase] attributes
        var candidateClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => IsCandidate(node),          // Filter all nodes to classes with attributes
                static (ctx, _) => GetSemanticTarget(ctx))      // Filter to those with [UnionCase]
            .Where(static c => c is not null);

        // 2. Combine with compilation
        var compilationAndClasses =
            context.CompilationProvider.Combine(candidateClasses.Collect());

        // 3. Execute
        context.RegisterSourceOutput(
            compilationAndClasses,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsCandidate(SyntaxNode node)
        => node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0;

    private static INamedTypeSymbol? GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax);
        var unionCaseAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(UnionCaseAttribute).FullName);

        if (symbol is not INamedTypeSymbol namedType)
        {
            return null;
        }

        foreach (var attribute in namedType.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, unionCaseAttributeSymbol))
            {
                return namedType;
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<INamedTypeSymbol?> classes, SourceProductionContext context)
    {
        foreach (var symbol in classes.Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>())
        {
            UnionAnalyzer.ValidateUnion(symbol, context);
            // Code generation volgt later
        }
    }
}
