namespace MyTranslationApp.Application.DTOs;

public class ApiErrorResponseDto
{
    public string ErrorCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ApiErrorDetailDto> Details { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
}
