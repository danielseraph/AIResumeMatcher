namespace AIResumeMatcher.Exceptions;

public sealed class ResumeMatchingException : Exception
{
    public ResumeMatchingException(string message) : base(message) { }
    public ResumeMatchingException(string message, Exception inner) : base(message, inner) { }
}
