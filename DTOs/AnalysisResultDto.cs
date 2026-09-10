namespace AIResumeMatcher.DTOs;

public class AnalysisResultDto
{
    /// <summary>Overall match score 0–100.</summary>
    public int MatchScore { get; init; }

    /// <summary>One-paragraph plain-English summary of how well the resume fits the role.</summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>Breakdown of the score across three dimensions.</summary>
    public ScoreBreakdownDto ScoreBreakdown { get; init; } = new();

    /// <summary>Technical skills found in the resume that are required by the job.</summary>
    public List<SkillDetailDto> MatchedHardSkills { get; init; } = [];

    /// <summary>Technical skills required by the job that are absent or unclear in the resume.</summary>
    public List<SkillDetailDto> MissingHardSkills { get; init; } = [];

    /// <summary>Soft skills found in the resume that align with the job.</summary>
    public List<SkillDetailDto> MatchedSoftSkills { get; init; } = [];

    /// <summary>Analysis of the candidate's experience level versus the job requirements.</summary>
    public string ExperienceAnalysis { get; init; } = string.Empty;

    /// <summary>Detailed, actionable improvement suggestions.</summary>
    public List<SuggestionDto> Suggestions { get; init; } = [];

    /// <summary>Final plain-English hiring recommendation verdict.</summary>
    public string Verdict { get; init; } = string.Empty;
}

public class ScoreBreakdownDto
{
    public int TechnicalSkills { get; init; }
    public int Experience { get; init; }
    public int SoftSkills { get; init; }
}

public class SkillDetailDto
{
    /// <summary>The skill name.</summary>
    public string Skill { get; init; } = string.Empty;

    /// <summary>Brief explanation of why this skill is relevant or how it was evidenced in the resume.</summary>
    public string Detail { get; init; } = string.Empty;
}

public class SuggestionDto
{
    /// <summary>Short title of the suggestion.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Detailed explanation of what to do and why it matters for this specific job.</summary>
    public string Detail { get; init; } = string.Empty;

    /// <summary>Priority level: High, Medium, or Low.</summary>
    public string Priority { get; init; } = string.Empty;
}
