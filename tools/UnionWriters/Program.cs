using UnionWriters.Writers;

const int minArity = 2;
const int maxArity = 15; // values above 15 are untested, increase at own risk.

Console.WriteLine("Welcome to the Generic Union Writer");
Console.WriteLine($"Please indicate the maximum arity to write (between {minArity} and {maxArity}):");

while (true)
{
    if (int.TryParse(Console.ReadLine(), out var parsedArity) && parsedArity is >= minArity and <= maxArity)
    {
        return WriteFiles(parsedArity);
    }
    Console.WriteLine($"Invalid input. Please enter a number between {minArity} and {maxArity}:");
}

static int WriteFiles(int upperBound)
{
    var range = Enumerable.Range(2, upperBound - 1).ToArray(); // arities from 2 to upperBound inclusive
    var outputRoot = "../../../../../src/Unions"; // location to write generated files

    Console.WriteLine("Writing unions");
    var unions = UnionWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Union.cs"), unions);

    Console.WriteLine("Writing match extensions");
    var matches = MatchExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/MatchExtensions.cs"), matches);

    Console.WriteLine("Writing switch extensions");
    var switches = SwitchExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/SwitchExtensions.cs"), switches);

    Console.WriteLine("Writing map extensions");
    var mapFull = MapFullExtensionsWriter.WriteFile(range);
    var mapPartial = MapPartialExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/MapExtensions.Full.cs"), mapFull);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/MapExtensions.Partial.cs"), mapPartial);

    Console.WriteLine("Writing bind extensions");
    var binds = BindExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/BindExtensions.cs"), binds);

    Console.WriteLine("Writing tap extensions");
    var taps = TapExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Extensions/TapExtensions.cs"), taps);

    Console.WriteLine("Writing collection extensions");
    var collectionExtensions = CollectionExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "Collections/CollectionExtensions.cs"), collectionExtensions);

    Console.WriteLine("Writing test extensions");
    var testExtensions = TestExtensionsWriter.WriteFile(range);
    File.WriteAllText(Path.Join(outputRoot, "TestExtensions/TestExtensions.cs"), testExtensions);

    Console.WriteLine("Writing files succeeded.");

    return 0;
}
