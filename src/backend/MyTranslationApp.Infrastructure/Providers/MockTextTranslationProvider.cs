using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Infrastructure.Providers;

public class MockTextTranslationProvider : ITextTranslationProvider
{
    public string ProviderName => "mock";

    public Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        string? sourceLanguage,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"[mock-{targetLanguage}] {text}");
    }
}
