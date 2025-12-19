using Toarnbeike.Unions.SourceGenerated.Complex;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class MatchTests
{
    [Test]
    public void Status_Match_Should_Work_ForActive()
    {
        var status = Status.Active(new Active("Running"));

        var result = status.Match(
            active => active.Description,
            retry => $"Retry {retry.Attempt}",
            aborted => $"Aborted at {aborted.CancellationDateTime}");
        
        result.ShouldBe("Running");
    }

    [Test]
    public void Status_Match_Should_Work_ForRetry()
    {
        var status = Status.Retry(new Retry(3));

        var result = status.Match(
            active => active.Description,
            retry => $"Retry {retry.Attempt}",
            aborted => $"Aborted at {aborted.CancellationDateTime}");
        
        result.ShouldBe("Retry 3");
    }

    [Test]
    public void Status_Match_Should_Work_ForAborted()
    {
        var abortTime = DateTime.UtcNow;
        var status = Status.Aborted(new Aborted(abortTime));

        var result = status.Match(
            active => active.Description,
            retry => $"Retry {retry.Attempt}",
            aborted => $"Aborted at {aborted.CancellationDateTime}");
        
        result.ShouldBe($"Aborted at {abortTime}");
    }
}
