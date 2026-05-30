using Microsoft.Extensions.Options;
using MyTranslationApp.Application.DTOs;
using MyTranslationApp.Application.Validation;

namespace MyTranslationApp.Tests.Validation;

public class TranslationValidationTests
{
    private readonly TranslationRequestValidator _validator;

    public TranslationValidationTests()
    {
        var options = Options.Create(new TranslationValidationOptions());
        _validator = new TranslationRequestValidator(options);
    }

    // --- Text validation ---

    [Fact]
    public void ValidateTextRequest_ValidRequest_ReturnsOk()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            TargetLanguage = "es"
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateTextRequest_EmptySourceText_ReturnsValidationError()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "",
            TargetLanguage = "es"
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.ValidationError, result.ErrorCode);
        Assert.Contains(result.Details, d => d.Field == "sourceText");
    }

    [Fact]
    public void ValidateTextRequest_WhitespaceSourceText_ReturnsValidationError()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "   ",
            TargetLanguage = "es"
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.ValidationError, result.ErrorCode);
    }

    [Fact]
    public void ValidateTextRequest_TextAtExactLimit_ReturnsOk()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = new string('a', 5000),
            TargetLanguage = "es"
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateTextRequest_TextExceedsLimit_ReturnsTextTooLong()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = new string('a', 5001),
            TargetLanguage = "es"
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.TextTooLong, result.ErrorCode);
    }

    [Fact]
    public void ValidateTextRequest_MissingTargetLanguage_ReturnsValidationError()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            TargetLanguage = ""
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.ValidationError, result.ErrorCode);
        Assert.Contains(result.Details, d => d.Field == "targetLanguage");
    }

    [Fact]
    public void ValidateTextRequest_OptionalSourceLanguage_ReturnsOk()
    {
        var request = new TextTranslationRequestDto
        {
            SourceText = "Hello",
            SourceLanguage = null,
            TargetLanguage = "es"
        };

        var result = _validator.ValidateTextRequest(request);

        Assert.True(result.IsValid);
    }

    // --- Audio validation ---

    [Fact]
    public void ValidateAudioRequest_ValidRequest_ReturnsOk()
    {
        var result = _validator.ValidateAudioRequest(
            hasAudio: true,
            audioSizeBytes: 1024,
            contentType: "audio/webm",
            targetLanguage: "es");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateAudioRequest_MissingAudio_ReturnsValidationError()
    {
        var result = _validator.ValidateAudioRequest(
            hasAudio: false,
            audioSizeBytes: 0,
            contentType: "",
            targetLanguage: "es");

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.ValidationError, result.ErrorCode);
        Assert.Contains(result.Details, d => d.Field == "audio");
    }

    [Fact]
    public void ValidateAudioRequest_UnsupportedMimeType_ReturnsUnsupportedAudioFormat()
    {
        var result = _validator.ValidateAudioRequest(
            hasAudio: true,
            audioSizeBytes: 1024,
            contentType: "video/mp4",
            targetLanguage: "es");

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.UnsupportedAudioFormat, result.ErrorCode);
    }

    [Fact]
    public void ValidateAudioRequest_AudioWebmWithCodecs_IsAccepted()
    {
        // Browsers may emit "audio/webm;codecs=opus" — normalised to "audio/webm" for comparison.
        var result = _validator.ValidateAudioRequest(
            hasAudio: true,
            audioSizeBytes: 1024,
            contentType: "audio/webm;codecs=opus",
            targetLanguage: "es");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateAudioRequest_AudioWebmWithOtherCodecs_IsAccepted()
    {
        // Any audio/webm variant should be accepted after parameter stripping.
        var result = _validator.ValidateAudioRequest(
            hasAudio: true,
            audioSizeBytes: 1024,
            contentType: "audio/webm;codecs=vp8",
            targetLanguage: "es");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateAudioRequest_AudioSizeOverLimit_ReturnsAudioTooLarge()
    {
        var result = _validator.ValidateAudioRequest(
            hasAudio: true,
            audioSizeBytes: 11 * 1024 * 1024,
            contentType: "audio/webm",
            targetLanguage: "es");

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.AudioTooLarge, result.ErrorCode);
    }

    [Fact]
    public void ValidateAudioRequest_MissingTargetLanguage_ReturnsValidationError()
    {
        var result = _validator.ValidateAudioRequest(
            hasAudio: true,
            audioSizeBytes: 1024,
            contentType: "audio/webm",
            targetLanguage: "");

        Assert.False(result.IsValid);
        Assert.Equal(ErrorCodes.ValidationError, result.ErrorCode);
        Assert.Contains(result.Details, d => d.Field == "targetLanguage");
    }

    [Fact]
    public void ValidateAudioRequest_AllAcceptedMimeTypes_AreAllowed()
    {
        var acceptedTypes = new[]
        {
            "audio/webm",
            "audio/webm;codecs=opus",
            "audio/ogg",
            "audio/mp4",
            "audio/mpeg",
            "audio/wav"
        };

        foreach (var mimeType in acceptedTypes)
        {
            var result = _validator.ValidateAudioRequest(
                hasAudio: true,
                audioSizeBytes: 1024,
                contentType: mimeType,
                targetLanguage: "es");

            Assert.True(result.IsValid, $"MIME type '{mimeType}' should be accepted.");
        }
    }
}
