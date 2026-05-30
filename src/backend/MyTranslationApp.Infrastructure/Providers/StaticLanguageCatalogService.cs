using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Infrastructure.Providers;

public class StaticLanguageCatalogService : ILanguageCatalogService
{
    private static readonly IReadOnlyList<LanguageOptionDto> SupportedLanguages = new[]
    {
        new LanguageOptionDto { Code = "ar",     Name = "Arabic" },
        new LanguageOptionDto { Code = "bg",     Name = "Bulgarian" },
        new LanguageOptionDto { Code = "hr",     Name = "Croatian" },
        new LanguageOptionDto { Code = "cs",     Name = "Czech" },
        new LanguageOptionDto { Code = "da",     Name = "Danish" },
        new LanguageOptionDto { Code = "nl",     Name = "Dutch" },
        new LanguageOptionDto { Code = "en",     Name = "English" },
        new LanguageOptionDto { Code = "et",     Name = "Estonian" },
        new LanguageOptionDto { Code = "fi",     Name = "Finnish" },
        new LanguageOptionDto { Code = "fr",     Name = "French" },
        new LanguageOptionDto { Code = "de",     Name = "German" },
        new LanguageOptionDto { Code = "el",     Name = "Greek" },
        new LanguageOptionDto { Code = "hi",     Name = "Hindi" },
        new LanguageOptionDto { Code = "hu",     Name = "Hungarian" },
        new LanguageOptionDto { Code = "id",     Name = "Indonesian" },
        new LanguageOptionDto { Code = "it",     Name = "Italian" },
        new LanguageOptionDto { Code = "ja",     Name = "Japanese" },
        new LanguageOptionDto { Code = "ko",     Name = "Korean" },
        new LanguageOptionDto { Code = "lv",     Name = "Latvian" },
        new LanguageOptionDto { Code = "lt",     Name = "Lithuanian" },
        new LanguageOptionDto { Code = "ms",     Name = "Malay" },
        new LanguageOptionDto { Code = "nb",     Name = "Norwegian" },
        new LanguageOptionDto { Code = "pl",     Name = "Polish" },
        new LanguageOptionDto { Code = "pt",     Name = "Portuguese" },
        new LanguageOptionDto { Code = "ro",     Name = "Romanian" },
        new LanguageOptionDto { Code = "ru",     Name = "Russian" },
        new LanguageOptionDto { Code = "sr-Cyrl", Name = "Serbian" },
        new LanguageOptionDto { Code = "sk",     Name = "Slovak" },
        new LanguageOptionDto { Code = "sl",     Name = "Slovenian" },
        new LanguageOptionDto { Code = "es",     Name = "Spanish" },
        new LanguageOptionDto { Code = "sv",     Name = "Swedish" },
        new LanguageOptionDto { Code = "th",     Name = "Thai" },
        new LanguageOptionDto { Code = "tr",     Name = "Turkish" },
        new LanguageOptionDto { Code = "uk",     Name = "Ukrainian" },
        new LanguageOptionDto { Code = "vi",     Name = "Vietnamese" },
        new LanguageOptionDto { Code = "zh",     Name = "Chinese (Simplified)" },
        new LanguageOptionDto { Code = "zh-Hant", Name = "Chinese (Traditional)" },
    };

    public Task<IReadOnlyList<LanguageOptionDto>> GetSupportedLanguagesAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SupportedLanguages);
    }
}
