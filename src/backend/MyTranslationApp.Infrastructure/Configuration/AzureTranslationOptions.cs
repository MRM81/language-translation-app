namespace MyTranslationApp.Infrastructure.Configuration;

public class AzureTranslationOptions
{
    public const string SectionName = "AzureTranslator";

    public string Endpoint { get; set; } = "https://api.cognitive.microsofttranslator.com/";
    public string Region { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Key) && !string.IsNullOrWhiteSpace(Region);
}
