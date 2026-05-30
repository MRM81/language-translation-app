namespace MyTranslationApp.Application.Interfaces;

public interface ISpeechToTextProvider
{
    string ProviderName { get; }

    Task<string> TranscribeAsync(
        Stream audioStream,
        string contentType,
        string? sourceLanguage,
        CancellationToken cancellationToken = default);
}
