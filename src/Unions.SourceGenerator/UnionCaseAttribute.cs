namespace Toarnbeike.Unions;

/// <summary>
/// Specifies a union case type for a class, indicating to the source generator to add the <paramref name="caseType"/> to the generated Union.
/// </summary>
/// <param name="caseType">The type that represents a specific case of the union. Cannot be null.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UnionCaseAttribute(Type caseType) : Attribute
{
    public Type CaseType { get; } = caseType;
}
