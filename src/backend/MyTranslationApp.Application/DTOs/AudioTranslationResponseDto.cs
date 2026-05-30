namespace MyTranslationApp.Application.DTOs;

public class AudioTranslationResponseDto
{
    public string TranscribedText { get; set; } = string.Empty;
    public string TranslatedText { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = string.Empty;
    public string TargetLanguage { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
}
