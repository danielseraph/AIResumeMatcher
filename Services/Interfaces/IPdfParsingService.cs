using AIResumeMatcher.DTOs;

namespace AIResumeMatcher.Services.Interfaces;

public interface IPdfParsingService
{
    /// <summary>
    /// Validates and parses the uploaded PDF file, returning extracted text and metadata.
    /// </summary>
    /// <param name="file">The uploaded PDF form file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="ParsedResumeDto"/> containing the extracted text and metadata.</returns>
    /// <exception cref="AIResumeMatcher.Exceptions.InvalidFileException">
    /// Thrown when the file is null, empty, wrong type, or exceeds the size limit.
    /// </exception>
    Task<ParsedResumeDto> ParseAsync(IFormFile file, CancellationToken cancellationToken);
}
