namespace MyTranslationApp.Application.DTOs;

public class LanguageListResponseDto
{
    public List<LanguageOptionDto> Languages { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
}
