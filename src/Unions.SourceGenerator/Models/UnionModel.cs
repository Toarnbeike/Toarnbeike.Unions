using System.Collections.Immutable;
using Toarnbeike.SourceGeneration.Naming;
using Toarnbeike.SourceGeneration.Rendering;

namespace Toarnbeike.Unions.SourceGenerator.Models;

internal sealed record UnionModel(
    string Namespace,
    string Name,
    ImmutableArray<UnionCaseModel> Cases)
{
    private string? _caseUsings;

    // BackingFieldName not static, is used as model.BackingFieldName, which is nicer than UnionModel.BackingFieldName
    public string BackingFieldName => "_value";

    public string GenericParameterList =>
        string.Join(", ", Cases.Select(c => c.Name));

    public string BackingUnionType =>
        $"Union<{GenericParameterList}>";

    public string ArgumentName => Name.ToCamelCase();

    public string CaseUsings => _caseUsings ??= CreateCaseUsings();

    private string CreateCaseUsings()
    {
        return Cases.Select(c => c.TypeName).CreateUsingStatements(Namespace);
    }
}
