using Toarnbeike.Unions.SourceGenerated.Complex;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class SwitchTests
{
    [Test]
    public void Status_Switch_Should_Switch_ForActive()
    {
        var status = Status.Active(new Active("Running"));
        var output = "";

        status.Switch(
            active => output = active.Description,
            retry => output = $"Retry {retry.Attempt}",
            aborted => output = $"Aborted at {aborted.CancellationDateTime}");

        output.ShouldBe("Running");
    }

    [Test]
    public void Status_Switch_Should_Switch_ForRetry()
    {
        var status = Status.Retry(new Retry(3));
        var output = "";

        status.Switch(
           active => output = active.Description,
            retry => output = $"Retry {retry.Attempt}",
            aborted => output = $"Aborted at {aborted.CancellationDateTime}");

        output.ShouldBe("Retry 3");
    }

    [Test]
    public void Status_Switch_Should_Switch_ForAborted()
    {
        var abortTime = DateTime.UtcNow;
        var status = Status.Aborted(new Aborted(abortTime));
        var output = "";

        status.Switch(
            active => output = active.Description,
            retry => output = $"Retry {retry.Attempt}",
            aborted => output = $"Aborted at {aborted.CancellationDateTime}");

        output.ShouldBe($"Aborted at {abortTime}");
    }

    [Test]
    public async Task Status_SwitchAsync_Should_Switch()
    {
        var status = Status.Active(new Active("Running"));
        var output = "";

        await status.SwitchAsync(
            async active => { await Task.Yield(); output = active.Description; },
            async retry => { await Task.Yield(); output = $"Retry {retry.Attempt}"; },
            async aborted => { await Task.Yield(); output = $"Aborted at {aborted.CancellationDateTime}"; }
            );

        output.ShouldBe("Running");
    }

    [Test]
    public async Task StatusTask_Match_Should_Work()
    {
        var statusTask = Task.FromResult(Status.Active(new Active("Running")));
        var output = "";

        await statusTask.Switch(
            active => output = active.Description,
            retry => output = $"Retry {retry.Attempt}",
            aborted => output = $"Aborted at {aborted.CancellationDateTime}");

        output.ShouldBe("Running");
    }

    [Test]
    public async Task StatusTask_MatchAsync_Should_Work()
    {
        var statusTask = Task.FromResult(Status.Active(new Active("Running")));
        var output = "";

        await statusTask.SwitchAsync(
            async active => { await Task.Yield(); output = active.Description; },
            async retry => { await Task.Yield(); output = $"Retry {retry.Attempt}"; },
            async aborted => { await Task.Yield(); output = $"Aborted at {aborted.CancellationDateTime}"; }
            );

        output.ShouldBe("Running");
    }
}
