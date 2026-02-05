using Toarnbeike.Unions.Nominal.TestExtensions;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class MapSyncTests
{
    [Test]
    public void Status_MapActive_Should_Map_WhenActive()
    {
        var status = Status.Active("Running");

        var newStatus = status.MapActive(prev => new Active($"still {prev.Description}"));

        newStatus.ShouldBeActive().Description.ShouldBe("still Running");
    }

    [Test]
    public void Status_MapActive_Should_NotMap_WhenNotActive()
    {
        var status = Status.Retry(1);

        var newStatus = status.MapActive(prev => new Active($"still {prev.Description}"));

        newStatus.ShouldBeRetry();
    }

    [Test]
    public async Task StatusTask_MapActive()
    {
        var statusTask = Task.FromResult(Status.Active("Running"));

        var newStatus = await statusTask.MapActive(prev => new Active($"still {prev.Description}"));

        newStatus.ShouldBeActive().Description.ShouldBe("still Running");
    }

    [Test]
    public void Status_MapRetry_Should_Map_WhenRetry()
    {
        var status = Status.Retry(1);

        var newStatus = status.MapRetry(prev => new Retry(prev.Attempt + 1));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(2);
    }

    [Test]
    public void Status_MapRetry_Should_NotMap_WhenNotRetry()
    {
        var status = Status.Active("Running");

        var newStatus = status.MapRetry(prev => new Retry(prev.Attempt + 1));

        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task StatusTask_MapRetry()
    {
        var statusTask = Task.FromResult(Status.Retry(1));

        var newStatus = await statusTask.MapRetry(prev => new Retry(prev.Attempt + 1));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(2);
    }

    [Test]
    public void Status_MapAborted_Should_Map_WhenAborted()
    {
        var updated = new DateTime(2026, 2, 1, 12, 0, 0);
        var status = Status.Aborted(DateTime.UtcNow);

        var newStatus = status.MapAborted(_ => new Aborted(updated));

        newStatus.ShouldBeAborted().CancellationDateTime.ShouldBe(updated);
    }

    [Test]
    public void Status_MapAborted_Should_NotMap_WhenNotAborted()
    {
        var updated = new DateTime(2026, 2, 1, 12, 0, 0);
        var status = Status.Active("Running");

        var newStatus = status.MapAborted(_ => new Aborted(updated));

        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task StatusTask_MapAborted()
    {
        var updated = new DateTime(2026, 2, 1, 12, 0, 0);
        var statusTask = Task.FromResult(Status.Aborted(DateTime.UtcNow));

        var newStatus = await statusTask.MapAborted(_ => new Aborted(updated));

        newStatus.ShouldBeAborted().CancellationDateTime.ShouldBe(updated);
    }
}