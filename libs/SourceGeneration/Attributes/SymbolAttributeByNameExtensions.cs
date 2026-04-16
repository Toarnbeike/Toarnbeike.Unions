using Microsoft.CodeAnalysis;

namespace Toarnbeike.SourceGeneration.Attributes;

/// <summary>
/// Extensions on any symbol checking for attributes based on the name of the attribute.
/// </summary>
/// <remarks>
/// WARNING: This is a fallback method if the type of the attribute can not be provided.
/// String comparison is very slow compared to symbol comparison, so use type or <c>INamedTypeSymbol</c> when possible.
/// </remarks>
public static class SymbolAttributeByNameExtensions
{

    /// <param name="symbol">The symbol to check for an attribute</param>
    extension(ISymbol symbol)
    {
        /// <summary>
        /// Checks if the symbol has an attribute with the given name.
        /// The name can be either the simple name of the attribute (e.g., "MyAttribute") or the fully qualified name (e.g., "MyNamespace.MyAttribute").
        /// </summary>
        /// <param name="attributeName">The requested name of the attribute.</param>
        /// <remarks>
        /// WARNING: This is a fallback method if the type of the attribute can not be provided.
        /// String comparison is very slow compared to symbol comparison, so use type or <c>INamedTypeSymbol</c> when possible.
        /// </remarks>
        public bool HasAttribute(string attributeName) =>
            symbol.GetAttributes().Any(attr => MatchAttributeByName(attr, attributeName));

        /// <summary>
        /// Get the attribute data if the symbol has an attribute with the given name.
        /// The name can be either the simple name of the attribute (e.g., "MyAttribute") or the fully qualified name (e.g., "MyNamespace.MyAttribute").
        /// </summary>
        /// <param name="attributeName">The requested name of the attribute.</param>
        /// <remarks>
        /// WARNING: This is a fallback method if the type of the attribute can not be provided.
        /// String comparison is very slow compared to symbol comparison, so use type or <c>INamedTypeSymbol</c> when possible.
        /// </remarks>
        public AttributeData? GetAttribute(string attributeName) =>
            symbol.GetAttributes().FirstOrDefault(attr => MatchAttributeByName(attr, attributeName));

        /// <summary>
        /// Get all the attributes of the symbol that match any of the given names.
        /// The names can be either the simple name of the attribute (e.g., "MyAttribute") or the fully qualified name (e.g., "MyNamespace.MyAttribute").
        /// </summary>
        /// <param name="attributeNames">Collection of names to search for.</param>
        /// <remarks>
        /// WARNING: This is a fallback method if the type of the attribute can not be provided.
        /// String comparison is very slow compared to symbol comparison, so use type or <c>INamedTypeSymbol</c> when possible.
        /// </remarks>
        public IEnumerable<AttributeData> GetAttributes(params string[] attributeNames) =>
            symbol.GetAttributes().Where(attr => MatchAny(attr, attributeNames));
    }

    private static bool MatchAny(AttributeData attribute, string[] names) =>
        names.Any(name => MatchAttributeByName(attribute, name));

    private static bool MatchAttributeByName(AttributeData attribute, string attributeName)
    {
        var attrClass = attribute.AttributeClass;
        if (attrClass is null)
            return false;

        return attrClass.Name == attributeName ||
               attrClass.ToDisplayString() == attributeName;
    }
}