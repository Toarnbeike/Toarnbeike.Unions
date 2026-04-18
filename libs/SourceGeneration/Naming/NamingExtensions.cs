using Microsoft.CodeAnalysis;

namespace Toarnbeike.SourceGeneration.Naming;

/// <summary>
/// Extensions on property symbols for naming conventions.
/// </summary>
public static class NamingExtensions
{
    extension(IPropertySymbol property)
    {
        /// <summary>
        /// Convert the property name to a parameter name in camelCase.
        /// </summary>
        /// <returns>The name of the property in camel case: e.g. converts FullName to fullName.</returns>
        public string ToParameterName() => property.Name.ToCamelCase();

        /// <summary>
        /// Convert the property name to a backing field name in camelCase with an underscore prefix.
        /// </summary>
        /// <returns>The name of the property in camel case, with an underscore prefix: e.g. converts FullName to _fullName.</returns>
        public string ToBackingFieldName() => "_" + property.Name.ToCamelCase();
    }

    public static string ToCamelCase(this string name)
    {
        return string.IsNullOrEmpty(name) || char.IsLower(name[0])
            ? name
            : name.Length == 1
                ? name.ToLower()
                : char.ToLower(name[0]) + name.Substring(1);
    }
}

public static class TypeNameUtilities
{
    /// <summary>
    /// Get the namespace part of a full type name.
    /// </summary>
    public static string? GetNamespace(string fullTypeName)
    {
        if (string.IsNullOrEmpty(fullTypeName))
            return null;

        var lastDot = fullTypeName.LastIndexOf('.');
        return lastDot > 0
            ? fullTypeName.Substring(0, lastDot)
            : null;
    }
}