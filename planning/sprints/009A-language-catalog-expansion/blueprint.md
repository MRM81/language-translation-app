# Sprint 009A Blueprint — Language Catalog Expansion

## Files Modified

| File | Change |
|---|---|
| `StaticLanguageCatalogService.cs` | Expanded from 10 to 37 entries, alphabetical order |
| `AzureSpeechToTextProvider.cs` | `LanguageMap` expanded from 10 to 37 entries |
| `AzureTextToSpeechProvider.cs` | `VoiceMap` and `LocaleMap` expanded from 10 to 37 entries |
| `LanguageCatalogTests.cs` | New test file: 44 tests |

## Key Decisions Made During Dry Run

### Chinese Simplified Code Preserved As `zh`
The existing code `zh` is preserved to avoid breaking the working Azure integration. Azure Translator SDK 1.0.0 accepts `zh` as an alias for `zh-Hans`. Chinese Traditional added as `zh-Hant`.

### Norwegian Uses `nb` (Bokmål)
Azure Translator code for Norwegian is `nb`, not the generic `no`. STT locale is `nb-NO`.

### Serbian Uses `sr-Cyrl`
Azure Translator requires a script-specific code. `sr-Cyrl` (Cyrillic) is chosen as the canonical Serbian representation. STT locale is `sr-RS`, TTS voice is `sr-RS-SophieNeural`.

### Decision ID Correction
Architect Pack referenced D-065 to D-068 but those IDs were already occupied by Sprint 008. New decisions recorded as D-070 to D-073.

## Azure Language Codes Reference

### Translation (Azure Translator v3.0)

All 37 app codes are valid Azure Translator target/source codes. The app code is passed directly to the SDK (with `zh` accepted as `zh-Hans` equivalent by the SDK).

### STT Locale Map (Azure Speech Fast Transcription + SDK)

| App Code | Azure STT Locale |
|---|---|
| ar | ar-SA |
| bg | bg-BG |
| hr | hr-HR |
| cs | cs-CZ |
| da | da-DK |
| nl | nl-NL |
| en | en-US |
| et | et-EE |
| fi | fi-FI |
| fr | fr-FR |
| de | de-DE |
| el | el-GR |
| hi | hi-IN |
| hu | hu-HU |
| id | id-ID |
| it | it-IT |
| ja | ja-JP |
| ko | ko-KR |
| lv | lv-LV |
| lt | lt-LT |
| ms | ms-MY |
| nb | nb-NO |
| pl | pl-PL |
| pt | pt-BR |
| ro | ro-RO |
| ru | ru-RU |
| sr-Cyrl | sr-RS |
| sk | sk-SK |
| sl | sl-SI |
| es | es-ES |
| sv | sv-SE |
| th | th-TH |
| tr | tr-TR |
| uk | uk-UA |
| vi | vi-VN |
| zh | zh-CN |
| zh-Hant | zh-TW |

### TTS Voice Map (Azure Neural Voices)

| App Code | Voice |
|---|---|
| ar | ar-SA-ZariyahNeural |
| bg | bg-BG-KalinaNeural |
| hr | hr-HR-GabrijelaNeural |
| cs | cs-CZ-VlastaNeural |
| da | da-DK-ChristelNeural |
| nl | nl-NL-FennaNeural |
| en | en-US-JennyNeural |
| et | et-EE-AnuNeural |
| fi | fi-FI-NooraNeural |
| fr | fr-FR-DeniseNeural |
| de | de-DE-KatjaNeural |
| el | el-GR-AthinaNeural |
| hi | hi-IN-SwaraNeural |
| hu | hu-HU-NoemiNeural |
| id | id-ID-GadisNeural |
| it | it-IT-ElsaNeural |
| ja | ja-JP-NanamiNeural |
| ko | ko-KR-SunHiNeural |
| lv | lv-LV-EveritaNeural |
| lt | lt-LT-OnaNeural |
| ms | ms-MY-YasminNeural |
| nb | nb-NO-PernilleNeural |
| pl | pl-PL-AgnieszkaNeural |
| pt | pt-BR-FranciscaNeural |
| ro | ro-RO-AlinaNeural |
| ru | ru-RU-SvetlanaNeural |
| sr-Cyrl | sr-RS-SophieNeural |
| sk | sk-SK-ViktoriaNeural |
| sl | sl-SI-PetraNeural |
| es | es-ES-ElviraNeural |
| sv | sv-SE-SofieNeural |
| th | th-TH-PremwadeeNeural |
| tr | tr-TR-EmelNeural |
| uk | uk-UA-PolinaNeural |
| vi | vi-VN-HoaiMyNeural |
| zh | zh-CN-XiaoxiaoNeural |
| zh-Hant | zh-TW-HsiaoChenNeural |
