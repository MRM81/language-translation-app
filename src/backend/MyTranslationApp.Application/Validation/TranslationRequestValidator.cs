using Microsoft.Extensions.Options;
using MyTranslationApp.Application.DTOs;

namespace MyTranslationApp.Application.Validation;

public class TranslationRequestValidator
{
    private readonly TranslationValidationOptions _options;

    public TranslationRequestValidator(IOptions<TranslationValidationOptions> options)
    {
        _options = options.Value;
    }

    public ValidationResult ValidateTextRequest(TextTranslationRequestDto request)
    {
        var details = new List<ApiErrorDetailDto>();

        if (string.IsNullOrWhiteSpace(request.SourceText))
        {
            details.Add(new ApiErrorDetailDto { Field = "sourceText", Message = "Source text is required." });
        }
        else if (request.SourceText.Length > _options.MaxTextCharacters)
        {
            return ValidationResult.Fail(
                ErrorCodes.TextTooLong,
                $"Source text must not exceed {_options.MaxTextCharacters} characters.",
                new[] { new ApiErrorDetailDto { Field = "sourceText", Message = $"Maximum length is {_options.MaxTextCharacters} characters." } });
        }

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
        {
            details.Add(new ApiErrorDetailDto { Field = "targetLanguage", Message = "Target language is required." });
        }

        if (details.Count > 0)
            return ValidationResult.Fail(ErrorCodes.ValidationError, "The request is invalid.", details);

        return ValidationResult.Ok();
    }

    public ValidationResult ValidateAudioRequest(
        bool hasAudio,
        long audioSizeBytes,
        string contentType,
        string? targetLanguage)
    {
        var details = new List<ApiErrorDetailDto>();

        if (!hasAudio)
        {
            details.Add(new ApiErrorDetailDto { Field = "audio", Message = "Audio file is required." });
        }
        else
        {
            if (audioSizeBytes > _options.MaxAudioBytes)
            {
                return ValidationResult.Fail(
                    ErrorCodes.AudioTooLarge,
                    "Audio file exceeds the maximum allowed size.",
                    new[] { new ApiErrorDetailDto
                    {
                        Field = "audio",
                        Message = $"Maximum audio size is {_options.MaxAudioBytes / 1024 / 1024} MB."
                    }});
            }

            if (!IsAllowedMimeType(contentType, _options.AllowedAudioMimeTypes))
            {
                return ValidationResult.Fail(
                    ErrorCodes.UnsupportedAudioFormat,
                    "The audio format is not supported.",
                    new[] { new ApiErrorDetailDto
                    {
                        Field = "audio",
                        Message = $"Accepted formats: {string.Join(", ", _options.AllowedAudioMimeTypes)}."
                    }});
            }
        }

        if (string.IsNullOrWhiteSpace(targetLanguage))
        {
            details.Add(new ApiErrorDetailDto { Field = "targetLanguage", Message = "Target language is required." });
        }

        if (details.Count > 0)
            return ValidationResult.Fail(ErrorCodes.ValidationError, "The request is invalid.", details);

        return ValidationResult.Ok();
    }

    public ValidationResult ValidateTtsRequest(string? text, string? language)
    {
        var details = new List<ApiErrorDetailDto>();

        if (string.IsNullOrWhiteSpace(text))
            details.Add(new ApiErrorDetailDto { Field = "text", Message = "Text is required." });

        if (string.IsNullOrWhiteSpace(language))
            details.Add(new ApiErrorDetailDto { Field = "language", Message = "Language is required." });

        if (details.Count > 0)
            return ValidationResult.Fail(ErrorCodes.ValidationError, "The request is invalid.", details);

        return ValidationResult.Ok();
    }

    // Strips codec and other parameters before comparing so that
    // "audio/webm;codecs=opus" matches "audio/webm" in the allowed list.
    private static bool IsAllowedMimeType(string contentType, IEnumerable<string> allowedTypes)
    {
        var submittedBase = contentType.Split(';', 2)[0].Trim().ToLowerInvariant();
        return allowedTypes.Any(allowed =>
            string.Equals(
                allowed.Split(';', 2)[0].Trim(),
                submittedBase,
                StringComparison.OrdinalIgnoreCase));
    }
}
