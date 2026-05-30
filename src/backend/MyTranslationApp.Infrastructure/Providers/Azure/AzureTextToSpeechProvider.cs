using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTranslationApp.Application.Exceptions;
using MyTranslationApp.Application.Interfaces;
using MyTranslationApp.Application.Validation;
using MyTranslationApp.Infrastructure.Configuration;

namespace MyTranslationApp.Infrastructure.Providers.Azure;

public class AzureTextToSpeechProvider : ITextToSpeechProvider
{
    private readonly IOptions<AzureSpeechOptions> _options;
    private readonly ILogger<AzureTextToSpeechProvider> _logger;

    // Maps app language codes to Azure Neural voice names.
    // Covers the 10 languages in the static language catalog.
    private static readonly Dictionary<string, string> VoiceMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "en",    "en-US-JennyNeural"      },
            { "es",    "es-ES-ElviraNeural"     },
            { "fr",    "fr-FR-DeniseNeural"     },
            { "de",    "de-DE-KatjaNeural"      },
            { "zh",    "zh-CN-XiaoxiaoNeural"   },
            { "ja",    "ja-JP-NanamiNeural"     },
            { "ar",    "ar-SA-ZariyahNeural"    },
            { "pt",    "pt-BR-FranciscaNeural"  },
            { "it",    "it-IT-ElsaNeural"       },
            { "ru",    "ru-RU-SvetlanaNeural"   },
        };

    // Maps short codes to full BCP-47 locales for the fallback path.
    private static readonly Dictionary<string, string> LocaleMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "en", "en-US" }, { "es", "es-ES" }, { "fr", "fr-FR" },
            { "de", "de-DE" }, { "zh", "zh-CN" }, { "ja", "ja-JP" },
            { "ar", "ar-SA" }, { "pt", "pt-BR" }, { "it", "it-IT" },
            { "ru", "ru-RU" },
        };

    public AzureTextToSpeechProvider(
        IOptions<AzureSpeechOptions> options,
        ILogger<AzureTextToSpeechProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<(byte[] AudioData, string ContentType)> SynthesizeAsync(
        string text,
        string language,
        string? voice,
        CancellationToken cancellationToken = default)
    {
        var opts = _options.Value;

        var speechConfig = SpeechConfig.FromSubscription(opts.Key, opts.Region);
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz96KBitRateMonoMp3);

        // Prefer the explicit voice override, then mapped voice, then language-based fallback.
        if (!string.IsNullOrWhiteSpace(voice))
        {
            speechConfig.SpeechSynthesisVoiceName = voice;
        }
        else if (VoiceMap.TryGetValue(language, out var mappedVoice))
        {
            speechConfig.SpeechSynthesisVoiceName = mappedVoice;
        }
        else
        {
            // Fallback: resolve short code to locale then let Azure pick its default voice.
            var locale = LocaleMap.TryGetValue(language, out var mapped) ? mapped : language;
            speechConfig.SpeechSynthesisLanguage = locale;
            _logger.LogInformation(
                "TTS voice not mapped for language '{Language}'. Using Azure default for locale '{Locale}'.",
                language, locale);
        }

        _logger.LogInformation("Calling Azure Speech TTS. Language: {Language}", language);

        try
        {
            using var synthesizer = new SpeechSynthesizer(speechConfig, audioConfig: null);
            using var result = await synthesizer.SpeakTextAsync(text);

            return result.Reason switch
            {
                ResultReason.SynthesizingAudioCompleted =>
                    (result.AudioData, "audio/mpeg"),

                ResultReason.Canceled =>
                    HandleCancellation(result),

                _ => throw new ProviderException(
                    ErrorCodes.ProviderError,
                    "The speech synthesis service returned an unexpected result.")
            };
        }
        catch (ProviderException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                "Azure Speech TTS exception. ExceptionType: {ExceptionType}",
                ex.GetType().Name);
            throw new ProviderException(
                ErrorCodes.ProviderError,
                "The speech synthesis service is temporarily unavailable. Please try again shortly.",
                ex);
        }
    }

    private static (byte[] AudioData, string ContentType) HandleCancellation(SpeechSynthesisResult result)
    {
        var details = SpeechSynthesisCancellationDetails.FromResult(result);
        if (details.Reason == CancellationReason.Error)
            throw new ProviderException(
                ErrorCodes.ProviderError,
                "The speech synthesis service is temporarily unavailable. Please try again shortly.");

        throw new ProviderException(
            ErrorCodes.ProviderError,
            "Speech synthesis was cancelled. Please try again.");
    }
}
