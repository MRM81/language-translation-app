using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Infrastructure.Providers;

public class MockSpeechToTextProvider : ISpeechToTextProvider
{
    public string ProviderName => "mock";

    public Task<string> TranscribeAsync(
        Stream audioStream,
        string contentType,
        string? sourceLanguage,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult("[mock transcript]");
    }
}
