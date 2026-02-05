using Toarnbeike.Unions.Nominal.TestExtensions;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class BindAsyncTests
{
    [Test]
    public async Task Status_BindActiveAsync_Should_Bind_WhenActive()
    {
        var status = Status.Active("Running");

        var newStatus = await status.BindActiveAsync(_ => Task.FromResult(Status.Retry(1)));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(1);
    }

    [Test]
    public async Task Status_BindActiveAsync_Should_NotBind_WhenNotActive()
    {
        var status = Status.Aborted(DateTime.UtcNow);

        var newStatus = await status.BindActiveAsync(_ => Task.FromResult(Status.Retry(1)));

        newStatus.ShouldBeAborted();
    }

    [Test]
    public async Task StatusTask_BindActiveAsync()
    {
        var statusTask = Task.FromResult(Status.Active("Running"));

        var newStatus = await statusTask.BindActiveAsync(_ => Task.FromResult(Status.Retry(1)));

        newStatus.ShouldBeRetry().Attempt.ShouldBe(1);
    }

    [Test]
    public async Task Status_BindRetryAsync_Should_Bind_WhenRetry()
    {
        var status = Status.Retry(3);

        var newStatus = await status.BindRetryAsync(current =>
            Task.FromResult(current.Attempt >= 3 ? Status.Aborted(DateTime.Now) : Status.Retry(current.Attempt + 1)));

        newStatus.ShouldBeAborted();
    }

    [Test]
    public async Task Status_BindRetryAsync_Should_NotBind_WhenNotRetry()
    {
        var status = Status.Active("Running");

        var newStatus = await status.BindRetryAsync(current =>
            Task.FromResult(current.Attempt >= 3 ? Status.Aborted(DateTime.Now) : Status.Retry(current.Attempt + 1)));


        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task StatusTask_BindRetryAsync()
    {
        var statusTask = Task.FromResult(Status.Retry(3));

        var newStatus = await statusTask.BindRetryAsync(current =>
            Task.FromResult(current.Attempt >= 3 ? Status.Aborted(DateTime.Now) : Status.Retry(current.Attempt + 1)));


        newStatus.ShouldBeAborted();
    }

    [Test]
    public async Task Status_BindAbortedAsync_Should_Bind_WhenAborted()
    {
        var status = Status.Aborted(DateTime.UtcNow);

        var newStatus = await status.BindAbortedAsync(current =>
            Task.FromResult(Status.Active($"Restarted after aborted {current.CancellationDateTime}")));

        newStatus.ShouldBeActive();
    }

    [Test]
    public async Task Status_BindAbortedAsync_Should_NotBind_WhenNotAborted()
    {
        var status = Status.Retry(3);

        var newStatus = await status.BindAbortedAsync(current =>
            Task.FromResult(Status.Active($"Restarted after aborted {current.CancellationDateTime}")));

        newStatus.ShouldBeRetry();
    }

    [Test]
    public async Task StatusTask_BindAbortedAsync()
    {
        var statusTask = Task.FromResult(Status.Aborted(DateTime.UtcNow));

        var newStatus = await statusTask.BindAbortedAsync(current =>
            Task.FromResult(Status.Active($"Restarted after aborted {current.CancellationDateTime}")));

        newStatus.ShouldBeActive();
    }
}