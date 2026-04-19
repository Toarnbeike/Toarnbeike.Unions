using Toarnbeike.Unions.Nominal.Collections;

namespace Toarnbeike.Unions.SourceGenerated.StatusUnion;

public class CollectionTests
{
    private static IEnumerable<Status> GenerateCollection()
    {
        yield return Status.Active("Running 1");
        yield return Status.Retry(1);
        yield return Status.Active("Running 2");
        yield return Status.Aborted(DateTime.UtcNow);
        yield return Status.Retry(2);
        yield return Status.Active("Running 3");
    }

    [Test]
    public void SelectActive_Should_SelectOnlyActiveStatusInstances()
    {
        var source = GenerateCollection();
        var result = source.SelectActive().ToList();
        
        result.Count.ShouldBe(3);
        result.ShouldAllBe(a => a.Description.StartsWith("Running"));
    }

    [Test]
    public void SelectRetry_Should_SelectOnlyRetryStatusInstances()
    {
        var source = GenerateCollection();
        var result = source.SelectRetry().ToList();

        result.Count.ShouldBe(2);
        result.Sum(r => r.Attempt).ShouldBe(3);
    }

    [Test]
    public void SelectAborted_Should_SelectOnlyRetryStatusInstances()
    {
        var source = GenerateCollection();
        var result = source.SelectAborted();

        result.Count().ShouldBe(1);
    }

    [Test]
    public void Partition_Should_SplitCollection()
    {
        var source = GenerateCollection();
        var (activeSet, retrySet, abortedSet) = source.Partition();

        activeSet.ShouldNotBeNull();
        activeSet.Count.ShouldBe(3);
        activeSet.Last().Description.ShouldBe("Running 3");

        retrySet.ShouldNotBeNull();
        retrySet.Count.ShouldBe(2);

        abortedSet.ShouldNotBeNull();
        abortedSet.Count.ShouldBe(1);
    }
}