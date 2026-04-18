using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Toarnbeike.SourceGeneration.Attributes;

public static class IncrementalGeneratorSelectionExtensions
{
    /// <summary>
    /// Transform the GeneratorSyntaxContext to only allow types that are annotated with the specified attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute to check for.</typeparam>
    /// <param name="context">The GeneratorSyntaxContext</param>
    /// <returns>The <see cref="INamedTypeSymbol"/> of the annotated type.</returns>
    public static INamedTypeSymbol? TypesAnnotatedWith<TAttribute>(this GeneratorSyntaxContext context) where TAttribute : Attribute
    {
        var classSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classSyntax);

        if (symbol is not INamedTypeSymbol namedType)
        {
            return null;
        }

        return namedType.HasAttribute<TAttribute>(context.SemanticModel.Compilation) ? namedType : null;
    }

    /// <summary>
    /// Filters syntax nodes to only allow for classes annotated with at least one attribute.
    /// </summary>
    /// <param name="node">The syntax node to filter.</param>
    /// <returns>boolean indicating if the node is a class with at least one attribute.</returns>
    public static bool FilterClassWithAttribute(this SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }
}
