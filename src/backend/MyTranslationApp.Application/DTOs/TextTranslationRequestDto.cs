namespace MyTranslationApp.Application.DTOs;

public class TextTranslationRequestDto
{
    public string SourceText { get; set; } = string.Empty;
    public string? SourceLanguage { get; set; }
    public string TargetLanguage { get; set; } = string.Empty;
}
