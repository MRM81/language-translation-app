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
    // Covers all 37 languages in the static language catalog.
    private static readonly Dictionary<string, string> VoiceMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "ar",      "ar-SA-ZariyahNeural"     },
            { "bg",      "bg-BG-KalinaNeural"       },
            { "hr",      "hr-HR-GabrijelaNeural"    },
            { "cs",      "cs-CZ-VlastaNeural"       },
            { "da",      "da-DK-ChristelNeural"     },
            { "nl",      "nl-NL-FennaNeural"        },
            { "en",      "en-US-JennyNeural"        },
            { "et",      "et-EE-AnuNeural"          },
            { "fi",      "fi-FI-NooraNeural"        },
            { "fr",      "fr-FR-DeniseNeural"       },
            { "de",      "de-DE-KatjaNeural"        },
            { "el",      "el-GR-AthinaNeural"       },
            { "hi",      "hi-IN-SwaraNeural"        },
            { "hu",      "hu-HU-NoemiNeural"        },
            { "id",      "id-ID-GadisNeural"        },
            { "it",      "it-IT-ElsaNeural"         },
            { "ja",      "ja-JP-NanamiNeural"       },
            { "ko",      "ko-KR-SunHiNeural"        },
            { "lv",      "lv-LV-EveritaNeural"      },
            { "lt",      "lt-LT-OnaNeural"          },
            { "ms",      "ms-MY-YasminNeural"       },
            { "nb",      "nb-NO-PernilleNeural"     },
            { "pl",      "pl-PL-AgnieszkaNeural"    },
            { "pt",      "pt-BR-FranciscaNeural"    },
            { "ro",      "ro-RO-AlinaNeural"        },
            { "ru",      "ru-RU-SvetlanaNeural"     },
            { "sr-Cyrl", "sr-RS-SophieNeural"       },
            { "sk",      "sk-SK-ViktoriaNeural"     },
            { "sl",      "sl-SI-PetraNeural"        },
            { "es",      "es-ES-ElviraNeural"       },
            { "sv",      "sv-SE-SofieNeural"        },
            { "th",      "th-TH-PremwadeeNeural"    },
            { "tr",      "tr-TR-EmelNeural"         },
            { "uk",      "uk-UA-PolinaNeural"       },
            { "vi",      "vi-VN-HoaiMyNeural"       },
            { "zh",      "zh-CN-XiaoxiaoNeural"     },
            { "zh-Hant", "zh-TW-HsiaoChenNeural"    },
        };

    // Maps short codes to full BCP-47 locales for the fallback path.
    private static readonly Dictionary<string, string> LocaleMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "ar",      "ar-SA" }, { "bg",      "bg-BG" }, { "hr",      "hr-HR" },
            { "cs",      "cs-CZ" }, { "da",      "da-DK" }, { "nl",      "nl-NL" },
            { "en",      "en-US" }, { "et",      "et-EE" }, { "fi",      "fi-FI" },
            { "fr",      "fr-FR" }, { "de",      "de-DE" }, { "el",      "el-GR" },
            { "hi",      "hi-IN" }, { "hu",      "hu-HU" }, { "id",      "id-ID" },
            { "it",      "it-IT" }, { "ja",      "ja-JP" }, { "ko",      "ko-KR" },
            { "lv",      "lv-LV" }, { "lt",      "lt-LT" }, { "ms",      "ms-MY" },
            { "nb",      "nb-NO" }, { "pl",      "pl-PL" }, { "pt",      "pt-BR" },
            { "ro",      "ro-RO" }, { "ru",      "ru-RU" }, { "sr-Cyrl", "sr-RS" },
            { "sk",      "sk-SK" }, { "sl",      "sl-SI" }, { "es",      "es-ES" },
            { "sv",      "sv-SE" }, { "th",      "th-TH" }, { "tr",      "tr-TR" },
            { "uk",      "uk-UA" }, { "vi",      "vi-VN" }, { "zh",      "zh-CN" },
            { "zh-Hant", "zh-TW" },
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
