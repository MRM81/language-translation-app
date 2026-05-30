using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MyTranslationApp.Tests.Controllers;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(b => b.UseSetting("Translation:Provider", "Mock"))
            .CreateClient();
    }

    [Fact]
    public async Task Health_Returns200()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/health");
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("healthy", doc.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Health_ReturnsJson()
    {
        var response = await _client.GetAsync("/health");
        Assert.Contains("application/json", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task Health_IsAvailableInMockMode()
    {
        // Health endpoint must not require Azure credentials — works in any provider mode.
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
