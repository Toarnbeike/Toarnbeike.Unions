using System.Collections.Immutable;
using Toarnbeike.SourceGeneration.Models;
using Toarnbeike.SourceGeneration.Naming;

namespace Toarnbeike.SourceGeneration.Rendering;

public static class ConstructorParameterRenderingExtensions
{
    extension(ImmutableArray<ConstructorParameterModel> parameters)
    {
        public string GetXmlComments(string separator)
        {
            return string.Join(separator, parameters.Select(param =>
                $"""
                 /// <param name="{param.Name.ToCamelCase()}">{param.XmlDescription ?? $"{param.Name} ({param.TypeName})"}</param>
                 """
            ));
        }

        public string GetAsParameters() =>
            string.Join(", ", parameters.Select(param => $"{param.TypeName} {param.Name.ToCamelCase()}"));

        public string GetAsArguments() =>
            string.Join(", ", parameters.Select(param => param.Name.ToCamelCase()));
    }
}