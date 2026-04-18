using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Xml.Linq;

namespace Toarnbeike.SourceGeneration.XmlDocs;

public static class XmlDocumentationExtensions
{
    public static IReadOnlyDictionary<string, string> GetParameterDocumentation(
        this IMethodSymbol method)
    {
        var xml = method.GetDocumentationCommentXml(expandIncludes: true);
        if (string.IsNullOrWhiteSpace(xml))
        {
            return ImmutableDictionary<string, string>.Empty;
        }

        try
        {
            var doc = XDocument.Parse(xml);

            return doc.Descendants("param")
                .Where(e => e.Attribute("name") is not null)
                .ToDictionary(
                    e => e.Attribute("name")!.Value,
                    e => Normalize(e.Value));
        }
        catch
        {
            return ImmutableDictionary<string, string>.Empty;
        }
    }

    private static string Normalize(string text) =>
        string.Join(" ",
            text.Split(['\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries));
}
