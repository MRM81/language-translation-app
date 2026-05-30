using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTranslationApp.Application.Exceptions;
using MyTranslationApp.Application.Interfaces;
using MyTranslationApp.Application.Validation;
using MyTranslationApp.Infrastructure.Configuration;

namespace MyTranslationApp.Infrastructure.Providers.Azure;

public class AzureSpeechToTextProvider : ISpeechToTextProvider
{
    private readonly IOptions<AzureSpeechOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AzureSpeechToTextProvider> _logger;

    private const string FastTranscriptionApiVersion = "2025-10-15";
    private const string FastTranscriptionPath = "speechtotext/transcriptions:transcribe";

    // Maps app-supported BCP-47 short codes to Azure Speech full language tags.
    private static readonly Dictionary<string, string> LanguageMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "ar",      "ar-SA" },
            { "bg",      "bg-BG" },
            { "hr",      "hr-HR" },
            { "cs",      "cs-CZ" },
            { "da",      "da-DK" },
            { "nl",      "nl-NL" },
            { "en",      "en-US" },
            { "et",      "et-EE" },
            { "fi",      "fi-FI" },
            { "fr",      "fr-FR" },
            { "de",      "de-DE" },
            { "el",      "el-GR" },
            { "hi",      "hi-IN" },
            { "hu",      "hu-HU" },
            { "id",      "id-ID" },
            { "it",      "it-IT" },
            { "ja",      "ja-JP" },
            { "ko",      "ko-KR" },
            { "lv",      "lv-LV" },
            { "lt",      "lt-LT" },
            { "ms",      "ms-MY" },
            { "nb",      "nb-NO" },
            { "pl",      "pl-PL" },
            { "pt",      "pt-BR" },
            { "ro",      "ro-RO" },
            { "ru",      "ru-RU" },
            { "sr-Cyrl", "sr-RS" },
            { "sk",      "sk-SK" },
            { "sl",      "sl-SI" },
            { "es",      "es-ES" },
            { "sv",      "sv-SE" },
            { "th",      "th-TH" },
            { "tr",      "tr-TR" },
            { "uk",      "uk-UA" },
            { "vi",      "vi-VN" },
            { "zh",      "zh-CN" },
            { "zh-Hant", "zh-TW" },
        };

    public string ProviderName => "azure";

    public AzureSpeechToTextProvider(
        IOptions<AzureSpeechOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<AzureSpeechToTextProvider> logger)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> TranscribeAsync(
        Stream audioStream,
        string contentType,
        string? sourceLanguage,
        CancellationToken cancellationToken = default)
    {
        var opts = _options.Value;
        var language = ResolveLanguage(sourceLanguage);
        var normalisedType = NormaliseMimeType(contentType);

        _logger.LogInformation(
            "Calling Azure Speech STT. NormalisedType: {NormalisedType}, Language: {Language}",
            normalisedType, language);

        using var ms = new MemoryStream();
        await audioStream.CopyToAsync(ms, cancellationToken);
        var audioBytes = ms.ToArray();

        try
        {
            return normalisedType == "audio/wav"
                ? await RecognizeFromWavBytesAsync(opts, audioBytes, cancellationToken)
                : await RecognizeFromFastTranscriptionAsync(opts, audioBytes, normalisedType, language, cancellationToken);
        }
        catch (ProviderException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Azure Speech exception. ExceptionType: {ExceptionType}",
                ex.GetType().Name);
            throw new ProviderException(
                ErrorCodes.ProviderError,
                "The speech transcription service is temporarily unavailable. Please try again shortly.",
                ex);
        }
    }

    // WAV path: unchanged from Sprint 005. Uses Speech SDK temp-file approach.
    // AudioConfig.FromWavFileInput reads the WAV header to determine format.
    private static async Task<string> RecognizeFromWavBytesAsync(
        AzureSpeechOptions opts,
        byte[] audioBytes,
        CancellationToken cancellationToken)
    {
        var speechConfig = SpeechConfig.FromSubscription(opts.Key, opts.Region);

        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");
        try
        {
            await File.WriteAllBytesAsync(tempPath, audioBytes, cancellationToken);
            using var audioConfig = AudioConfig.FromWavFileInput(tempPath);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
            return await RunSdkRecognitionAsync(recognizer);
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static async Task<string> RunSdkRecognitionAsync(SpeechRecognizer recognizer)
    {
        var result = await recognizer.RecognizeOnceAsync();

        return result.Reason switch
        {
            ResultReason.RecognizedSpeech => result.Text,
            ResultReason.NoMatch => throw new ProviderException(
                ErrorCodes.TranscriptionFailed,
                "Speech recognition could not understand the audio. Please speak clearly and try again."),
            ResultReason.Canceled => HandleSdkCancellation(result),
            _ => throw new ProviderException(
                ErrorCodes.ProviderError,
                "Speech recognition returned an unexpected result.")
        };
    }

    private static string HandleSdkCancellation(SpeechRecognitionResult result)
    {
        var details = CancellationDetails.FromResult(result);
        if (details.Reason == CancellationReason.Error)
            throw new ProviderException(
                ErrorCodes.ProviderError,
                "The speech transcription service is temporarily unavailable. Please try again shortly.");

        throw new ProviderException(
            ErrorCodes.TranscriptionFailed,
            "Speech recognition was cancelled. Please try again.");
    }

    // Compressed audio path: MP3, WebM, OGG.
    // Uses Azure Speech Fast Transcription REST API (api-version 2025-10-15).
    // No GStreamer dependency — Azure decodes compressed formats server-side.
    private async Task<string> RecognizeFromFastTranscriptionAsync(
        AzureSpeechOptions opts,
        byte[] audioBytes,
        string normalisedMimeType,
        string language,
        CancellationToken cancellationToken)
    {
        ValidateSupportedCompressedFormat(normalisedMimeType);

        var url = BuildFastTranscriptionUrl(opts.Endpoint);
        var definitionJson = $"{{\"locales\":[\"{language}\"]}}";

        using var audioContent = new ByteArrayContent(audioBytes);
        audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse(normalisedMimeType);

        using var definitionContent = new StringContent(definitionJson, Encoding.UTF8, "application/json");

        using var form = new MultipartFormDataContent();
        form.Add(audioContent, "audio", "audio");
        form.Add(definitionContent, "definition");

        var httpClient = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        request.Headers.Add("Ocp-Apim-Subscription-Key", opts.Key);

        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(
                "Fast Transcription HTTP request failed. ExceptionType: {ExceptionType}",
                ex.GetType().Name);
            throw new ProviderException(
                ErrorCodes.ProviderError,
                "The speech transcription service is temporarily unavailable. Please try again shortly.",
                ex);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Fast Transcription returned non-success status. StatusCode: {StatusCode}",
                    (int)response.StatusCode);
                throw new ProviderException(
                    ErrorCodes.ProviderError,
                    "The speech transcription service is temporarily unavailable. Please try again shortly.");
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            return ParseFastTranscriptionResponse(json);
        }
    }

    private static string ParseFastTranscriptionResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("combinedPhrases", out var combinedPhrases) ||
            combinedPhrases.GetArrayLength() == 0)
        {
            throw new ProviderException(
                ErrorCodes.TranscriptionFailed,
                "Speech recognition could not understand the audio. Please speak clearly and try again.");
        }

        var text = combinedPhrases[0].GetProperty("text").GetString();

        if (string.IsNullOrWhiteSpace(text))
            throw new ProviderException(
                ErrorCodes.TranscriptionFailed,
                "Speech recognition could not understand the audio. Please speak clearly and try again.");

        return text;
    }

    private static string BuildFastTranscriptionUrl(string endpoint) =>
        $"{endpoint.TrimEnd('/')}/{FastTranscriptionPath}?api-version={FastTranscriptionApiVersion}";

    // Only WAV, MP3, WebM, and OGG are routed to this provider.
    // MP4/AAC is rejected here as UNSUPPORTED_AUDIO_FORMAT (passes MIME validation but is not
    // supported by the Fast Transcription API in a predictable way for MVP).
    private static void ValidateSupportedCompressedFormat(string normalisedMimeType)
    {
        if (normalisedMimeType is "audio/mpeg" or "audio/webm" or "audio/ogg")
            return;

        throw new ProviderException(
            ErrorCodes.UnsupportedAudioFormat,
            $"Audio format '{normalisedMimeType}' is not supported by the speech transcription provider. Supported formats: WAV, MP3, WebM/Opus, OGG/Opus.");
    }

    private static string NormaliseMimeType(string contentType) =>
        contentType.Split(';')[0].Trim().ToLowerInvariant();

    private static string ResolveLanguage(string? sourceLanguage)
    {
        if (string.IsNullOrWhiteSpace(sourceLanguage) || sourceLanguage == "auto")
            return "en-US";

        if (LanguageMap.TryGetValue(sourceLanguage, out var bcp47))
            return bcp47;

        // Already in full BCP-47 form (e.g., "en-US") — use as provided.
        return sourceLanguage;
    }
}
