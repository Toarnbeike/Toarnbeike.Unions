namespace Toarnbeike.Unions.SourceGenerated.Complex;

public record Active(string Description);
public record Retry(int Attempt);
public record Aborted(DateTime CancellationDateTime);

