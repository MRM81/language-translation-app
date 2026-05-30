using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MyTranslationApp.Application.Exceptions;
using MyTranslationApp.Application.Validation;
using MyTranslationApp.Infrastructure.Configuration;
using MyTranslationApp.Infrastructure.Providers.Azure;

namespace MyTranslationApp.Tests.Providers;

public class AzureSpeechProviderRoutingTests
{
    private const string FakeKey = "fake-key-for-testing";
    private const string FakeRegion = "eastus";
    private const string FakeEndpoint = "https://fake-resource.cognitiveservices.azure.com";

    private static AzureSpeechToTextProvider CreateProvider(
        IHttpClientFactory factory,
        string endpoint = FakeEndpoint)
    {
        var options = Options.Create(new AzureSpeechOptions
        {
            Key = FakeKey,
            Region = FakeRegion,
            Endpoint = endpoint
        });
        return new AzureSpeechToTextProvider(
            options,
            factory,
            NullLogger<AzureSpeechToTextProvider>.Instance);
    }

    private static (FakeHttpMessageHandler Handler, IHttpClientFactory Factory) MakeFactory(
        HttpResponseMessage response)
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(response));
        return (handler, new FakeHttpClientFactory(handler));
    }

    private static (FakeHttpMessageHandler Handler, IHttpClientFactory Factory) MakeFactory(
        Func<HttpRequestMessage, Task<HttpResponseMessage>> respond)
    {
        var handler = new FakeHttpMessageHandler(respond);
        return (handler, new FakeHttpClientFactory(handler));
    }

    private static HttpResponseMessage OkResponse(string text) =>
        new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                $"{{\"durationMilliseconds\":1000," +
                $"\"combinedPhrases\":[{{\"channel\":0,\"text\":\"{text}\",\"locale\":\"en-US\"}}]," +
                $"\"phrases\":[]}}",
                Encoding.UTF8, "application/json")
        };

    // ── Routing: WAV stays on SDK path ──────────────────────────────────────

    [Fact]
    public async Task WavInput_DoesNotCallHttpClient()
    {
        // WAV input takes the SDK path (which throws without real Azure credentials
        // and fake audio bytes — that is expected). The important assertion is that
        // the HTTP client is never invoked.
        var (handler, factory) = MakeFactory(
            _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        var provider = CreateProvider(factory);

        await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0x52, 0x49, 0x46, 0x46 }),
                "audio/wav", "en", CancellationToken.None));

        Assert.Empty(handler.Requests);
    }

    // ── Routing: compressed formats use Fast Transcription REST ─────────────

    [Fact]
    public async Task Mp3Input_CallsTranscribeEndpoint()
    {
        var (handler, factory) = MakeFactory(OkResponse("hello mp3"));
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0xFF, 0xFB }),
            "audio/mpeg", "en", CancellationToken.None);

        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task WebmInput_CallsTranscribeEndpoint()
    {
        var (handler, factory) = MakeFactory(OkResponse("hello webm"));
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0x1A, 0x45 }),
            "audio/webm", "en", CancellationToken.None);

        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task WebmWithCodecsSuffix_NormalisedAndCallsTranscribeEndpoint()
    {
        // Browser sends audio/webm;codecs=opus — provider must normalise to audio/webm
        // before routing and set that as the audio part Content-Type.
        // Content-Type is captured inside the handler while the request is still live.
        string? capturedAudioContentType = null;
        var (handler, factory) = MakeFactory(async req =>
        {
            if (req.Content is MultipartFormDataContent multipart)
            {
                foreach (var part in multipart)
                {
                    if (part.Headers.ContentDisposition?.Name?.Trim('"') == "audio")
                        capturedAudioContentType = part.Headers.ContentType?.MediaType;
                }
            }
            return await Task.FromResult(OkResponse("hello webm-opus"));
        });
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0x1A, 0x45 }),
            "audio/webm;codecs=opus", "en", CancellationToken.None);

        Assert.Single(handler.Requests);
        Assert.Equal("audio/webm", capturedAudioContentType);
    }

    [Fact]
    public async Task OggInput_CallsTranscribeEndpoint()
    {
        var (handler, factory) = MakeFactory(OkResponse("hello ogg"));
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0x4F, 0x67, 0x67, 0x53 }),
            "audio/ogg", "en", CancellationToken.None);

        Assert.Single(handler.Requests);
    }

    // ── Request shape ────────────────────────────────────────────────────────

    [Fact]
    public async Task Request_IncludesSubscriptionKeyHeader()
    {
        var (handler, factory) = MakeFactory(OkResponse("test"));
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0xFF, 0xFB }),
            "audio/mpeg", "en", CancellationToken.None);

        var req = handler.Requests[0];
        Assert.True(req.Headers.Contains("Ocp-Apim-Subscription-Key"));
        Assert.Equal(FakeKey, req.Headers.GetValues("Ocp-Apim-Subscription-Key").First());
    }

    [Fact]
    public async Task Request_UrlContainsEndpointAndApiVersion()
    {
        var (handler, factory) = MakeFactory(OkResponse("test"));
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0xFF, 0xFB }),
            "audio/mpeg", "en", CancellationToken.None);

        var url = handler.Requests[0].RequestUri!.ToString();
        Assert.Contains("fake-resource.cognitiveservices.azure.com", url);
        Assert.Contains("speechtotext/transcriptions:transcribe", url);
        Assert.Contains("api-version=2025-10-15", url);
    }

    [Fact]
    public async Task Request_DefinitionPartContainsResolvedLocale()
    {
        string? capturedDefinition = null;
        var (_, factory) = MakeFactory(async req =>
        {
            if (req.Content is MultipartFormDataContent multipart)
            {
                foreach (var part in multipart)
                {
                    if (part.Headers.ContentDisposition?.Name?.Trim('"') == "definition")
                        capturedDefinition = await part.ReadAsStringAsync();
                }
            }
            return OkResponse("test");
        });
        var provider = CreateProvider(factory);

        await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0xFF, 0xFB }),
            "audio/mpeg", "en", CancellationToken.None);

        Assert.NotNull(capturedDefinition);
        Assert.Contains("en-US", capturedDefinition);
    }

    // ── Response parsing ─────────���───────────────────────────────────────────

    [Fact]
    public async Task TranscribeResponse_Success_ReturnsCombinedPhrasesText()
    {
        var (_, factory) = MakeFactory(OkResponse("Good afternoon."));
        var provider = CreateProvider(factory);

        var result = await provider.TranscribeAsync(
            new MemoryStream(new byte[] { 0xFF, 0xFB }),
            "audio/mpeg", "en", CancellationToken.None);

        Assert.Equal("Good afternoon.", result);
    }

    [Fact]
    public async Task TranscribeResponse_EmptyCombinedPhrases_ThrowsTranscriptionFailed()
    {
        var (_, factory) = MakeFactory(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "{\"durationMilliseconds\":0,\"combinedPhrases\":[],\"phrases\":[]}",
                Encoding.UTF8, "application/json")
        });
        var provider = CreateProvider(factory);

        var ex = await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0xFF, 0xFB }),
                "audio/mpeg", "en", CancellationToken.None));

        Assert.Equal(ErrorCodes.TranscriptionFailed, ex.ErrorCode);
    }

    [Fact]
    public async Task TranscribeResponse_EmptyText_ThrowsTranscriptionFailed()
    {
        var (_, factory) = MakeFactory(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                "{\"durationMilliseconds\":100,\"combinedPhrases\":[{\"channel\":0,\"text\":\"\",\"locale\":\"en-US\"}],\"phrases\":[]}",
                Encoding.UTF8, "application/json")
        });
        var provider = CreateProvider(factory);

        var ex = await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0xFF, 0xFB }),
                "audio/mpeg", "en", CancellationToken.None));

        Assert.Equal(ErrorCodes.TranscriptionFailed, ex.ErrorCode);
    }

    [Fact]
    public async Task TranscribeResponse_Http401_ThrowsProviderError()
    {
        var (_, factory) = MakeFactory(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var provider = CreateProvider(factory);

        var ex = await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0xFF, 0xFB }),
                "audio/mpeg", "en", CancellationToken.None));

        Assert.Equal(ErrorCodes.ProviderError, ex.ErrorCode);
    }

    [Fact]
    public async Task TranscribeResponse_Http400_ThrowsProviderError()
    {
        var (_, factory) = MakeFactory(new HttpResponseMessage(HttpStatusCode.BadRequest));
        var provider = CreateProvider(factory);

        var ex = await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0xFF, 0xFB }),
                "audio/mpeg", "en", CancellationToken.None));

        Assert.Equal(ErrorCodes.ProviderError, ex.ErrorCode);
    }

    [Fact]
    public async Task TranscribeResponse_NetworkFailure_ThrowsProviderError()
    {
        var handler = new FakeHttpMessageHandler(
            _ => throw new HttpRequestException("Simulated network failure"));
        var provider = CreateProvider(new FakeHttpClientFactory(handler));

        var ex = await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0xFF, 0xFB }),
                "audio/mpeg", "en", CancellationToken.None));

        Assert.Equal(ErrorCodes.ProviderError, ex.ErrorCode);
    }

    // ── Unsupported format ───────────────────────────────────────────────────

    [Fact]
    public async Task Mp4Input_ThrowsUnsupportedAudioFormat_WithoutCallingHttpClient()
    {
        // audio/mp4 passes MIME validation but is not supported by the Fast Transcription path.
        var (handler, factory) = MakeFactory(OkResponse("should not reach"));
        var provider = CreateProvider(factory);

        var ex = await Assert.ThrowsAsync<ProviderException>(async () =>
            await provider.TranscribeAsync(
                new MemoryStream(new byte[] { 0x00 }),
                "audio/mp4", "en", CancellationToken.None));

        Assert.Equal(ErrorCodes.UnsupportedAudioFormat, ex.ErrorCode);
        Assert.Empty(handler.Requests);
    }

    // ── Startup config: missing Endpoint fails at startup ────────────────────

    [Fact]
    public void AzureProvider_MissingSpeechEndpoint_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
                builder.UseSetting("AzureSpeech:Endpoint", ""); // empty overrides User Secrets
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _respond;
        public List<HttpRequestMessage> Requests { get; } = new();

        public FakeHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond)
            => _respond = respond;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return await _respond(request);
        }
    }

    private sealed class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        public FakeHttpClientFactory(HttpMessageHandler handler) => _handler = handler;
        public HttpClient CreateClient(string name) => new HttpClient(_handler);
    }
}
