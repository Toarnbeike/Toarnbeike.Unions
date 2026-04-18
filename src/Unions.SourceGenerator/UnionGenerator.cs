using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Toarnbeike.SourceGeneration.Selection;
using Toarnbeike.Unions.SourceGenerator.ModelFactories;
using Toarnbeike.Unions.SourceGenerator.Models;
using Toarnbeike.Unions.SourceGenerator.Rendering;

namespace Toarnbeike.Unions.SourceGenerator;

[Generator]
public sealed class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var unionCaseAttribute = context.ForType<UnionCaseAttribute>();

        var unionModels = context.ForAttribute<UnionCaseAttribute>()
            .CombineWith(unionCaseAttribute)
            .Select((tuple, _) => UnionModelFactory.Create(tuple.Left, tuple.Right!))
            .WithComparer(UnionModelComparer.Instance);

        context.RegisterSourceOutput(
            unionModels,
            static (spc, model) => Execute(spc, model));
    }

    private static void Execute(SourceProductionContext context, UnionModel model)
    {
        context.AddSource($"{model.Name}_Union.g.cs", SourceText.From(CoreGenerator.Execute(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Match.g.cs", SourceText.From(MatchGenerator.Execute(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Switch.g.cs", SourceText.From(SwitchGenerator.Execute(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_TestExtensions.g.cs", SourceText.From(TestExtensionsGenerator.Execute(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Map.g.cs", SourceText.From(MapGenerator.Execute(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Bind.g.cs", SourceText.From(BindGenerator.Execute(model), Encoding.UTF8));
    }
}