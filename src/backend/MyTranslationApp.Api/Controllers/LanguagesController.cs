using Microsoft.AspNetCore.Mvc;
using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Api.Controllers;

[ApiController]
[Route("api")]
public class LanguagesController : ControllerBase
{
    private readonly ILanguageCatalogService _catalogService;
    private readonly ILogger<LanguagesController> _logger;

    public LanguagesController(
        ILanguageCatalogService catalogService,
        ILogger<LanguagesController> logger)
    {
        _catalogService = catalogService;
        _logger = logger;
    }

    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages(CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? string.Empty;

        _logger.LogInformation("GET /api/languages. CorrelationId: {CorrelationId}", correlationId);

        var languages = await _catalogService.GetSupportedLanguagesAsync(cancellationToken);

        return Ok(new LanguageListResponseDto
        {
            Languages = languages.ToList(),
            CorrelationId = correlationId
        });
    }
}
