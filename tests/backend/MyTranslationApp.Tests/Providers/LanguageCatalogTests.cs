using MyTranslationApp.Infrastructure.Providers;

namespace MyTranslationApp.Tests.Providers;

public class LanguageCatalogTests
{
    private readonly StaticLanguageCatalogService _service = new();

    private async Task<IReadOnlyList<MyTranslationApp.Application.DTOs.LanguageOptionDto>> GetLanguagesAsync()
        => await _service.GetSupportedLanguagesAsync(CancellationToken.None);

    // --- Catalog size ---

    [Fact]
    public async Task Catalog_Contains37Languages()
    {
        var languages = await GetLanguagesAsync();
        Assert.Equal(37, languages.Count);
    }

    // --- All codes and names are non-empty ---

    [Fact]
    public async Task Catalog_AllCodesAreNonEmpty()
    {
        var languages = await GetLanguagesAsync();
        Assert.All(languages, l => Assert.False(string.IsNullOrWhiteSpace(l.Code)));
    }

    [Fact]
    public async Task Catalog_AllNamesAreNonEmpty()
    {
        var languages = await GetLanguagesAsync();
        Assert.All(languages, l => Assert.False(string.IsNullOrWhiteSpace(l.Name)));
    }

    // --- No duplicate codes or names ---

    [Fact]
    public async Task Catalog_CodesAreUnique()
    {
        var languages = await GetLanguagesAsync();
        var codes = languages.Select(l => l.Code).ToList();
        Assert.Equal(codes.Count, codes.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public async Task Catalog_NamesAreUnique()
    {
        var languages = await GetLanguagesAsync();
        var names = languages.Select(l => l.Name).ToList();
        Assert.Equal(names.Count, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    // --- Original 10 languages preserved ---

    [Theory]
    [InlineData("en", "English")]
    [InlineData("es", "Spanish")]
    [InlineData("fr", "French")]
    [InlineData("de", "German")]
    [InlineData("zh", "Chinese (Simplified)")]
    [InlineData("ja", "Japanese")]
    [InlineData("ar", "Arabic")]
    [InlineData("pt", "Portuguese")]
    [InlineData("it", "Italian")]
    [InlineData("ru", "Russian")]
    public async Task Catalog_ContainsOriginalLanguage(string code, string expectedName)
    {
        var languages = await GetLanguagesAsync();
        var entry = languages.SingleOrDefault(l => l.Code == code);
        Assert.NotNull(entry);
        Assert.Equal(expectedName, entry.Name);
    }

    // --- Sprint 009A acceptance criteria ---

    [Fact]
    public async Task Catalog_ContainsCzech()
    {
        var languages = await GetLanguagesAsync();
        Assert.Contains(languages, l => l.Code == "cs" && l.Name == "Czech");
    }

    [Fact]
    public async Task Catalog_ContainsSlovak()
    {
        var languages = await GetLanguagesAsync();
        Assert.Contains(languages, l => l.Code == "sk" && l.Name == "Slovak");
    }

    [Fact]
    public async Task Catalog_ContainsGerman()
    {
        var languages = await GetLanguagesAsync();
        Assert.Contains(languages, l => l.Code == "de" && l.Name == "German");
    }

    [Fact]
    public async Task Catalog_ContainsChineseSimplifiedWithClearLabel()
    {
        var languages = await GetLanguagesAsync();
        Assert.Contains(languages, l => l.Code == "zh" && l.Name == "Chinese (Simplified)");
    }

    [Fact]
    public async Task Catalog_ContainsChineseTraditional()
    {
        var languages = await GetLanguagesAsync();
        Assert.Contains(languages, l => l.Code == "zh-Hant" && l.Name == "Chinese (Traditional)");
    }

    // --- European expansion ---

    [Theory]
    [InlineData("nl", "Dutch")]
    [InlineData("pl", "Polish")]
    [InlineData("ro", "Romanian")]
    [InlineData("hu", "Hungarian")]
    [InlineData("el", "Greek")]
    [InlineData("sv", "Swedish")]
    [InlineData("da", "Danish")]
    [InlineData("nb", "Norwegian")]
    [InlineData("fi", "Finnish")]
    [InlineData("uk", "Ukrainian")]
    [InlineData("tr", "Turkish")]
    [InlineData("hr", "Croatian")]
    [InlineData("sr-Cyrl", "Serbian")]
    [InlineData("sl", "Slovenian")]
    [InlineData("bg", "Bulgarian")]
    [InlineData("lt", "Lithuanian")]
    [InlineData("lv", "Latvian")]
    [InlineData("et", "Estonian")]
    public async Task Catalog_ContainsEuropeanExpansionLanguage(string code, string expectedName)
    {
        var languages = await GetLanguagesAsync();
        var entry = languages.SingleOrDefault(l => l.Code == code);
        Assert.NotNull(entry);
        Assert.Equal(expectedName, entry.Name);
    }

    // --- Global expansion ---

    [Theory]
    [InlineData("ko", "Korean")]
    [InlineData("vi", "Vietnamese")]
    [InlineData("th", "Thai")]
    [InlineData("id", "Indonesian")]
    [InlineData("ms", "Malay")]
    [InlineData("hi", "Hindi")]
    public async Task Catalog_ContainsGlobalExpansionLanguage(string code, string expectedName)
    {
        var languages = await GetLanguagesAsync();
        var entry = languages.SingleOrDefault(l => l.Code == code);
        Assert.NotNull(entry);
        Assert.Equal(expectedName, entry.Name);
    }
}
