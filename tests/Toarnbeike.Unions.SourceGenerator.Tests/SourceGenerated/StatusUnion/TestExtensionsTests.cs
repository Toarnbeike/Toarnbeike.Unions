namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

using Toarnbeike.Unions.SourceGenerated.Complex;
using Toarnbeike.Unions.TestExtensions;

public class TestExtensionsTests
{
    [Test]
    public void ShouldBeActive_Should_ReturnInstance_WhenActive()
    {
        var expected = new Active("Running");
        var status = Status.Active(expected);

        var actual = status.ShouldBeActive();
        actual.ShouldBe(expected);
    }

    [Test]
    public void ShouldBeActive_Should_Throw_WhenRetry()
    {
        var status = Status.Retry(new Retry(3));
        
        Should.Throw<AssertionFailedException>(status.ShouldBeActive);
    }

    [Test]
    public void ShouldBeActive_Should_ReturnInstance_WhenExpectationMatches()
    {
        var expected = new Active("Running");
        var status = Status.Active(expected);

        var actual = status.ShouldBeActive(expected);
        actual.ShouldBe(expected);
    }

    [Test]
    public void ShouldBeActive_Should_Throw_WhenExpectationDoesNotMatch()
    {
        var expected = new Active("Running");
        var unexpected = new Active("Stopped");
        var status = Status.Active(expected);

        Should.Throw<AssertionFailedException>(() => status.ShouldBeActive(unexpected));
    }

    [Test]
    public void ShouldBeRetry_Should_ReturnInstance_WhenExpectationMatches()
    {
        var expected = new Retry(3);
        var status = Status.Retry(expected);

        var actual = status.ShouldBeRetry(expected);
        actual.ShouldBe(expected);
    }

    [Test]
    public void ShouldBeAborted_Should_ReturnInstance_WhenExpectationMatches()
    {
        var expected = new Aborted(DateTime.Now);
        var status = Status.Aborted(expected);

        var actual = status.ShouldBeAborted(expected);
        actual.ShouldBe(expected);
    }
}
