using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MyTranslationApp.Application.DTOs;

namespace MyTranslationApp.Tests.Controllers;

public class TtsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TtsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(b => b.UseSetting("Translation:Provider", "Mock"))
            .CreateClient();
    }

    // --- POST /api/translate/tts ---

    [Fact]
    public async Task Tts_ValidRequest_Returns200()
    {
        var request = new TtsSynthesisRequestDto { Text = "Hello", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Tts_ValidRequest_ReturnsAudioContentType()
    {
        var request = new TtsSynthesisRequestDto { Text = "Hello", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var contentType = response.Content.Headers.ContentType?.MediaType;
        // Mock returns audio/wav; Azure returns audio/mpeg.
        Assert.NotNull(contentType);
        Assert.StartsWith("audio/", contentType);
    }

    [Fact]
    public async Task Tts_ValidRequest_ReturnsNonEmptyBody()
    {
        var request = new TtsSynthesisRequestDto { Text = "Hello", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(bytes);
    }

    [Fact]
    public async Task Tts_ValidRequest_CorrelationIdInResponseHeader()
    {
        var request = new TtsSynthesisRequestDto { Text = "Hello", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Correlation-ID"));
        var correlationId = response.Headers.GetValues("X-Correlation-ID").First();
        Assert.False(string.IsNullOrWhiteSpace(correlationId));
    }

    [Fact]
    public async Task Tts_EmptyText_Returns400ValidationError()
    {
        var request = new TtsSynthesisRequestDto { Text = "", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body.ErrorCode);
    }

    [Fact]
    public async Task Tts_EmptyLanguage_Returns400ValidationError()
    {
        var request = new TtsSynthesisRequestDto { Text = "Hello", Language = "" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body.ErrorCode);
    }

    [Fact]
    public async Task Tts_WhitespaceText_Returns400ValidationError()
    {
        var request = new TtsSynthesisRequestDto { Text = "   ", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.Equal("VALIDATION_ERROR", body.ErrorCode);
    }

    [Fact]
    public async Task Tts_EmptyText_ErrorResponseContainsCorrelationId()
    {
        var request = new TtsSynthesisRequestDto { Text = "", Language = "en" };

        var response = await _client.PostAsJsonAsync("/api/translate/tts", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.CorrelationId));
    }
}
