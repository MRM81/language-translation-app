using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTranslationApp.Application.DTOs;

namespace MyTranslationApp.Tests.Controllers;

public class TranslationControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TranslationControllerTests(WebApplicationFactory<Program> factory)
    {
        // Pin provider to Mock regardless of User Secrets or environment overrides
        // so controller tests are environment-independent.
        _client = factory
            .WithWebHostBuilder(b => b.UseSetting("Translation:Provider", "Mock"))
            .CreateClient();
    }

    // --- POST /api/translate/text ---

    [Fact]
    public async Task TranslateText_ValidRequest_Returns200WithMockTranslation()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            SourceLanguage = "en",
            TargetLanguage = "es"
        };

        var response = await _client.PostAsJsonAsync("/api/translate/text", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TextTranslationResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("[mock-es] Hello", body.TranslatedText);
        Assert.Equal("mock", body.Provider);
    }

    [Fact]
    public async Task TranslateText_ValidRequest_ResponseContainsCorrelationId()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            TargetLanguage = "es"
        };

        var response = await _client.PostAsJsonAsync("/api/translate/text", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TextTranslationResponseDto>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.CorrelationId));
    }

    [Fact]
    public async Task TranslateText_IncomingCorrelationId_IsReusedInResponse()
    {
        var expectedCorrelationId = "test-correlation-123";
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            TargetLanguage = "es"
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/translate/text")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Add("X-Correlation-ID", expectedCorrelationId);

        var response = await _client.SendAsync(httpRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TextTranslationResponseDto>();
        Assert.NotNull(body);
        Assert.Equal(expectedCorrelationId, body.CorrelationId);
        Assert.True(response.Headers.Contains("X-Correlation-ID"));
        Assert.Equal(expectedCorrelationId, response.Headers.GetValues("X-Correlation-ID").First());
    }

    [Fact]
    public async Task TranslateText_EmptySourceText_Returns400WithCorrelationId()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "",
            TargetLanguage = "es"
        };

        var response = await _client.PostAsJsonAsync("/api/translate/text", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.CorrelationId));
        Assert.False(string.IsNullOrWhiteSpace(body.ErrorCode));
    }

    [Fact]
    public async Task TranslateText_TextExceedsLimit_Returns400TextTooLong()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = new string('a', 5001),
            TargetLanguage = "es"
        };

        var response = await _client.PostAsJsonAsync("/api/translate/text", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("TEXT_TOO_LONG", body.ErrorCode);
    }

    [Fact]
    public async Task TranslateText_MissingTargetLanguage_Returns400()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            TargetLanguage = ""
        };

        var response = await _client.PostAsJsonAsync("/api/translate/text", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body.ErrorCode);
    }

    // --- POST /api/translate/audio ---

    [Fact]
    public async Task TranslateAudio_ValidRequest_Returns200WithMockTranscriptAndTranslation()
    {
        using var content = new MultipartFormDataContent();
        var audioBytes = new byte[] { 0x1A, 0x45, 0xDF, 0xA3 };
        var audioContent = new ByteArrayContent(audioBytes);
        audioContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/webm");
        content.Add(audioContent, "audio", "recording.webm");
        content.Add(new StringContent("es"), "targetLanguage");

        var response = await _client.PostAsync("/api/translate/audio", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AudioTranslationResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("[mock transcript]", body.TranscribedText);
        Assert.Equal("[mock-es] [mock transcript]", body.TranslatedText);
        Assert.Equal("mock", body.Provider);
        Assert.False(string.IsNullOrWhiteSpace(body.CorrelationId));
    }

    [Fact]
    public async Task TranslateAudio_UnsupportedMimeType_Returns400UnsupportedAudioFormat()
    {
        using var content = new MultipartFormDataContent();
        var audioContent = new ByteArrayContent(new byte[] { 0x01 });
        audioContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");
        content.Add(audioContent, "audio", "recording.mp4");
        content.Add(new StringContent("es"), "targetLanguage");

        var response = await _client.PostAsync("/api/translate/audio", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("UNSUPPORTED_AUDIO_FORMAT", body.ErrorCode);
        Assert.False(string.IsNullOrWhiteSpace(body.CorrelationId));
    }

    [Fact]
    public async Task TranslateAudio_MissingAudioFile_Returns400()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("es"), "targetLanguage");

        var response = await _client.PostAsync("/api/translate/audio", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body.ErrorCode);
    }

    // --- GET /api/languages ---

    [Fact]
    public async Task GetLanguages_Returns200WithLanguageList()
    {
        var response = await _client.GetAsync("/api/languages");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LanguageListResponseDto>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Languages);
        Assert.Contains(body.Languages, l => l.Code == "en");
        Assert.All(body.Languages, l =>
        {
            Assert.False(string.IsNullOrWhiteSpace(l.Code));
            Assert.False(string.IsNullOrWhiteSpace(l.Name));
        });
    }
}
