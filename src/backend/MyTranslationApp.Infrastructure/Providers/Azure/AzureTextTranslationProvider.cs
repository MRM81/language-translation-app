using Azure;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyTranslationApp.Application.Exceptions;
using MyTranslationApp.Application.Interfaces;
using MyTranslationApp.Application.Validation;
using MyTranslationApp.Infrastructure.Configuration;

namespace MyTranslationApp.Infrastructure.Providers.Azure;

public class AzureTextTranslationProvider : ITextTranslationProvider
{
    private readonly TextTranslationClient _client;
    private readonly ILogger<AzureTextTranslationProvider> _logger;

    public string ProviderName => "azure";

    public AzureTextTranslationProvider(
        IOptions<AzureTranslationOptions> options,
        ILogger<AzureTextTranslationProvider> logger)
    {
        var opts = options.Value;
        _client = new TextTranslationClient(
            new AzureKeyCredential(opts.Key),
            new Uri(opts.Endpoint),
            opts.Region);
        _logger = logger;
    }

    public async Task<string> TranslateAsync(
        string text,
        string targetLanguage,
        string? sourceLanguage,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Calling Azure Translator. TargetLanguage: {TargetLanguage}",
            targetLanguage);

        try
        {
            var from = string.IsNullOrWhiteSpace(sourceLanguage) || sourceLanguage == "auto"
                ? null
                : sourceLanguage;

            var response = await _client.TranslateAsync(
                targetLanguage,
                text,
                sourceLanguage: from,
                cancellationToken: cancellationToken);

            var translation = response.Value
                ?.FirstOrDefault()
                ?.Translations
                ?.FirstOrDefault();

            if (translation is null || string.IsNullOrEmpty(translation.Text))
                throw new ProviderException(
                    ErrorCodes.TranslationFailed,
                    "Translation service returned no result. Please try again.");

            return translation.Text;
        }
        catch (ProviderException)
        {
            throw;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogWarning(
                "Azure Translator request failed. Status: {Status}",
                ex.Status);
            throw new ProviderException(
                ErrorCodes.ProviderError,
                "The translation service is temporarily unavailable. Please try again shortly.",
                ex);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new ProviderException(
                ErrorCodes.ProviderTimeout,
                "The translation service took too long to respond. Please try again.",
                ex);
        }
    }
}
