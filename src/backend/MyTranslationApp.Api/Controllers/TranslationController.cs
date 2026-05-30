using Microsoft.AspNetCore.Mvc;
using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Services;
using MyTranslationApp.Application.Validation;

namespace MyTranslationApp.Api.Controllers;

[ApiController]
[Route("api/translate")]
public class TranslationController : ControllerBase
{
    private readonly TranslationService _service;
    private readonly TextToSpeechService _ttsService;
    private readonly TranslationRequestValidator _validator;
    private readonly ILogger<TranslationController> _logger;

    public TranslationController(
        TranslationService service,
        TextToSpeechService ttsService,
        TranslationRequestValidator validator,
        ILogger<TranslationController> logger)
    {
        _service = service;
        _ttsService = ttsService;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost("text")]
    public async Task<IActionResult> TranslateText(
        [FromBody] TextTranslationRequestDto request,
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? string.Empty;

        _logger.LogInformation(
            "POST /api/translate/text. CorrelationId: {CorrelationId}, TargetLanguage: {TargetLanguage}",
            correlationId, request.TargetLanguage);

        var validation = _validator.ValidateTextRequest(request);
        if (!validation.IsValid)
        {
            _logger.LogInformation(
                "Validation failed. CorrelationId: {CorrelationId}, ErrorCode: {ErrorCode}",
                correlationId, validation.ErrorCode);

            return BadRequest(new ApiErrorResponseDto
            {
                ErrorCode = validation.ErrorCode!,
                Message = validation.Message!,
                Details = validation.Details.ToList(),
                CorrelationId = correlationId
            });
        }

        var result = await _service.TranslateTextAsync(request, cancellationToken);
        result.CorrelationId = correlationId;

        _logger.LogInformation(
            "POST /api/translate/text completed. CorrelationId: {CorrelationId}",
            correlationId);

        return Ok(result);
    }

    [HttpPost("audio")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> TranslateAudio(
        IFormFile? audio,
        [FromForm] string? targetLanguage,
        [FromForm] string? sourceLanguage,
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? string.Empty;
        var contentType = audio?.ContentType ?? string.Empty;

        _logger.LogInformation(
            "POST /api/translate/audio. CorrelationId: {CorrelationId}, ContentType: {ContentType}, TargetLanguage: {TargetLanguage}",
            correlationId, contentType, targetLanguage);

        var hasAudio = audio != null && audio.Length > 0;
        var audioSize = audio?.Length ?? 0;

        var validation = _validator.ValidateAudioRequest(hasAudio, audioSize, contentType, targetLanguage);
        if (!validation.IsValid)
        {
            _logger.LogInformation(
                "Validation failed. CorrelationId: {CorrelationId}, ErrorCode: {ErrorCode}",
                correlationId, validation.ErrorCode);

            return BadRequest(new ApiErrorResponseDto
            {
                ErrorCode = validation.ErrorCode!,
                Message = validation.Message!,
                Details = validation.Details.ToList(),
                CorrelationId = correlationId
            });
        }

        await using var stream = audio!.OpenReadStream();
        var result = await _service.TranslateAudioAsync(
            stream, contentType, targetLanguage!, sourceLanguage, cancellationToken);
        result.CorrelationId = correlationId;

        _logger.LogInformation(
            "POST /api/translate/audio completed. CorrelationId: {CorrelationId}",
            correlationId);

        return Ok(result);
    }

    [HttpPost("tts")]
    public async Task<IActionResult> SynthesizeSpeech(
        [FromBody] TtsSynthesisRequestDto request,
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? string.Empty;

        _logger.LogInformation(
            "POST /api/translate/tts. CorrelationId: {CorrelationId}, Language: {Language}",
            correlationId, request.Language);

        var validation = _validator.ValidateTtsRequest(request.Text, request.Language);
        if (!validation.IsValid)
        {
            _logger.LogInformation(
                "TTS validation failed. CorrelationId: {CorrelationId}, ErrorCode: {ErrorCode}",
                correlationId, validation.ErrorCode);

            return BadRequest(new ApiErrorResponseDto
            {
                ErrorCode = validation.ErrorCode!,
                Message = validation.Message!,
                Details = validation.Details.ToList(),
                CorrelationId = correlationId
            });
        }

        var (audioData, contentType) = await _ttsService.SynthesizeAsync(request, cancellationToken);

        Response.Headers["X-Correlation-ID"] = correlationId;

        _logger.LogInformation(
            "POST /api/translate/tts completed. CorrelationId: {CorrelationId}, ContentType: {ContentType}, Bytes: {Bytes}",
            correlationId, contentType, audioData.Length);

        return File(audioData, contentType);
    }
}
