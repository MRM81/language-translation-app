using Microsoft.Extensions.Logging.Abstractions;
using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Services;
using MyTranslationApp.Infrastructure.Providers;

namespace MyTranslationApp.Tests.Services;

public class TranslationServiceTests
{
    private readonly TranslationService _service;

    public TranslationServiceTests()
    {
        _service = new TranslationService(
            new MockTextTranslationProvider(),
            new MockSpeechToTextProvider(),
            NullLogger<TranslationService>.Instance);
    }

    [Fact]
    public async Task TranslateTextAsync_ValidRequest_ReturnsMockTranslation()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            SourceLanguage = "en",
            TargetLanguage = "es"
        };

        var result = await _service.TranslateTextAsync(request);

        Assert.Equal("[mock-es] Hello", result.TranslatedText);
        Assert.Equal("en", result.SourceLanguage);
        Assert.Equal("es", result.TargetLanguage);
        Assert.Equal("mock", result.Provider);
    }

    [Fact]
    public async Task TranslateTextAsync_NullSourceLanguage_UsesAutoFallback()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            SourceLanguage = null,
            TargetLanguage = "fr"
        };

        var result = await _service.TranslateTextAsync(request);

        Assert.Equal("auto", result.SourceLanguage);
        Assert.Equal("[mock-fr] Hello", result.TranslatedText);
    }

    [Fact]
    public async Task TranslateAudioAsync_ValidRequest_ReturnsMockTranscriptAndTranslation()
    {
        using var stream = new MemoryStream(new byte[] { 0x01, 0x02, 0x03 });

        var result = await _service.TranslateAudioAsync(
            stream,
            contentType: "audio/webm",
            targetLanguage: "es",
            sourceLanguage: "en");

        Assert.Equal("[mock transcript]", result.TranscribedText);
        Assert.Equal("[mock-es] [mock transcript]", result.TranslatedText);
        Assert.Equal("en", result.SourceLanguage);
        Assert.Equal("es", result.TargetLanguage);
        Assert.Equal("mock", result.Provider);
    }

    [Fact]
    public async Task TranslateAudioAsync_NullSourceLanguage_UsesAutoFallback()
    {
        using var stream = new MemoryStream(new byte[] { 0x01 });

        var result = await _service.TranslateAudioAsync(
            stream,
            contentType: "audio/webm",
            targetLanguage: "de",
            sourceLanguage: null);

        Assert.Equal("auto", result.SourceLanguage);
        Assert.Equal("[mock-de] [mock transcript]", result.TranslatedText);
    }

    [Fact]
    public async Task TranslateTextAsync_SupportsCancellationToken()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Test",
            TargetLanguage = "es"
        };

        using var cts = new CancellationTokenSource();
        var result = await _service.TranslateTextAsync(request, cts.Token);

        Assert.NotNull(result);
    }
}
