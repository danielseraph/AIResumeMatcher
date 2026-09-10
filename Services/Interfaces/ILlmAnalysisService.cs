using AIResumeMatcher.DTOs;

namespace AIResumeMatcher.Services.Interfaces;

public interface ILlmAnalysisService
{
    Task<AnalysisResultDto> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken);
}
