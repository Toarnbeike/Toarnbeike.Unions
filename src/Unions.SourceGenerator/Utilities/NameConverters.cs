namespace Toarnbeike.Unions.SourceGenerator.Utilities;

internal static class NameConverters
{
    public static string ToCamelCase(this string name)
    {
        return string.IsNullOrEmpty(name) || char.IsLower(name[0])
            ? name
            : name.Length == 1 
                ? name.ToLower() 
                : char.ToLower(name[0]) + name.Substring(1);
    }
}
