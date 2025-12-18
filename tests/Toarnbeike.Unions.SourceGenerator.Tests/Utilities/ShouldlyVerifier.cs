using Microsoft.CodeAnalysis.Testing;
using System.Diagnostics.CodeAnalysis;

namespace Toarnbeike.Unions.Utilities;

internal sealed class ShouldlyVerifier : IVerifier
{
    public void Equal<T>(T expected, T actual, string? message = null)
    {
        actual.ShouldBe(expected, message);
    }

    public void True(bool condition, string? message = null)
    {
        condition.ShouldBeTrue(message);
    }

    public void False(bool condition, string? message = null)
    {
        condition.ShouldBeFalse(message);
    }
    public void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T>? equalityComparer = null, string? message = null)
    {
        if (equalityComparer is null)
        {
            actual.ShouldBe(expected, message);
        }
        else
        {
            actual.ShouldBe(expected, comparer: equalityComparer, ignoreOrder: false, message);
        }
    }

    public void Empty<T>(string collectionName, IEnumerable<T> collection)
    {
        collection.ShouldBeEmpty($"Expected collection {collectionName} to be empty");
    }

    public void NotEmpty<T>(string collectionName, IEnumerable<T> collection)
    {
        collection.ShouldNotBeEmpty($"Expected collection {collectionName} to have items");
    }

    [DoesNotReturn]
    public void Fail(string? message)
    {
        throw new ShouldAssertException(message ?? "Test failed");
    }

    public void LanguageIsSupported(string language)
    {
        // Roslyn testing only calls this for C# / VB; fail if unsupported
        if (language != "C#" && language != "Visual Basic")
            throw new ShouldAssertException($"Language '{language}' is not supported");
    }

    public IVerifier PushContext(string context)
    {
        // No-op for Shouldly; return self
        return this;
    }
}
