using System.Collections.Immutable;

namespace Toarnbeike.Unions.SourceGenerator.Model;

internal sealed record ConstructorParameterModel(string TypeName, string Name, string? XmlDescription);

// todo: move somewhere generic; potentially a new library project for code generation utilities.
internal static class ConstructorParameterModelExtensions
{
    extension(ImmutableArray<ConstructorParameterModel> parameters)
    {
        public string GetXmlComments(string separator)
        {
            return string.Join(separator, parameters.Select(param =>
                $"""
                 /// <param name="{ToParameterName(param.Name)}">{param.XmlDescription ?? $"{param.Name} ({param.TypeName})"}</param>
                 """
            ));
        }

        public string GetAsParameters() => 
            string.Join(", ", parameters.Select(param => $"{param.TypeName} {ToParameterName(param.Name)}"));

        public string GetAsArguments() =>
            string.Join(", ", parameters.Select(param => ToParameterName(param.Name)));
    }
    private static string ToParameterName(string name)
        => char.ToLowerInvariant(name[0]) + name.Substring(1);
}