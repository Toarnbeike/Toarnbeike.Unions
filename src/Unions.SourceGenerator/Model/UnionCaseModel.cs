using Microsoft.CodeAnalysis;
using Toarnbeike.Unions.Utilities;

namespace Toarnbeike.Unions.Model;

internal sealed record UnionCaseModel
{
    public int Index { get; } // 1-based index
    public string Name { get; }
    public string TypeName { get; }
    public string ArgumentName => Name.ToCamelCase();

    public string BackingTypeName => $"T{Index}";

    internal UnionCaseModel(INamedTypeSymbol symbol, int index)
    {
        Index = index;
        Name = symbol.Name;
        TypeName = symbol.Name;
    }
}