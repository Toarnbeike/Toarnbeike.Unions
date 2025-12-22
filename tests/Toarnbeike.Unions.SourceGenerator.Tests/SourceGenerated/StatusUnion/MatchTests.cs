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

    [Test]
    public async Task Status_MatchAsync_Should_Work()
    {
        var status = Status.Active(new Active("Running"));

        var result = await status.MatchAsync(
            active => Task.FromResult(active.Description),
            retry => Task.FromResult($"Retry {retry.Attempt}"),
            aborted => Task.FromResult($"Aborted at {aborted.CancellationDateTime}")
            );

        result.ShouldBe("Running");
    }

    [Test]
    public async Task StatusTask_Match_Should_Work()
    {
        var statusTask = Task.FromResult(Status.Active(new Active("Running")));

        var result = await statusTask.Match(
            active => active.Description,
            retry => $"Retry {retry.Attempt}",
            aborted => $"Aborted at {aborted.CancellationDateTime}");

        result.ShouldBe("Running");
    }

    [Test]
    public async Task StatusTask_MatchAsync_Should_Work()
    {
        var statusTask = Task.FromResult(Status.Active(new Active("Running")));

        var result = await statusTask.MatchAsync(
            active => Task.FromResult(active.Description),
            retry => Task.FromResult($"Retry {retry.Attempt}"),
            aborted => Task.FromResult($"Aborted at {aborted.CancellationDateTime}"));

        result.ShouldBe("Running");
    }
}
