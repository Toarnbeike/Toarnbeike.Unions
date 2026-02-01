namespace Toarnbeike.Unions.Generic.TestExtensions;

/// <summary>
/// Exception thrown when an assertion fails.
/// </summary>
public class AssertionFailedException(string message) : Exception(message);
