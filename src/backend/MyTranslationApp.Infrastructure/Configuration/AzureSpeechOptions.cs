namespace MyTranslationApp.Infrastructure.Configuration;

public class AzureSpeechOptions
{
    public const string SectionName = "AzureSpeech";

    public string Region { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;

    // Base endpoint URL for the Azure AI Services resource.
    // Used by the Fast Transcription REST path for compressed audio (MP3, WebM, OGG).
    // Example: https://my-resource.cognitiveservices.azure.com
    // Region is still used by the WAV SDK path (SpeechConfig.FromSubscription).
    public string Endpoint { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Key) &&
        !string.IsNullOrWhiteSpace(Region) &&
        !string.IsNullOrWhiteSpace(Endpoint);
}
