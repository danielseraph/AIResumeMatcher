namespace AIResumeMatcher.DTOs;

public sealed class ResumeMatchRequest
{
    public string ResumeText { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
}
