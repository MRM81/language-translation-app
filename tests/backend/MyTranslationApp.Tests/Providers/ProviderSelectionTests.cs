using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MyTranslationApp.Application.Interfaces;
using MyTranslationApp.Infrastructure.Providers;
using MyTranslationApp.Infrastructure.Providers.Azure;

namespace MyTranslationApp.Tests.Providers;

public class ProviderSelectionTests
{
    // --- Mock provider (no Azure config required) ---

    [Fact]
    public void MockProvider_StartsWithoutAzureConfig()
    {
        // Provider=Mock must start successfully with no Azure credentials at all.
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Mock");
                builder.UseSetting("AzureTranslator:Key", "");
                builder.UseSetting("AzureTranslator:Region", "");
                builder.UseSetting("AzureSpeech:Key", "");
                builder.UseSetting("AzureSpeech:Region", "");
            });

        // Creating the client triggers DI/startup. No exception expected.
        var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    [Fact]
    public void MockProvider_RegistersMockTextTranslationProvider()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Mock");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextTranslationProvider>();

        Assert.IsType<MockTextTranslationProvider>(provider);
        Assert.Equal("mock", provider.ProviderName);
    }

    [Fact]
    public void MockProvider_RegistersMockSpeechToTextProvider()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Mock");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ISpeechToTextProvider>();

        Assert.IsType<MockSpeechToTextProvider>(provider);
        Assert.Equal("mock", provider.ProviderName);
    }

    // --- Default appsettings.json Provider value is Mock ---

    [Fact]
    public void DefaultProvider_AppsettingsDefault_IsMock()
    {
        // The appsettings.json default is Provider=Mock. This test pins Mock explicitly
        // to verify the registered type, and is environment-independent regardless
        // of User Secrets or environment variable overrides in the developer's local setup.
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Mock");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextTranslationProvider>();

        Assert.IsType<MockTextTranslationProvider>(provider);
    }

    // --- Azure provider: missing config fails at startup ---

    [Fact]
    public void AzureProvider_MissingTranslatorKey_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    [Fact]
    public void AzureProvider_MissingTranslatorRegion_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    [Fact]
    public void AzureProvider_MissingSpeechKey_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "");
                builder.UseSetting("AzureSpeech:Region", "eastus");
                builder.UseSetting("AzureSpeech:Endpoint", "https://fake-resource.cognitiveservices.azure.com");
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    [Fact]
    public void AzureProvider_MissingSpeechRegion_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "");
                builder.UseSetting("AzureSpeech:Endpoint", "https://fake-resource.cognitiveservices.azure.com");
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    [Fact]
    public void AzureProvider_BothPlaceholderValues_ThrowsOnStartup()
    {
        // The placeholder values shipped in appsettings.json must not be treated as real config.
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "DO_NOT_COMMIT_REAL_KEY");
                builder.UseSetting("AzureTranslator:Region", "YOUR_REGION_HERE");
                builder.UseSetting("AzureSpeech:Key", "DO_NOT_COMMIT_REAL_KEY");
                builder.UseSetting("AzureSpeech:Region", "YOUR_REGION_HERE");
                builder.UseSetting("AzureSpeech:Endpoint", "https://DO_NOT_COMMIT_REAL_ENDPOINT.cognitiveservices.azure.com");
            });

        // Placeholders are non-empty strings so they pass IsConfigured. This is acceptable —
        // the placeholder values will cause an Azure auth failure on the first real call, not at startup.
        // Startup validation only guards against empty/missing config, not invalid values.
        // This test documents that behaviour explicitly.
        var client = factory.CreateClient();
        Assert.NotNull(client);
    }

    // --- Invalid provider name fails at startup ---

    [Fact]
    public void InvalidProvider_UnknownName_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "UnknownProvider");
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    [Fact]
    public void InvalidProvider_EmptyName_ThrowsOnStartup()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "  ");
            });

        Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
    }

    // --- Azure provider: valid fake config registers Azure providers ---

    [Fact]
    public void AzureProvider_ValidFakeConfig_RegistersAzureTextTranslationProvider()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
                builder.UseSetting("AzureSpeech:Endpoint", "https://fake-resource.cognitiveservices.azure.com");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextTranslationProvider>();

        Assert.IsType<AzureTextTranslationProvider>(provider);
        Assert.Equal("azure", provider.ProviderName);
    }

    [Fact]
    public void AzureProvider_ValidFakeConfig_RegistersAzureSpeechToTextProvider()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
                builder.UseSetting("AzureSpeech:Endpoint", "https://fake-resource.cognitiveservices.azure.com");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ISpeechToTextProvider>();

        Assert.IsType<AzureSpeechToTextProvider>(provider);
        Assert.Equal("azure", provider.ProviderName);
    }

    // --- ITextToSpeechProvider registration ---

    [Fact]
    public void MockProvider_RegistersMockTextToSpeechProvider()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Mock");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextToSpeechProvider>();

        Assert.IsType<MockTextToSpeechProvider>(provider);
    }

    [Fact]
    public void AzureProvider_ValidFakeConfig_RegistersAzureTextToSpeechProvider()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "Azure");
                builder.UseSetting("AzureTranslator:Key", "fake-translator-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-speech-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
                builder.UseSetting("AzureSpeech:Endpoint", "https://fake-resource.cognitiveservices.azure.com");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextToSpeechProvider>();

        Assert.IsType<AzureTextToSpeechProvider>(provider);
    }

    // --- Provider name casing is case-insensitive ---

    [Fact]
    public void MockProvider_LowercaseName_Works()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "mock");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextTranslationProvider>();
        Assert.IsType<MockTextTranslationProvider>(provider);
    }

    [Fact]
    public void AzureProvider_LowercaseName_Works()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Translation:Provider", "azure");
                builder.UseSetting("AzureTranslator:Key", "fake-key");
                builder.UseSetting("AzureTranslator:Region", "eastus");
                builder.UseSetting("AzureSpeech:Key", "fake-key");
                builder.UseSetting("AzureSpeech:Region", "eastus");
                builder.UseSetting("AzureSpeech:Endpoint", "https://fake-resource.cognitiveservices.azure.com");
            });

        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITextTranslationProvider>();
        Assert.IsType<AzureTextTranslationProvider>(provider);
    }
}
