using Toarnbeike.SourceGeneration.NetStandardCompatibility;
using Toarnbeike.Unions.SourceGenerator.Models;

namespace Toarnbeike.Unions.SourceGenerator.Rendering;

internal sealed class UnionModelComparer : IEqualityComparer<UnionModel>
{
    public static readonly UnionModelComparer Instance = new();

    public bool Equals(UnionModel? x, UnionModel? y)
        => x?.Name == y?.Name && x?.Namespace == y?.Namespace;

    public int GetHashCode(UnionModel obj)
        => HashCode.Combine(obj.Namespace, obj.Name);
}