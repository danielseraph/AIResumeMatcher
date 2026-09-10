namespace AIResumeMatcher.Configuration;

public sealed class LlmOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 60;
}
