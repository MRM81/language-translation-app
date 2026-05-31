# Release Notes — My Translation App v1.0

**Release Date:** 2026-05-31
**Status:** Production — Live

---

## Major Features

### Translation Mode
- Text translation across 37 languages via Azure Translator
- Audio translation via push-to-talk recording (Chrome, Edge, Firefox) or file upload
- Text-to-speech playback of translated output via Azure Speech Services
- Language swap (⇄) for quick direction reversal
- Auto-detect source language option
- Text / Audio segmented toggle — one form visible at a time

### Conversation Mode
- Turn-based bilingual conversation between two speakers (A and B)
- Push-To-Talk recording for natural spoken input
- Auto-play translated output after each turn
- Speaker switching mid-conversation
- Scrollable message history with timestamps
- Text and audio file input as alternatives to recording

### Conversation Management
- Multiple named conversations (create, rename, delete, switch)
- Auto-title from first message
- Full-text search across all conversations (title + message content)
- Conversation persistence across page refresh (localStorage)
- Export active conversation — TXT, JSON, or clipboard

### Product Experience
- Landing page as default first screen with branding
- Workspace header logo — links back to landing page
- Text / Audio translation toggle in workspace
- Favicon configured

---

## Technology Stack

| Layer | Technology |
|---|---|
| Frontend | React 18 + TypeScript + Vite 5 |
| Styling | Plain CSS with custom properties |
| Backend | .NET 8 LTS (ASP.NET Core) |
| Text translation | Azure Translator (API v3.0) |
| Speech-to-text | Azure Speech SDK 1.50.0 + Fast Transcription REST API |
| Text-to-speech | Azure Speech SDK 1.50.0 (MP3 output) |
| Frontend hosting | AWS S3 + CloudFront (HTTPS) |
| Backend hosting | AWS Elastic Beanstalk (.NET 8 on AL2023) |

---

## Production Environment

| Service | Detail |
|---|---|
| Frontend | AWS CloudFront — `d2ftspeokj49uq.cloudfront.net` |
| Backend | AWS Elastic Beanstalk — `my-translation-api-prod`, ap-southeast-2 |
| Health endpoint | `https://d2ftspeokj49uq.cloudfront.net/health` |
| Azure region | Azure AI Speech + Translator configured in ap-southeast-2 proximity |

---

## Test Coverage

- 133 automated tests — 133/133 passing at v1.0
- xUnit — integration tests via `WebApplicationFactory`
- Coverage: validation, translation service, audio routing, TTS, provider selection, health endpoint, language capability

---

## Known Limitations

| Limitation | Notes |
|---|---|
| Audio duration enforcement | 60-second limit enforced client-side only; server-side enforcement deferred (R-016) |
| No authentication | v1 is zero-account by design; cloud sync requires auth |
| localStorage only | Conversations do not sync across devices or browsers |
| Single-user | No shared or team conversations |
| EB SingleInstance | No auto-scaling; single EC2 instance; acceptable for portfolio |
| HTTP-only EB endpoint | Mitigated by CloudFront same-origin proxy pattern |

---

## Future Ideas (Post-v1)

- Custom domain (Route 53 + ACM)
- Route-based navigation for deep-linking
- Real-time continuous speech translation
- Authentication and user accounts
- Cloud sync for conversation history
- CI/CD pipeline (GitHub Actions or AWS CodePipeline)
- Team and shared conversations
- Demo video
