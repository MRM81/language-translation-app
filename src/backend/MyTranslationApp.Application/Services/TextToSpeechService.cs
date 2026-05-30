using Microsoft.Extensions.Logging;
using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Application.Services;

public class TextToSpeechService
{
    private readonly ITextToSpeechProvider _ttsProvider;
    private readonly ILogger<TextToSpeechService> _logger;

    public TextToSpeechService(
        ITextToSpeechProvider ttsProvider,
        ILogger<TextToSpeechService> logger)
    {
        _ttsProvider = ttsProvider;
        _logger = logger;
    }

    public async Task<(byte[] AudioData, string ContentType)> SynthesizeAsync(
        TtsSynthesisRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Synthesizing speech. Language: {Language}",
            request.Language);

        return await _ttsProvider.SynthesizeAsync(
            request.Text,
            request.Language,
            voice: null,
            cancellationToken);
    }
}
