using System.Text.Json;
using AIResumeMatcher.Configuration;
using AIResumeMatcher.Middleware;
using AIResumeMatcher.Services;
using AIResumeMatcher.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers with camelCase JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Health checks
builder.Services.AddHealthChecks();

// Strongly-typed configuration
builder.Services.Configure<LlmOptions>(builder.Configuration.GetSection("Llm"));
builder.Services.Configure<FileUploadOptions>(builder.Configuration.GetSection("FileUpload"));

// Application services
builder.Services.AddScoped<IPdfParsingService, PdfParsingService>();

builder.Services.AddHttpClient<ILlmAnalysisService, LlmAnalysisService>()
    .AddStandardResilienceHandler();

// CORS — credentials require an explicit origin, not AllowAnyOrigin()
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin == "https://ai-resume-matcher-app.vercel.app" ||
                origin == "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Swagger/OpenAPI — Development only
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handling must be first in the pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
