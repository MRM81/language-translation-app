namespace MyTranslationApp.Application.Interfaces;

public interface ITextTranslationProvider
{
    string ProviderName { get; }

    Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        string? sourceLanguage,
        CancellationToken cancellationToken = default);
}
