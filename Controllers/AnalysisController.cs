using AIResumeMatcher.DTOs;
using AIResumeMatcher.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeMatcher.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AnalysisController : ControllerBase
{
    private readonly IPdfParsingService _pdfParsingService;
    private readonly ILlmAnalysisService _llmAnalysisService;

    public AnalysisController(IPdfParsingService pdfParsingService, ILlmAnalysisService llmAnalysisService)
    {
        _pdfParsingService = pdfParsingService;
        _llmAnalysisService = llmAnalysisService;
    }

    /// <summary>
    /// Uploads a PDF resume and returns the extracted text and metadata.
    /// </summary>
    /// <param name="file">The PDF file to parse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted resume data including text and page count.</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ParsedResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await _pdfParsingService.ParseAsync(file, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Analyzes a parsed resume against a job description using an LLM.
    /// Accepts multipart/form-data so multi-line text can be submitted without JSON escaping.
    /// </summary>
    /// <param name="request">The resume text and job description as form fields.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A structured analysis result with match score, skills, and suggestions.</returns>
    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Analyze([FromForm] AnalysisRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _llmAnalysisService.AnalyzeAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Analyzes a parsed resume against a job description using an LLM.
    /// Accepts application/json — ensure newlines in strings are escaped as \n.
    /// </summary>
    /// <param name="request">The resume text and job description as JSON body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A structured analysis result with match score, skills, and suggestions.</returns>
    [HttpPost("analyze/json")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AnalysisResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AnalyzeJson([FromBody] AnalysisRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _llmAnalysisService.AnalyzeAsync(request, cancellationToken);
        return Ok(result);
    }
}
