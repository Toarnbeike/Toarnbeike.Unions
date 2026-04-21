using UnionTestWriters.Writers;

const int minArity = 2;
const int maxArity = 15; // values above 15 are untested, increase at own risk.

Console.WriteLine("Welcome to the Generic Union Test Writer");
Console.WriteLine($"Please indicate the maximum arity to write test for (between {minArity} and {maxArity}):");

while (true)
{
    if (int.TryParse(Console.ReadLine(), out var parsedArity) && parsedArity is >= minArity and <= maxArity)
    {
        return GenerateTests(parsedArity);
    }
    Console.WriteLine($"Invalid input. Please enter a number between {minArity} and {maxArity}:");
}

static int GenerateTests(int maxArity)
{
    var range = Enumerable.Range(2, maxArity - 1).ToArray();  // arities from 2 to upperBound inclusive
    var outputRoot = "../../../../../tests/Unions.Tests"; // location to write generated test files

    Console.WriteLine("\nGenerating union test types");
    var unionTestTypes = UnionTestTypeGenerator.Generate(maxArity);
    File.WriteAllText(Path.Join(outputRoot, "TestTypes/UnionTestTypes.cs"), unionTestTypes);


    foreach (var arity in range)
    {
        Console.WriteLine($"Write tests for arity {arity}");

        var unionTests = UnionTestWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Union{arity:D2}Tests.cs"), unionTests);

        var matchTests = MatchExtensionsTestsWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Extensions/Match/Match{arity:D2}Tests.cs"), matchTests);

        var switchTests = SwitchExtensionsTestsGenerator.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Extensions/Switch/Switch{arity:D2}Tests.cs"), switchTests);

        var mapFullTests = MapFullExtensionsTestsWriter.WriteFile(arity);
        var mapPartialTests = MapPartialExtensionsTestsWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Extensions/Map/Map{arity:D2}FullTests.cs"), mapFullTests);
        File.WriteAllText(Path.Join(outputRoot, $"Extensions/Map/Map{arity:D2}PartialTests.cs"), mapPartialTests);

        var bindPartialTests = BindExtensionsTestsWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Extensions/Bind/Bind{arity:D2}Tests.cs"), bindPartialTests);

        var tapTests = TapExtensionsTestsWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Extensions/Tap/Tap{arity:D2}Tests.cs"), tapTests);

        var collectionExtensionsTests = CollectionExtensionsTestsWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"Collections/CollectionExtensions{arity:D2}Tests.cs"), collectionExtensionsTests);

        var testExtensionsTests = TestExtensionsTestsWriter.WriteFile(arity);
        File.WriteAllText(Path.Join(outputRoot, $"TestExtensions/TestExtensions{arity:D2}Tests.cs"), testExtensionsTests);
    }

    Console.WriteLine("\nTest generation completed successfully.");

    return 0;
}
