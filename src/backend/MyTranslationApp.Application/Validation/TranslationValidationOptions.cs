namespace MyTranslationApp.Application.Validation;

public class TranslationValidationOptions
{
    public const string SectionName = "Translation";

    public int MaxTextCharacters { get; set; } = 5000;

    // Audio duration enforcement is deferred — cannot be measured reliably from raw stream
    // without an audio parsing library. File size limit is enforced as the nearest proxy.
    // TODO: Enforce MaxAudioSeconds in Sprint 005+ when audio parsing dependency is introduced.
    public int MaxAudioSeconds { get; set; } = 60;

    public long MaxAudioBytes { get; set; } = 10 * 1024 * 1024;

    public List<string> AllowedAudioMimeTypes { get; set; } = new()
    {
        "audio/webm",
        "audio/webm;codecs=opus",
        "audio/ogg",
        "audio/mp4",
        "audio/mpeg",
        "audio/wav"
    };

    public string Provider { get; set; } = "Mock";
}
