namespace MyTranslationApp.Application.Validation;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string TextTooLong = "TEXT_TOO_LONG";
    public const string AudioTooLarge = "AUDIO_TOO_LARGE";
    public const string AudioTooLong = "AUDIO_TOO_LONG";
    public const string UnsupportedAudioFormat = "UNSUPPORTED_AUDIO_FORMAT";
    public const string TranscriptionFailed = "TRANSCRIPTION_FAILED";
    public const string TranslationFailed = "TRANSLATION_FAILED";
    public const string ProviderError = "PROVIDER_ERROR";
    public const string ProviderTimeout = "PROVIDER_TIMEOUT";
    public const string ProviderUnavailable = "PROVIDER_UNAVAILABLE";
    public const string InternalError = "INTERNAL_ERROR";
}
