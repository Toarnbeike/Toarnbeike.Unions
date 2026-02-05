using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Toarnbeike.Unions.SourceGenerator.Model;

namespace Toarnbeike.Unions.SourceGenerator.Generators;

[Generator]
public sealed class UnionSourceGenerator : IIncrementalGenerator
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
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };

    private static INamedTypeSymbol? GetSemanticTarget(GeneratorSyntaxContext context)
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax);
        var unionCaseAttributeSymbol = context.SemanticModel.Compilation.GetTypeByMetadataName(typeof(UnionCaseAttribute).FullName!);

        if (symbol is not INamedTypeSymbol namedType)
        {
            return null;
        }

        return Enumerable.Any(namedType.GetAttributes(), attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, unionCaseAttributeSymbol)) 
            ? namedType : null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<INamedTypeSymbol?> classes, SourceProductionContext context)
    {
        foreach (var symbol in classes.Distinct(SymbolEqualityComparer.Default).OfType<INamedTypeSymbol>())
        {
            var model = new UnionModel(symbol);
            context.AddSource($"{symbol.Name}_Union.g.cs", SourceText.From(CoreGenerator.Execute(model), Encoding.UTF8));
            context.AddSource($"{symbol.Name}_Match.g.cs", SourceText.From(MatchGenerator.Execute(model), Encoding.UTF8));
            context.AddSource($"{symbol.Name}_Switch.g.cs", SourceText.From(SwitchGenerator.Execute(model), Encoding.UTF8));
            context.AddSource($"{symbol.Name}_TestExtensions.g.cs", SourceText.From(TestExtensionsGenerator.Execute(model), Encoding.UTF8));
            context.AddSource($"{symbol.Name}_Map.g.cs", SourceText.From(MapGenerator.Execute(model), Encoding.UTF8));
            context.AddSource($"{symbol.Name}_Bind.g.cs", SourceText.From(BindGenerator.Execute(model), Encoding.UTF8));
        }
    }
}
