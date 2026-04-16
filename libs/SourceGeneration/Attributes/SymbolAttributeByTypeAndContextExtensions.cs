using Microsoft.CodeAnalysis;

namespace Toarnbeike.SourceGeneration.Attributes;

/// <summary>
/// Extensions on any symbol checking for attributes based on the attribute type and the Compilation.
/// </summary>
/// <remarks>
/// This is an okay performant method of checking for attributes,
/// because the symbol can be obtained from the Context and symbol comparison is faster than string comparison.
/// If the check is repeated for the same argument, cache the argument and use overloads requiring the <c>INamedTypeSymbol</c>.
/// </remarks>
public static class SymbolAttributeByTypeAndContextExtensions
{
    /// <param name="symbol">The symbol to check for an attribute</param>
    extension(ISymbol symbol)
    {
        /// <summary>
        /// Checks if the symbol has an attribute with the attribute symbol.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to check for.</typeparam>
        /// <param name="compilation">The source generator compilation context, to load the INamedTypeSymbol of the <typeparam name="TAttribute"/>.</param>
        ///<remarks>
        /// WARNING: Use the overload providing the <see cref="INamedTypeSymbol"/> of the attribute is this called is repeated.
        /// </remarks>
        public bool HasAttribute<TAttribute>(Compilation compilation)
            where TAttribute : Attribute
        {
            var attributeSymbol = GetAttributeSymbol<TAttribute>(compilation);
            return attributeSymbol is not null && symbol.HasAttribute(attributeSymbol);
        }

        /// <summary>
        /// Get the attribute data if the symbol has an attribute with the attribute symbol.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to check for.</typeparam>
        /// <param name="compilation">The source generator compilation context, to load the INamedTypeSymbol of the <typeparam name="TAttribute"/>.</param>
        ///<remarks>
        /// WARNING: Use the overload providing the <see cref="INamedTypeSymbol"/> of the attribute is this called is repeated.
        /// </remarks>
        public AttributeData? GetAttribute<TAttribute>(Compilation compilation)
            where TAttribute : Attribute
        {
            var attributeSymbol = GetAttributeSymbol<TAttribute>(compilation);
            return attributeSymbol is null ? null : symbol.GetAttribute(attributeSymbol);
        }
    }

    /// <summary>
    /// Get the attribute as <see cref="INamedTypeSymbol"/> from the compilation.
    /// </summary>
    private static INamedTypeSymbol? GetAttributeSymbol<TAttribute>(Compilation compilation)
        where TAttribute : Attribute =>
        compilation.GetTypeByMetadataName(typeof(TAttribute).FullName!);
}