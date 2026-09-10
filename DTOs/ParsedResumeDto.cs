namespace AIResumeMatcher.DTOs;

public sealed class ParsedResumeDto
{
    public string FileName { get; init; } = string.Empty;
    public int PageCount { get; init; }
    public string ExtractedText { get; init; } = string.Empty;
    public DateTimeOffset ExtractedAt { get; init; }
}
