namespace MyTranslationApp.Application.DTOs;

public class LanguageOptionDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool SupportsTextTranslation { get; set; } = true;
    public bool SupportsSpeechToText { get; set; } = true;
    public bool SupportsTextToSpeech { get; set; } = true;
}
