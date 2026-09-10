using Microsoft.AspNetCore.Mvc;

namespace AIResumeMatcher.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Status = "healthy" });
    }
}
