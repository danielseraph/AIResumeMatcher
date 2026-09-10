using System.Net.Http.Headers;
using System.Text.Json;
using AIResumeMatcher.Configuration;
using AIResumeMatcher.DTOs;
using AIResumeMatcher.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AIResumeMatcher.Services;

public sealed class LlmAnalysisService : ILlmAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly LlmOptions _options;
    private readonly ILogger<LlmAnalysisService> _logger;

    public LlmAnalysisService(HttpClient httpClient, IOptions<LlmOptions> options, ILogger<LlmAnalysisService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AnalysisResultDto> AnalyzeAsync(AnalysisRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Analyzing resume against job description using model {Model}", _options.Model);

        var prompt = ConstructPrompt(request);
        
        var payload = new
        {
            model = _options.Model,
            messages = new[]
            {
                // Groq requires the word "JSON" in the system prompt when using response_format json_object
                new { role = "system", content = "You are an expert technical recruiter and resume analyzer. You must respond with a valid JSON object only. Do not include any markdown, code blocks, or explanation outside the JSON." },
                new { role = "user", content = prompt }
            },
            response_format = new { type = "json_object" },
            temperature = 0.2
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl);
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        httpRequest.Content = requestContent;

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("LLM API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            throw new InvalidOperationException($"LLM API call failed: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        
        return ParseResponse(jsonResponse);
    }

    private static string ConstructPrompt(AnalysisRequest request)
    {
        return $@"
        You are an expert senior technical recruiter with 15+ years of experience evaluating software engineering candidates.

        Perform a thorough, detailed analysis of the resume against the job description below.

        Return a single valid JSON object with EXACTLY this structure — no extra keys, no markdown:

        {{
          ""matchScore"": <integer 0-100, overall fit score>,
          ""summary"": ""<2-3 sentence plain English overview of how well the candidate fits the role and why>"",
          ""scoreBreakdown"": {{
            ""technicalSkills"": <integer 0-100, how well technical skills match>,
            ""experience"": <integer 0-100, how well experience level and domain match>,
            ""softSkills"": <integer 0-100, how well soft skills and culture match>
          }},
          ""matchedHardSkills"": [
            {{ ""skill"": ""<skill name>"", ""detail"": ""<where/how this skill was evidenced in the resume and why it satisfies the job requirement>"" }}
          ],
          ""missingHardSkills"": [
            {{ ""skill"": ""<skill name>"", ""detail"": ""<why this skill is required by the job and what gap it creates for the candidate>"" }}
          ],
          ""matchedSoftSkills"": [
            {{ ""skill"": ""<soft skill>"", ""detail"": ""<how this was demonstrated in the resume>"" }}
          ],
          ""experienceAnalysis"": ""<Detailed paragraph analysing the candidate's years of experience, seniority level, industry background, and how closely they match what the job requires>"",
          ""suggestions"": [
            {{
              ""title"": ""<short action title>"",
              ""detail"": ""<specific, actionable advice — what exactly to add, reword, or highlight on the resume, and why it will improve the match for this job>"",
              ""priority"": ""High"" | ""Medium"" | ""Low""
            }}
          ],
          ""verdict"": ""<Final 1-2 sentence hiring recommendation — would you shortlist this candidate? Be direct and honest.>""
        }}

        Rules:
        - Every ""detail"" field must be a full sentence — not just a label.
        - ""suggestions"" must have at least 3 items, ordered by priority descending.
        - ""missingHardSkills"" must list every required skill from the job description that the resume does not clearly demonstrate.
        - Be honest — do not inflate matchScore. A score of 40 is fine if the candidate is a poor fit.
        - Do not include any text outside the JSON object.

        --- JOB DESCRIPTION ---
        {request.JobDescription}

        --- RESUME ---
        {request.ResumeText}
        ";
    }

    private AnalysisResultDto ParseResponse(string jsonResponse)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonResponse);
            var root = doc.RootElement;
            
            // OpenAI chat completions format
            if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var message = choices[0].GetProperty("message");
                var content = message.GetProperty("content").GetString();
                
                if (!string.IsNullOrEmpty(content))
                {
                    return JsonSerializer.Deserialize<AnalysisResultDto>(content, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    }) ?? new AnalysisResultDto();
                }
            }
            
            _logger.LogWarning("Unexpected LLM response format: {Response}", jsonResponse);
            return new AnalysisResultDto();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse LLM response.");
            throw new InvalidOperationException("Failed to parse LLM response.", ex);
        }
    }
}
