namespace Toarnbeike.SourceGeneration.NetStandardCompatibility;

/// <summary>
/// Helper to combine two hashcodes.
/// Added because HashCode.Combine is not available for .netstandard2.0
/// </summary>
public static class HashCode
{
    public static int Combine<T1, T2>(T1 v1, T2 v2)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + (v1?.GetHashCode() ?? 0);
            hash = hash * 23 + (v2?.GetHashCode() ?? 0);
            return hash;
        }
    }
}
