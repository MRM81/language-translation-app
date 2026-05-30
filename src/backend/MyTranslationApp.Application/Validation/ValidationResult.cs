using MyTranslationApp.Application.DTOs;

namespace MyTranslationApp.Application.Validation;

public class ValidationResult
{
    public bool IsValid { get; private init; }
    public string? ErrorCode { get; private init; }
    public string? Message { get; private init; }
    public IReadOnlyList<ApiErrorDetailDto> Details { get; private init; } = Array.Empty<ApiErrorDetailDto>();

    public static ValidationResult Ok() => new() { IsValid = true };

    public static ValidationResult Fail(
        string errorCode,
        string message,
        IReadOnlyList<ApiErrorDetailDto>? details = null) =>
        new()
        {
            IsValid = false,
            ErrorCode = errorCode,
            Message = message,
            Details = details ?? Array.Empty<ApiErrorDetailDto>()
        };
}
