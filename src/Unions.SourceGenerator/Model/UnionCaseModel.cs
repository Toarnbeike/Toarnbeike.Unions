using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Xml.Linq;
using Toarnbeike.Unions.SourceGenerator.Utilities;

namespace Toarnbeike.Unions.SourceGenerator.Model;

internal sealed record UnionCaseModel
{
    public int Index { get; } // 1-based index
    public string Name { get; }
    public string TypeName { get; }
    public ImmutableArray<ConstructorParameterModel> ConstructorParameters { get; }
    public string ArgumentName => Name.ToCamelCase();

    public string BackingTypeName => $"T{Index}";

    internal UnionCaseModel(INamedTypeSymbol symbol, int index)
    {
        Index = index;
        Name = symbol.Name;
        TypeName = symbol.Name;
        ConstructorParameters = GetConstructorParameters(symbol);
    }

    // todo: move somewhere generic; potentially a new library project for code generation utilities.
    private static ImmutableArray<ConstructorParameterModel> GetConstructorParameters(
        INamedTypeSymbol symbol)
    {
        var constructors = symbol.InstanceConstructors
            .Where(c => c.DeclaredAccessibility == Accessibility.Public)
            .ToImmutableArray();

        var ctor =
            constructors.FirstOrDefault(c => !c.IsImplicitlyDeclared && c.Parameters.Length > 0)
            ?? constructors.FirstOrDefault(c => c.Parameters.Length > 0);

        if (ctor is null)
            return ImmutableArray<ConstructorParameterModel>.Empty;

        var xml = ctor.GetDocumentationCommentXml(expandIncludes: true);
        var paramDocs = ParseParamDocumentation(xml);

        return [
            ..ctor.Parameters
                .Select(p =>
                    new ConstructorParameterModel(
                        TypeName: p.Type.ToDisplayString(TypeFormat),
                        Name: p.Name,
                        XmlDescription: paramDocs.TryGetValue(p.Name, out var doc) ? doc : null))
        ];
    }

    private static IReadOnlyDictionary<string, string> ParseParamDocumentation(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return ImmutableDictionary<string, string>.Empty;

        try
        {
            var doc = XDocument.Parse(xml);
            return doc.Descendants("param")
                .Where(e => e.Attribute("name") is not null)
                .ToDictionary(
                    e => e.Attribute("name")!.Value,
                    e => NormalizeWhitespace(e.Value));
        }
        catch
        {
            return ImmutableDictionary<string, string>.Empty;
        }
    }

    private static string NormalizeWhitespace(string text)
        => string.Join(" ",
            text.Split(['\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries));

    private static readonly SymbolDisplayFormat TypeFormat =
        new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions:
            SymbolDisplayMiscellaneousOptions.UseSpecialTypes |
            SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
        );
}