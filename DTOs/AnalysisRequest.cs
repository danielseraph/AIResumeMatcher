using System.ComponentModel.DataAnnotations;

namespace AIResumeMatcher.DTOs;

public class AnalysisRequest
{
    [Required]
    [MinLength(50, ErrorMessage = "Resume text is too short. Please provide the full resume text.")]
    public string ResumeText { get; init; } = string.Empty;

    [Required]
    [MinLength(20, ErrorMessage = "Job description is too short. Please provide the full job description.")]
    public string JobDescription { get; init; } = string.Empty;
}
