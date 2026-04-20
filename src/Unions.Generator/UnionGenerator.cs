using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Toarnbeike.SourceGeneration.Selection;
using Toarnbeike.Unions.Generator.Analysis;
using Toarnbeike.Unions.SourceGenerator.Emitters;
using Toarnbeike.Unions.SourceGenerator.ModelFactories;
using Toarnbeike.Unions.SourceGenerator.Models;

namespace Toarnbeike.Unions.SourceGenerator;

[Generator]
public sealed class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var unionCaseAttribute = context.ForType<UnionCaseAttribute>();

        var analyzedUnions = context.ForAttribute<UnionCaseAttribute>()
            // todo: create overload of CombineWith that allows naming the tuple items.
            .CombineWith(unionCaseAttribute)
            .Select((tuple, _) =>
            {
                var union = tuple.Left;
                var attribute = tuple.Right;
                var analysis = UnionAnalyzer.Analyze(union, attribute!);
                return (union, attribute, analysis);
            });

        // todo: create tap extension on IncrementalValuesProvider in library.
        context.RegisterSourceOutput(analyzedUnions,
            static (spc, tuple) =>
            {
                foreach (var diagnostic in tuple.analysis)
                {
                    spc.ReportDiagnostic(diagnostic);
                }
            });

        var unionModels = analyzedUnions
            .Where(tuple => !tuple.analysis.Any())
            .Select((tuple, _) => UnionModelFactory.Create(tuple.union, tuple.attribute!))
            .WithComparer(UnionModelComparer.Instance);

        context.RegisterSourceOutput(
            unionModels,
            static (spc, model) => Execute(spc, model));
    }

    private static void Execute(SourceProductionContext context, UnionModel model)
    {
        context.AddSource($"{model.Name}_Union.g.cs", SourceText.From(UnionEmitter.Emit(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Match.g.cs", SourceText.From(MatchEmitter.Emit(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Switch.g.cs", SourceText.From(SwitchEmitter.Emit(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_TestExtensions.g.cs", SourceText.From(TestExtensionsEmitter.Emit(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Map.g.cs", SourceText.From(MapEmitter.Emit(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_Bind.g.cs", SourceText.From(BindEmitter.Emit(model), Encoding.UTF8));
        context.AddSource($"{model.Name}_CollectionExtensions.g.cs", SourceText.From(CollectionExtensionsEmitter.Emit(model), Encoding.UTF8));
    }
}