using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MyTranslationApp.Api.Middleware;
using MyTranslationApp.Application.Interfaces;
using MyTranslationApp.Application.Services;
using MyTranslationApp.Application.Validation;
using MyTranslationApp.Infrastructure.Configuration;
using MyTranslationApp.Infrastructure.Providers;
using MyTranslationApp.Infrastructure.Providers.Azure;

var builder = WebApplication.CreateBuilder(args);

// Local development CORS — allows the Vite dev server to call the backend without browser blocking.
// This is intentionally scoped to the local dev origin only and is not a production CORS policy.
const string localDevCorsPolicy = "LocalDevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(localDevCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Server-level size limits — prevents large uploads from reaching memory before logical validation.
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize =
        builder.Configuration.GetValue<long>("Translation:MaxAudioBytes", 10 * 1024 * 1024);
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit =
        builder.Configuration.GetValue<long>("Translation:MaxAudioBytes", 10 * 1024 * 1024);
});

// Bind validation configuration from appsettings.json "Translation" section.
builder.Services.Configure<TranslationValidationOptions>(
    builder.Configuration.GetSection(TranslationValidationOptions.SectionName));

// HttpClient for provider REST calls (Fast Transcription API).
builder.Services.AddHttpClient();

// Application services
builder.Services.AddScoped<TranslationService>();
builder.Services.AddScoped<TextToSpeechService>();
builder.Services.AddScoped<TranslationRequestValidator>();

// Provider selection — driven by Translation:Provider configuration key.
// Startup validation runs for Azure mode only; Mock mode requires no credentials.
var providerName = builder.Configuration.GetValue<string>("Translation:Provider") ?? "Mock";

switch (providerName.Trim().ToUpperInvariant())
{
    case "MOCK":
        builder.Services.AddScoped<ITextTranslationProvider, MockTextTranslationProvider>();
        builder.Services.AddScoped<ISpeechToTextProvider, MockSpeechToTextProvider>();
        builder.Services.AddScoped<ITextToSpeechProvider, MockTextToSpeechProvider>();
        break;

    case "AZURE":
        var translatorSection = builder.Configuration.GetSection(AzureTranslationOptions.SectionName);
        var speechSection = builder.Configuration.GetSection(AzureSpeechOptions.SectionName);

        var translatorOpts = translatorSection.Get<AzureTranslationOptions>() ?? new AzureTranslationOptions();
        var speechOpts = speechSection.Get<AzureSpeechOptions>() ?? new AzureSpeechOptions();

        if (!translatorOpts.IsConfigured)
            throw new InvalidOperationException(
                "Azure Translator is not configured. " +
                "Set AzureTranslator:Key and AzureTranslator:Region via User Secrets or environment variables. " +
                "See src/backend/README.md for setup instructions.");

        if (!speechOpts.IsConfigured)
            throw new InvalidOperationException(
                "Azure Speech is not configured. " +
                "Set AzureSpeech:Key, AzureSpeech:Region, and AzureSpeech:Endpoint via User Secrets or environment variables. " +
                "Endpoint example: https://your-resource-name.cognitiveservices.azure.com " +
                "See src/backend/README.md for setup instructions.");

        builder.Services.Configure<AzureTranslationOptions>(translatorSection);
        builder.Services.Configure<AzureSpeechOptions>(speechSection);
        builder.Services.AddScoped<ITextTranslationProvider, AzureTextTranslationProvider>();
        builder.Services.AddScoped<ISpeechToTextProvider, AzureSpeechToTextProvider>();
        builder.Services.AddScoped<ITextToSpeechProvider, AzureTextToSpeechProvider>();
        break;

    default:
        throw new InvalidOperationException(
            $"Unknown translation provider '{providerName}'. " +
            "Supported values: Mock, Azure. Check Translation:Provider in configuration.");
}

builder.Services.AddScoped<ILanguageCatalogService, StaticLanguageCatalogService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors(localDevCorsPolicy);

app.MapControllers();

app.Run();

// Required for WebApplicationFactory<Program> in integration tests.
public partial class Program { }
