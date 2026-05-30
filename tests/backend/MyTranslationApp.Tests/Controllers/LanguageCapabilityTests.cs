using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTranslationApp.Application.DTOs;

namespace MyTranslationApp.Tests.Controllers;

public class LanguageCapabilityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LanguageCapabilityTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(b => b.UseSetting("Translation:Provider", "Mock"))
            .CreateClient();
    }

    // --- Capability fields present in API response ---

    [Fact]
    public async Task GetLanguages_AllLanguagesHaveSupportsTextTranslation()
    {
        var body = await GetLanguageListAsync();
        Assert.All(body.Languages, l => Assert.True(l.SupportsTextTranslation));
    }

    [Fact]
    public async Task GetLanguages_AllLanguagesHaveSupportsSpeechToText()
    {
        var body = await GetLanguageListAsync();
        Assert.All(body.Languages, l => Assert.True(l.SupportsSpeechToText));
    }

    [Fact]
    public async Task GetLanguages_AllLanguagesHaveSupportsTextToSpeech()
    {
        var body = await GetLanguageListAsync();
        Assert.All(body.Languages, l => Assert.True(l.SupportsTextToSpeech));
    }

    // --- Sprint 009A acceptance criteria languages retain full capability ---

    [Theory]
    [InlineData("cs")]  // Czech
    [InlineData("sk")]  // Slovak
    [InlineData("de")]  // German
    [InlineData("zh")]  // Chinese (Simplified)
    [InlineData("zh-Hant")] // Chinese (Traditional)
    [InlineData("nl")]  // Dutch
    [InlineData("pl")]  // Polish
    [InlineData("ko")]  // Korean
    [InlineData("hi")]  // Hindi
    public async Task GetLanguages_SpecificLanguage_HasAllCapabilities(string code)
    {
        var body = await GetLanguageListAsync();
        var lang = body.Languages.SingleOrDefault(l => l.Code == code);
        Assert.NotNull(lang);
        Assert.True(lang.SupportsTextTranslation);
        Assert.True(lang.SupportsSpeechToText);
        Assert.True(lang.SupportsTextToSpeech);
    }

    // --- Existing code and name fields not broken ---

    [Fact]
    public async Task GetLanguages_CapabilityFields_DoNotBreakExistingCodeAndName()
    {
        var body = await GetLanguageListAsync();
        Assert.All(body.Languages, l =>
        {
            Assert.False(string.IsNullOrWhiteSpace(l.Code));
            Assert.False(string.IsNullOrWhiteSpace(l.Name));
        });
    }

    // --- All 37 languages have capability metadata ---

    [Fact]
    public async Task GetLanguages_Returns37LanguagesAllWithCapabilityMetadata()
    {
        var body = await GetLanguageListAsync();
        Assert.Equal(37, body.Languages.Count);
        Assert.All(body.Languages, l =>
        {
            Assert.True(l.SupportsTextTranslation, $"{l.Code} missing SupportsTextTranslation");
            Assert.True(l.SupportsSpeechToText, $"{l.Code} missing SupportsSpeechToText");
            Assert.True(l.SupportsTextToSpeech, $"{l.Code} missing SupportsTextToSpeech");
        });
    }

    private async Task<LanguageListResponseDto> GetLanguageListAsync()
    {
        var response = await _client.GetAsync("/api/languages");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LanguageListResponseDto>();
        Assert.NotNull(body);
        return body;
    }
}
