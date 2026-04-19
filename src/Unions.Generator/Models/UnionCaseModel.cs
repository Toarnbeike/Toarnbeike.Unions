using System.Collections.Immutable;
using Toarnbeike.SourceGeneration.Models;
using Toarnbeike.SourceGeneration.Rendering;

namespace Toarnbeike.Unions.SourceGenerator.Models;

internal sealed record UnionCaseModel(
    int Index,
    string Name,
    string TypeName,
    ImmutableArray<ConstructorParameterModel> ConstructorParameters)
{
    public string ArgumentName => Name.ToCamelCase();

    public string BackingTypeName => $"T{Index}";
}