using Microsoft.Extensions.Logging;
using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Application.Services;

public class TranslationService
{
    private readonly ITextTranslationProvider _translationProvider;
    private readonly ISpeechToTextProvider _speechProvider;
    private readonly ILogger<TranslationService> _logger;

    public TranslationService(
        ITextTranslationProvider translationProvider,
        ISpeechToTextProvider speechProvider,
        ILogger<TranslationService> logger)
    {
        _translationProvider = translationProvider;
        _speechProvider = speechProvider;
        _logger = logger;
    }

    public async Task<TextTranslationResponseDto> TranslateTextAsync(
        TextTranslationRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Translating text. TargetLanguage: {TargetLanguage}, Provider: {Provider}",
            request.TargetLanguage, _translationProvider.ProviderName);

        var translatedText = await _translationProvider.TranslateAsync(
            request.SourceText,
            request.TargetLanguage,
            request.SourceLanguage,
            cancellationToken);

        return new TextTranslationResponseDto
        {
            TranslatedText = translatedText,
            SourceLanguage = request.SourceLanguage ?? "auto",
            TargetLanguage = request.TargetLanguage,
            Provider = _translationProvider.ProviderName
        };
    }

    public async Task<AudioTranslationResponseDto> TranslateAudioAsync(
        Stream audioStream,
        string contentType,
        string targetLanguage,
        string? sourceLanguage,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Transcribing audio. ContentType: {ContentType}, TargetLanguage: {TargetLanguage}, Provider: {Provider}",
            contentType, targetLanguage, _speechProvider.ProviderName);

        var transcript = await _speechProvider.TranscribeAsync(
            audioStream, contentType, sourceLanguage, cancellationToken);

        _logger.LogInformation("Translating transcript. TargetLanguage: {TargetLanguage}", targetLanguage);

        var translatedText = await _translationProvider.TranslateAsync(
            transcript, targetLanguage, sourceLanguage, cancellationToken);

        return new AudioTranslationResponseDto
        {
            TranscribedText = transcript,
            TranslatedText = translatedText,
            SourceLanguage = sourceLanguage ?? "auto",
            TargetLanguage = targetLanguage,
            Provider = _translationProvider.ProviderName
        };
    }
}
