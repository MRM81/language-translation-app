# My Translation App — Backend

.NET 8 backend API. Sprint 005 — Azure Provider Integration.

---

## Prerequisites

- .NET 8 SDK (`dotnet --version` should show `8.x.x`)

---

## Running Locally (Mock Mode)

Mock mode requires no Azure credentials. All provider calls return deterministic mock output.

```bash
cd src/backend/MyTranslationApp.Api
dotnet run
```

The API starts at `http://localhost:5074` by default (see `Properties/launchSettings.json`).

Mock mode is the default (`Translation:Provider = "Mock"` in `appsettings.json`).

---

## Running Locally (Azure Mode)

### Step 1 — Create Azure resources

You need two Azure resources:

1. **Azure Translator** — text translation
   - Resource type: Translator (or multi-service Cognitive Services)
   - Note your: API key, region (e.g. `eastus`), endpoint

2. **Azure Speech** — speech-to-text
   - Resource type: Speech
   - Note your: API key, region (e.g. `eastus`)

### Step 2 — Set User Secrets

User Secrets are stored outside the repo on your local machine. They are never committed.

From the solution root:

```bash
cd src/backend/MyTranslationApp.Api

dotnet user-secrets init
dotnet user-secrets set "AzureTranslator:Key" "YOUR_REAL_TRANSLATOR_KEY"
dotnet user-secrets set "AzureTranslator:Region" "YOUR_REGION"
dotnet user-secrets set "AzureSpeech:Key" "YOUR_REAL_SPEECH_KEY"
dotnet user-secrets set "AzureSpeech:Region" "YOUR_REGION"
dotnet user-secrets set "Translation:Provider" "Azure"
```

To verify:

```bash
dotnet user-secrets list
```

### Step 3 — Run with Azure provider

```bash
dotnet run
```

The API will start in Azure mode. If any required secret is missing, the app will fail at startup with a clear error message before accepting any requests.

### Step 4 — Revert to Mock mode

To switch back to Mock (no credentials needed):

```bash
dotnet user-secrets set "Translation:Provider" "Mock"
dotnet run
```

Or remove the provider override entirely:

```bash
dotnet user-secrets remove "Translation:Provider"
dotnet run
```

### Alternative — Environment Variables

If User Secrets are not available, set environment variables before running:

**PowerShell:**

```powershell
$env:Translation__Provider = "Azure"
$env:AzureTranslator__Key = "YOUR_REAL_TRANSLATOR_KEY"
$env:AzureTranslator__Region = "YOUR_REGION"
$env:AzureSpeech__Key = "YOUR_REAL_SPEECH_KEY"
$env:AzureSpeech__Region = "YOUR_REGION"
dotnet run
```

Note: Use double underscores (`__`) as the separator for nested config keys in environment variables.

---

## Manual Live Validation

With the backend running in Azure mode and the frontend running (`cd src/frontend && npm run dev`):

### Text translation

1. Open `http://localhost:5173` in a browser.
2. Select source and target languages (e.g. English → Spanish).
3. Type: `Hello, how are you?`
4. Click Translate.
5. Expected: real translated text (e.g. `Hola, ¿cómo estás?`), `provider: "azure"` in response.

```bash
curl -X POST http://localhost:5074/api/translate/text \
  -H "Content-Type: application/json" \
  -d '{"sourceText": "Hello", "sourceLanguage": "en", "targetLanguage": "es"}'
```

Expected:
```json
{
  "translatedText": "Hola",
  "sourceLanguage": "en",
  "targetLanguage": "es",
  "provider": "azure",
  "correlationId": "..."
}
```

### Audio translation

Recommended test format: **WAV** (16-bit PCM, 16kHz). MP3 files also work.

```bash
curl -X POST http://localhost:5074/api/translate/audio \
  -F "audio=@sample.wav;type=audio/wav" \
  -F "targetLanguage=es" \
  -F "sourceLanguage=en"
```

Expected:
```json
{
  "transcribedText": "Hello, how are you?",
  "translatedText": "Hola, ¿cómo estás?",
  "sourceLanguage": "en",
  "targetLanguage": "es",
  "provider": "azure",
  "correlationId": "..."
}
```

**Known audio format limitations (Sprint 005):**

| Format | Status |
|---|---|
| WAV (PCM) | Fully supported |
| MP3 | Fully supported |
| WebM/Opus | Best-effort (same codec, different container — may work) |
| MP4/AAC | Not supported by Azure Speech SDK |

---

## Configuration Reference

| Key | Default | Description |
|---|---|---|
| `Translation:Provider` | `Mock` | Active provider. Supported values: `Mock`, `Azure`. Case-insensitive. |
| `Translation:MaxTextCharacters` | `5000` | Maximum text input length |
| `Translation:MaxAudioSeconds` | `60` | Max audio duration (enforcement deferred — see VALIDATION.md) |
| `Translation:MaxAudioBytes` | `10485760` | Max audio file size (10 MB) |
| `Translation:AllowedAudioMimeTypes` | `[...]` | Accepted audio MIME types |
| `AzureTranslator:Endpoint` | `https://api.cognitive.microsofttranslator.com/` | Azure Translator endpoint |
| `AzureTranslator:Region` | (placeholder) | Azure region. Set via User Secrets or env var. |
| `AzureTranslator:Key` | (placeholder) | Azure Translator API key. **Never commit.** |
| `AzureSpeech:Region` | (placeholder) | Azure Speech region. Set via User Secrets or env var. |
| `AzureSpeech:Key` | (placeholder) | Azure Speech API key. **Never commit.** |

---

## Startup Validation

When `Translation:Provider = "Azure"`:

- Both `AzureTranslator:Key` and `AzureTranslator:Region` must be non-empty.
- Both `AzureSpeech:Key` and `AzureSpeech:Region` must be non-empty.
- If any value is missing, the app throws `InvalidOperationException` at startup with a message naming the missing config key.
- No requests are accepted until all required config is present.

When `Translation:Provider = "Mock"`:

- No Azure credentials are checked or required.
- App starts immediately with mock provider output.

---

## Running Tests

```bash
dotnet test tests/backend/MyTranslationApp.Tests/
```

All tests use Mock mode. No Azure credentials are required to run the test suite.

---

## Project Structure

```
src/backend/
  MyTranslationApp.Api/          HTTP layer — controllers, middleware, composition root
  MyTranslationApp.Application/  Business logic — interfaces, DTOs, validation, services
  MyTranslationApp.Infrastructure/  Provider implementations (mock + Azure)

tests/backend/
  MyTranslationApp.Tests/        xUnit tests
```

Dependency direction: `Api → Application ← Infrastructure`

Application has zero references to Infrastructure.
Azure SDK packages are confined to Infrastructure.

---

## Security Notes

- Azure API keys must never be committed to source control.
- Use User Secrets for local development. See Step 2 above.
- `appsettings.json` contains only placeholder values — not real credentials.
- The backend proxies all provider calls. The frontend never holds Azure credentials.
- Raw audio, source text, and translated text are never logged.
