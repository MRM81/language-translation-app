using MyTranslationApp.Application.Interfaces;

namespace MyTranslationApp.Infrastructure.Providers;

// Sprint 003 placeholder — updated in Sprint 007 to return a minimal valid WAV
// so integration tests can verify a non-empty binary response without Azure credentials.
// Mock mode returns audio/wav (silent, 0-duration). Azure mode returns audio/mpeg (MP3).
public class MockTextToSpeechProvider : ITextToSpeechProvider
{
    // Minimal valid PCM WAV: 8kHz, 16-bit, mono, 0 samples (silent, 44 bytes).
    // Allows browser-facing integration tests to exercise the full playback flow without real audio.
    private static readonly byte[] SilentWavFixture =
    [
        0x52, 0x49, 0x46, 0x46, // "RIFF"
        0x24, 0x00, 0x00, 0x00, // ChunkSize = 36 (no data bytes)
        0x57, 0x41, 0x56, 0x45, // "WAVE"
        0x66, 0x6D, 0x74, 0x20, // "fmt "
        0x10, 0x00, 0x00, 0x00, // Subchunk1Size = 16
        0x01, 0x00,             // AudioFormat = PCM (1)
        0x01, 0x00,             // NumChannels = 1 (mono)
        0x40, 0x1F, 0x00, 0x00, // SampleRate = 8000
        0x80, 0x3E, 0x00, 0x00, // ByteRate = 16000
        0x02, 0x00,             // BlockAlign = 2
        0x10, 0x00,             // BitsPerSample = 16
        0x64, 0x61, 0x74, 0x61, // "data"
        0x00, 0x00, 0x00, 0x00, // Subchunk2Size = 0
    ];

    public Task<(byte[] AudioData, string ContentType)> SynthesizeAsync(
        string text,
        string language,
        string? voice,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult((SilentWavFixture, "audio/wav"));
    }
}
