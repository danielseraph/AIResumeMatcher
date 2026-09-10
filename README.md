# AI Resume Matcher

## Description

A .NET 10 Web API for AI-powered resume-to-job-description matching.

## Current Phase

**Phase 1 — Foundation**

## Technology

| Item | Details |
|---|---|
| Runtime | .NET 10 |
| Language | C# |
| Framework | ASP.NET Core Web API |
| API Docs | Swagger / OpenAPI (Swashbuckle) |
| Serialization | System.Text.Json (camelCase) |
| IoC | Built-in ASP.NET Core DI |
| Solution | `.slnx` (VS 2022 / .NET 10 format) |

## Running the Project

```bash
dotnet restore
dotnet build
dotnet run
```

## Swagger UI

Available in **Development** environment only:

```
http://localhost:{port}/swagger
```

## Health Check Endpoints

| Endpoint | Description |
|---|---|
| `GET /api/health` | Controller-based health endpoint → `{ "status": "healthy" }` |
| `GET /health` | ASP.NET Core built-in health checks |

## Environment Configuration

| Environment | Swagger | Developer Exceptions |
|---|---|---|
| Development | ✅ Enabled | ✅ Enabled |
| Staging | ❌ Disabled | ❌ Disabled |
| Production | ❌ Disabled | ❌ Disabled |

## LLM Configuration (Future Use)

Add LLM settings to `appsettings.json` under the `Llm` section:

```json
{
  "Llm": {
    "BaseUrl": "https://api.openai.com/v1",
    "Model": "gpt-4o",
    "TimeoutSeconds": 60
  }
}
```

> ⚠️ **Never store API keys in appsettings.json.**
> Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) during development:
>
> ```bash
> dotnet user-secrets set "Llm:ApiKey" "your-api-key-here"
> ```
>
> In production, use environment variables or a cloud secret manager (Azure Key Vault, AWS Secrets Manager, etc).

## CORS

The API allows requests from `http://localhost:4200` (Angular dev server) via the `FrontendPolicy` CORS policy.
