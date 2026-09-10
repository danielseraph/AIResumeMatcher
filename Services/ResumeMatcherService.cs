using AIResumeMatcher.Services.Interfaces;
using AIResumeMatcher.DTOs;

namespace AIResumeMatcher.Services;

public sealed class ResumeMatcherService : IResumeMatcherService
{
    private readonly ILogger<ResumeMatcherService> _logger;

    public ResumeMatcherService(ILogger<ResumeMatcherService> logger)
    {
        _logger = logger;
    }

    public Task<ResumeMatchResponse> MatchAsync(ResumeMatchRequest request, CancellationToken cancellationToken = default)
    {
        // TODO: Implement LLM-powered resume matching logic
        _logger.LogInformation("Matching resume against job description.");
        throw new NotImplementedException();
    }
}
