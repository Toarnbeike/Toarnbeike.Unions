using Toarnbeike.Unions.Nominal.TestExtensions;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class MapAsyncTests
{
    [Test]
    public async Task Status_MapActiveAsync_Should_Map_WhenActive()
    {
        var status = Status.Active("Running");

        var newStatus =
            await status.MapActiveAsync(prev => Task.FromResult(new Active($"still {prev.Description}")));

        newStatus.ShouldBeActive().Description.ShouldBe("still Running");
    }

    [Test]
    public async Task Status_MapActiveAsync_Should_NotMap_WhenNotActive()
    {
        var status = Status.Retry(1);

        var newStatus =
            await status.MapActiveAsync(prev => Task.FromResult(new Active($"still {prev.Description}")));

        newStatus.ShouldBeRetry();
    }

    [Test]
    public async Task StatusTask_MapActiveAsync()
    {
        var statusTask = Task.FromResult(Status.Active("Running"));

        var newStatus =
            await statusTask.MapActiveAsync(prev => Task.FromResult(new Active($"still {prev.Description}")));

        newStatus.ShouldBeActive().Description.ShouldBe("still Running");
    }

    [Test]
    public async Task Status_MapRetryAsync_Should_Map_WhenRetry()
    {
        var status = Status.Retry(1);

        var newStatus = await status.MapRetryAsync(prev => Task.FromResult(new Retry(prev.Attempt + 1)));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(2);
    }

    [Test]
    public async Task Status_MapRetryAsync_Should_NotMap_WhenNotRetry()
    {
        var status = Status.Active("Running");

        var newStatus = await status.MapRetryAsync(prev => Task.FromResult(new Retry(prev.Attempt + 1)));

        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task StatusTask_MapRetryAsync()
    {
        var statusTask = Task.FromResult(Status.Retry(1));

        var newStatus = await statusTask.MapRetryAsync(prev => Task.FromResult(new Retry(prev.Attempt + 1)));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(2);
    }

    [Test]
    public async Task Status_MapAbortedAsync_Should_Map_WhenAborted()
    {
        var updated = new DateTime(2026, 2, 1, 12, 0, 0);
        var status = Status.Aborted(DateTime.UtcNow);

        var newStatus = await status.MapAbortedAsync(_ => Task.FromResult(new Aborted(updated)));

        newStatus.ShouldBeAborted().CancellationDateTime.ShouldBe(updated);
    }

    [Test]
    public async Task Status_MapAbortedAsync_Should_NotMap_WhenNotAborted()
    {
        var updated = new DateTime(2026, 2, 1, 12, 0, 0);
        var status = Status.Active("Running");

        var newStatus = await status.MapAbortedAsync(_ => Task.FromResult(new Aborted(updated)));

        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task StatusTask_MapAbortedAsync()
    {
        var updated = new DateTime(2026, 2, 1, 12, 0, 0);
        var statusTask = Task.FromResult(Status.Aborted(DateTime.UtcNow));

        var newStatus = await statusTask.MapAbortedAsync(_ => Task.FromResult(new Aborted(updated)));

        newStatus.ShouldBeAborted().CancellationDateTime.ShouldBe(updated);
    }
}