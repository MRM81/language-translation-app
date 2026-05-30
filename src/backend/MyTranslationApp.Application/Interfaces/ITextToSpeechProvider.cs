namespace MyTranslationApp.Application.Interfaces;

// Future placeholder — not wired to any endpoint in Sprint 003.
public interface ITextToSpeechProvider
{
    Task<(byte[] AudioData, string ContentType)> SynthesizeAsync(
        string text,
        string language,
        string? voice,
        CancellationToken cancellationToken = default);
}
