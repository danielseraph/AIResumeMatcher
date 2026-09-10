using AIResumeMatcher.DTOs;

namespace AIResumeMatcher.Services.Interfaces;

public interface IResumeMatcherService
{
    Task<ResumeMatchResponse> MatchAsync(ResumeMatchRequest request, CancellationToken cancellationToken = default);
}
