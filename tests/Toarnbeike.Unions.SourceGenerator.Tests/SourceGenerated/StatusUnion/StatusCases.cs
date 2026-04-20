namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public record Active(string Description);
public record Retry(int Attempt);
public record Aborted(DateTime CancellationDateTime);

