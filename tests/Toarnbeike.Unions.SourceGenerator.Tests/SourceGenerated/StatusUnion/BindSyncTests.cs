using Toarnbeike.Unions.Nominal.TestExtensions;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class BindSyncTests
{
    [Test]
    public void Status_BindActive_Should_Bind_WhenActive()
    {
        var status = Status.Active("Running");

        var newStatus = status.BindActive(_ => Status.Retry(1));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(1);
    }

    [Test]
    public void Status_BindActive_Should_NotBind_WhenNotActive()
    {
        var status = Status.Aborted(DateTime.UtcNow);

        var newStatus = status.BindActive(_ => Status.Retry(1));

        newStatus.ShouldBeAborted();
    }

    [Test]
    public async Task StatusTask_BindActive()
    {
        var statusTask = Task.FromResult(Status.Active("Running"));

        var newStatus = await statusTask.BindActive(_ => Status.Retry(1));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(1);
    }

    [Test]
    public void Status_BindRetry_Should_Bind_WhenRetry()
    {
        var status = Status.Retry(3);

        var newStatus = status.BindRetry(current =>
            current.Attempt >= 3 ? Status.Aborted(DateTime.Now) : Status.Retry(current.Attempt + 1));

        newStatus.ShouldBeAborted();
    }

    [Test]
    public void Status_BindRetry_Should_NotBind_WhenNotRetry()
    {
        var status = Status.Active("Running");

        var newStatus = status.BindRetry(current =>
            current.Attempt > 3 ? Status.Aborted(DateTime.Now) : Status.Retry(current.Attempt + 1));

        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task StatusTask_BindRetry()
    {
        var statusTask = Task.FromResult(Status.Retry(3));

        var newStatus = await statusTask.BindRetry(current =>
            current.Attempt >= 3 ? Status.Aborted(DateTime.Now) : Status.Retry(current.Attempt + 1));

        newStatus.ShouldBeAborted();
    }

    [Test]
    public void Status_BindAborted_Should_Bind_WhenAborted()
    {
        var status = Status.Aborted(DateTime.UtcNow);

        var newStatus = status.BindAborted(current =>
            Status.Active($"Restarted after aborted {current.CancellationDateTime}"));

        newStatus.ShouldBeActive();
    }

    [Test]
    public void Status_BindAborted_Should_NotBind_WhenNotAborted()
    {
        var status = Status.Retry(3);

        var newStatus = status.BindAborted(current =>
            Status.Active($"Restarted after aborted {current.CancellationDateTime}"));

        newStatus.ShouldBeRetry();
    }

    [Test]
    public async Task StatusTask_BindAborted()
    {
        var statusTask = Task.FromResult(Status.Aborted(DateTime.UtcNow));

        var newStatus = await statusTask.BindAborted(current =>
            Status.Active($"Restarted after aborted {current.CancellationDateTime}"));

        newStatus.ShouldBeActive();
    }
}