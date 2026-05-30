using MyTranslationApp.Application.DTOs;

namespace MyTranslationApp.Application.Interfaces;

public interface ILanguageCatalogService
{
    Task<IReadOnlyList<LanguageOptionDto>> GetSupportedLanguagesAsync(
        CancellationToken cancellationToken = default);
}
