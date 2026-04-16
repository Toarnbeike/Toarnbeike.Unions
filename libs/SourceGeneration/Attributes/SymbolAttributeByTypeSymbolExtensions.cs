using Microsoft.CodeAnalysis;

namespace Toarnbeike.SourceGeneration.Attributes;

/// <summary>
/// Extensions on any symbol checking for attributes based on a (cached) version of the attribute as <c>INamedTypeSymbol</c>.
/// </summary>
/// <remarks>
/// This is the most performant method of checking for attributes,
/// because symbol comparison is faster than string comparison, 
/// and by caching the attributeSymbol it can be queried more often without reevaluating the context.
/// </remarks>
public static class SymbolAttributeByTypeSymbolExtensions
{
    /// <param name="symbol">The symbol to check for an attribute</param>
    extension(ISymbol symbol)
    {
        /// <summary>
        /// Checks if the symbol has an attribute with the attribute symbol.
        /// </summary>
        /// <param name="attributeSymbol">The (cached) type symbol of the attribute.</param>
        public bool HasAttribute(INamedTypeSymbol attributeSymbol) =>
            symbol.GetAttributes().Any(attr =>
                SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol));

        /// <summary>
        /// Get the attribute data if the symbol has an attribute with the attribute symbol.
        /// </summary>
        /// <param name="attributeSymbol">The (cached) type symbol of the attribute.</param>
        public AttributeData? GetAttribute(INamedTypeSymbol attributeSymbol) =>
            symbol.GetAttributes().FirstOrDefault(attr =>
                SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol));

        /// <summary>
        /// Get all the attributes data for the attribute that match given the provided attribute symbol.
        /// </summary>
        /// <param name="attributeSymbol">The (cached) type symbol of the attribute.</param>
        public IEnumerable<AttributeData> GetAttributes(INamedTypeSymbol attributeSymbol) =>
            symbol.GetAttributes()
                .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol));

        /// <summary>
        /// Get all the attributes data for the attributes that match given the provided attribute symbol.
        /// </summary>
        /// <param name="attributeSymbols">The collection of (cached) type symbols of the attributes.</param>
        public IEnumerable<AttributeData> GetAttributes(params INamedTypeSymbol[] attributeSymbols)
        {
            var comparer = SymbolEqualityComparer.Default;

            return symbol.GetAttributes().Where(attr =>
                attributeSymbols.Any(target => comparer.Equals(attr.AttributeClass, target)));
        }
    }
}