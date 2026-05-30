using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Infrastructure.Providers;

public class StaticLanguageCatalogService : ILanguageCatalogService
{
    private static readonly IReadOnlyList<LanguageOptionDto> SupportedLanguages = new[]
    {
        new LanguageOptionDto { Code = "en", Name = "English" },
        new LanguageOptionDto { Code = "es", Name = "Spanish" },
        new LanguageOptionDto { Code = "fr", Name = "French" },
        new LanguageOptionDto { Code = "de", Name = "German" },
        new LanguageOptionDto { Code = "zh", Name = "Chinese (Simplified)" },
        new LanguageOptionDto { Code = "ja", Name = "Japanese" },
        new LanguageOptionDto { Code = "ar", Name = "Arabic" },
        new LanguageOptionDto { Code = "pt", Name = "Portuguese" },
        new LanguageOptionDto { Code = "it", Name = "Italian" },
        new LanguageOptionDto { Code = "ru", Name = "Russian" }
    };

    public Task<IReadOnlyList<LanguageOptionDto>> GetSupportedLanguagesAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SupportedLanguages);
    }
}
