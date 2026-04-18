using Toarnbeike.SourceGeneration.Naming;

namespace Toarnbeike.SourceGeneration.Rendering;

/// <summary>
/// Generation methods to make rendering of using statements easier
/// </summary>
public static class UsingStatementRenderingExtensions
{
    public static string CreateUsingStatements(this IEnumerable<string> typeNames, params IEnumerable<string> exclude)
    {
        return string.Join("\n",
            typeNames.Select(TypeNameUtilities.GetNamespace)
                .Where(ns => ns is not null)
                .Except(exclude)
                .Distinct()
                .OrderBy(ns => ns) // ordering to make output stable (deterministic output)
                .Select(ns => $"using {ns};"));
    }
}