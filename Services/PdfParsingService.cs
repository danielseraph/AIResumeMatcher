using System.Text;
using AIResumeMatcher.Configuration;
using AIResumeMatcher.DTOs;
using AIResumeMatcher.Exceptions;
using AIResumeMatcher.Services.Interfaces;
using Microsoft.Extensions.Options;
using UglyToad.PdfPig;

namespace AIResumeMatcher.Services;

public sealed class PdfParsingService : IPdfParsingService
{
    private readonly FileUploadOptions _options;
    private readonly ILogger<PdfParsingService> _logger;

    public PdfParsingService(IOptions<FileUploadOptions> options, ILogger<PdfParsingService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ParsedResumeDto> ParseAsync(IFormFile file, CancellationToken cancellationToken)
    {
        ValidateFile(file);

        _logger.LogInformation("Parsing PDF: {FileName} ({Size} bytes)", file.FileName, file.Length);

        // Copy to a MemoryStream so PdfPig can seek freely.
        // Use 81920 buffer size to avoid LOH (Large Object Heap) allocations
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream, 81920, cancellationToken);
        memoryStream.Position = 0;

        var extractedText = ExtractText(memoryStream, out int pageCount);

        _logger.LogInformation(
            "Parsed PDF '{FileName}': {PageCount} page(s), {CharCount} characters extracted.",
            file.FileName, pageCount, extractedText.Length);

        return new ParsedResumeDto
        {
            FileName = file.FileName,
            PageCount = pageCount,
            ExtractedText = extractedText,
            ExtractedAt = DateTimeOffset.UtcNow
        };
    }

    private void ValidateFile(IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new InvalidFileException("No file was provided or the file is empty.");

        if (file.Length > _options.MaxFileSizeBytes)
        {
            var limitMb = _options.MaxFileSizeBytes / (1024 * 1024);
            throw new InvalidFileException($"File size exceeds the maximum allowed limit of {limitMb} MB.");
        }

        var contentType = file.ContentType?.ToLowerInvariant();
        if (contentType is null || !_options.AllowedContentTypes.Contains(contentType))
            throw new InvalidFileException(
                $"Invalid file type. Only the following types are accepted: {string.Join(", ", _options.AllowedContentTypes)}.");
    }

    private static string ExtractText(Stream pdfStream, out int pageCount)
    {
        using var document = PdfDocument.Open(pdfStream);
        
        // Optimize StringBuilder allocation (heuristic: ~10% of PDF size is text)
        var estimatedCapacity = (int)Math.Min(pdfStream.Length / 10, int.MaxValue);
        var sb = new StringBuilder(estimatedCapacity);
        
        pageCount = document.NumberOfPages;

        foreach (var page in document.GetPages())
        {
            // GetWords() provides better word grouping than iterating raw letters.
            var words = page.GetWords().ToList();
            
            // Handle empty pages safely (e.g., scanned images)
            if (words.Count == 0)
            {
                sb.AppendLine();
                continue;
            }

            sb.AppendJoin(' ', words.Select(w => w.Text));
            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }
}
