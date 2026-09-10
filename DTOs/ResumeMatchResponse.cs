namespace AIResumeMatcher.DTOs;

public sealed class ResumeMatchResponse
{
    public double MatchScore { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<string> MatchedKeywords { get; set; } = [];
    public List<string> MissingKeywords { get; set; } = [];
}
