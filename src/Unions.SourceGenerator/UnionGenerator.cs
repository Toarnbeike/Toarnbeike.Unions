using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Toarnbeike.SourceGeneration.Attributes;
using Toarnbeike.Unions.SourceGenerator.ModelFactories;
using Toarnbeike.Unions.SourceGenerator.Models;
using Toarnbeike.Unions.SourceGenerator.Rendering;

namespace Toarnbeike.Unions.SourceGenerator;

[Generator]
public sealed class UnionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 0. Cache the [UnionCase] attribute
        var unionCaseAttribute = context.CompilationProvider
            .Select((compilation, _) =>
                compilation.GetTypeByMetadataName(typeof(UnionCaseAttribute).FullName!));

        // 1. Find classes with [UnionCase] attributes
        var candidateClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node.FilterClassWithAttribute(),
                static (ctx, _) => ctx.TypesAnnotatedWith<UnionCaseAttribute>())
            .Where(static c => c is not null);

        // 2. Combine and create model
        var unionModels = candidateClasses
            .Combine(unionCaseAttribute)
            .Select((tuple, _) =>
            {
                var (symbol, attributeSymbol) = tuple;

                return symbol is not { } type || attributeSymbol is null
                    ? null
                    : UnionModelFactory.Create(type, attributeSymbol);
            })
            .Where(m => m is not null)!
            .WithComparer(UnionModelComparer.Instance);

        // 3. Execute
        context.RegisterSourceOutput(
            unionModels,
            static (spc, model) => Execute(spc, model!));
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