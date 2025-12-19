using Toarnbeike.Unions.SourceGenerated.Complex;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class ConstructionTests
{
    [Test]
    public void Status_Should_BeConstructable_AsActive()
    {
        var status = Status.Active(new Active("Running"));
        status.IsActive.ShouldBeTrue();
        status.TryGetActive(out var active).ShouldBeTrue();
        active.Description.ShouldBe("Running");
    }

    [Test]
    public void Status_Should_BeConstructable_AsRetry()
    {
        var status = Status.Retry(new Retry(3));
        status.IsRetry.ShouldBeTrue();
        status.TryGetRetry(out var retry).ShouldBeTrue();
        retry.Attempt.ShouldBe(3);
    }

    [Test]
    public void Status_Should_BeConstructable_AsAborted()
    {
        var dateTime = DateTime.UtcNow;
        var status = Status.Aborted(new Aborted(dateTime));
        status.IsAborted.ShouldBeTrue();
        status.TryGetAborted(out var aborted).ShouldBeTrue();
        aborted.CancellationDateTime.ShouldBe(dateTime);
    }
}
