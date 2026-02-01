using Toarnbeike.Unions.Generators;

const int MinArity = 2;
const int MaxArity = 15; // values above 15 are untested, increase at own risk.

Console.WriteLine("Welcome to the Static Union Generator");
Console.WriteLine($"Please indicate the maximum arity to generate (between {MinArity} and {MaxArity}):");

while (true)
{
    var input = Console.ReadLine();
    var isValid = int.TryParse(input, out var parsedArity) && parsedArity >= MinArity && parsedArity <= MaxArity;
    if (isValid)
    {
        return Generate(parsedArity);
    }
    Console.WriteLine($"Invalid input. Please enter a number between {MinArity} and {MaxArity}:");
}

static int Generate(int upperBound)
{
    var range = Enumerable.Range(2, upperBound - 1).ToArray(); // arities from 2 to upperBound inclusive
    var outputRoot = "../../../../Unions"; // location to write generated files

    Console.WriteLine("\nGenerating unions");
    var unions = UnionGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Union.cs"), unions);

    Console.WriteLine("Generating match extensions");
    var matches = MatchExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/MatchExtensions.cs"), matches);

    Console.WriteLine("Generating switch extensions");
    var switches = SwitchExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/SwitchExtensions.cs"), switches);

    Console.WriteLine("Generating map extensions");
    var mapFull = MapFullExtensionsGenerator.Generate(range);
    var mapPartial = MapPartialExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/MapExtensions.Full.cs"), mapFull);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/MapExtensions.Partial.cs"), mapPartial);

    Console.WriteLine("Generating bind extensions");
    var bindFull = BindFullExtensionsGenerator.Generate(range);
    var bindPartial = BindPartialExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/BindExtensions.Full.cs"), bindFull);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/BindExtensions.Partial.cs"), bindPartial);

    Console.WriteLine("Generating tap extensions");
    var taps = TapExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/TapExtensions.cs"), taps);

    Console.WriteLine("Generating partition extensions");
    var partitions = PartitionExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "Collections/PartitionExtensions.cs"), partitions);

    Console.WriteLine("Generating test extensions");
    var testHelpers = TestExtensionsGenerator.Generate(range);
    File.WriteAllText(Path.Join(outputRoot, "TestExtensions/TestExtensions.cs"), testHelpers);

    Console.WriteLine("\nGeneration completed successfully.");

    return 0;
}
