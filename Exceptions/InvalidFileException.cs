namespace AIResumeMatcher.Exceptions;

/// <summary>
/// Thrown when an uploaded file fails validation (wrong type, too large, empty, etc.).
/// Maps to HTTP 400 Bad Request — the message is safe to expose to API clients.
/// </summary>
public sealed class InvalidFileException : Exception
{
    public InvalidFileException(string message) : base(message) { }
}
