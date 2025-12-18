using Microsoft.CodeAnalysis;

namespace Toarnbeike.Unions;

internal static class Diagnostics
{
    private const string Category = "Toarnbeike.Unions";

    public static readonly DiagnosticDescriptor TooFewCases =
        new(
            id: "TU001",
            title: "A union must declare at least two cases",
            messageFormat: "Union '{0}' must declare at least two distinct cases",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "A union type must have at least two distinct cases."
        );

    public static readonly DiagnosticDescriptor DuplicateCase =
        new(
            id: "TU002",
            title: "Duplicate union case",
            messageFormat: "Union case '{0}' is specified more than once",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "A union case cannot be specified more than once."
        );

    public static readonly DiagnosticDescriptor InvalidCaseType =
        new(
            id: "TU003",
            title: "Invalid union case type",
            messageFormat: "Type '{0}' is not a valid union case",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The specified type is not valid as a union case."
        );

    public static readonly DiagnosticDescriptor UnionMustBePartial =
        new(
            id: "TU004",
            title: "Union must be partial",
            messageFormat: "Union declaration '{0}' must be declared partial",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Union declarations must be marked as partial."
        );

    public static readonly DiagnosticDescriptor UnionCaseMustBeRecord =
        new(
            id: "TU005",
            title: "Union case should be a record",
            messageFormat: "Union case '{0}' should be a record",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Union cases should be records for best compatibility."
        );

    public static readonly DiagnosticDescriptor UnionCaseMustBeConcrete =
        new(
            id: "TU006",
            title: "Union case must be concrete",
            messageFormat: "Union case '{0}' must be a concrete type, not an interface or abstract type",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Union cases must be concrete types."
        );

    public static readonly DiagnosticDescriptor UnionCaseMustBeNonGeneric =
        new(
            id: "TU007",
            title: "Union case must be non generic",
            messageFormat: "Union case '{0}' must be a non generic type",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Union cases must not be generic types."
        );

    public static readonly DiagnosticDescriptor UnionCaseMustBeUnnested =
        new(
            id: "TU008",
            title: "Union case must be non nested",
            messageFormat: "Union case '{0}' must be an unnested type",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Union cases must not be nested types."
        );
}

