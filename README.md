# My Translation App

A production-deployed web application for translating text, speech, and conversations across 37 languages.

The app supports text translation, audio translation, speech-to-text, text-to-speech playback, push-to-talk conversation mode, saved conversations, conversation search, and export options.

---

## Project Status

**Version:** 1.0  
**Status:** Complete  
**Type:** Full-stack Web App  
**Deployment:** AWS CloudFront + S3 frontend, AWS Elastic Beanstalk backend  
**AI Services:** Azure Translator + Azure Speech Services  

Live app:

```text
https://d2ftspeokj49uq.cloudfront.net
```

---

## Key Features

- Text translation
- Audio translation
- Speech-to-text
- Text-to-speech playback
- 37 supported languages
- Conversation Mode
- Push-to-talk recording
- Auto-play translated responses
- Multi-conversation management
- Conversation search
- Local conversation persistence
- TXT export
- JSON export
- Clipboard copy
- Landing page with production branding
- Mobile-responsive UI

---

## Tech Stack

### Frontend

- React
- TypeScript
- Vite
- CSS

### Backend

- .NET 8
- ASP.NET Core Web API
- C#

### AI / Cloud Services

- Azure Translator
- Azure Speech-to-Text
- Azure Text-to-Speech

### Deployment

- AWS S3
- AWS CloudFront
- AWS Elastic Beanstalk

### Testing

- xUnit
- ASP.NET Core integration tests
- 133 backend tests passing

---

## Architecture Overview

```text
Browser
  ↓
CloudFront
  ├── S3 Frontend
  └── Elastic Beanstalk Backend
          ↓
      Azure Translator
      Azure Speech Services
```

The frontend is deployed as a static React app through S3 and CloudFront.

The backend is a .NET 8 API deployed to Elastic Beanstalk.

Azure provides the translation, speech-to-text, and text-to-speech services.

---

## Main Workflows

### Text Translation

```text
User enters text
  ↓
Backend translation API
  ↓
Azure Translator
  ↓
Translated result
```

### Audio Translation

```text
User uploads or records audio
  ↓
Azure Speech-to-Text
  ↓
Azure Translator
  ↓
Translated result
```

### Conversation Mode

```text
Speaker A talks
  ↓
Speech-to-text
  ↓
Translation
  ↓
Text-to-speech playback

Speaker B replies
  ↓
Same flow in reverse
```

---

## Project Structure

```text
docs/                       Technical documentation
planning/                   Sprint planning, decisions, risks, questions
portfolio/                  Portfolio case study, demo script, release notes
design/                     Design references and screenshots
src/backend/                .NET backend API
src/frontend/               React frontend
tests/backend/              Backend automated tests
```

---

## Important Documentation

| File | Purpose |
|---|---|
| `docs/ARCHITECTURE.md` | System architecture |
| `docs/API.md` | API contract |
| `docs/DEPLOYMENT.md` | Deployment guide |
| `docs/AWS_DEPLOYMENT.md` | AWS deployment notes |
| `docs/OPERATIONS.md` | Production operations |
| `docs/VALIDATION.md` | Validation and test notes |
| `portfolio/case-study.md` | Portfolio case study |
| `portfolio/architecture-overview.md` | Portfolio architecture summary |
| `portfolio/demo-script.md` | Demo walkthrough |
| `portfolio/release-notes-v1.md` | Version 1.0 release notes |

---

## Development Setup

### Backend

```bash
cd src/backend
dotnet build
dotnet test
```

### Frontend

```bash
cd src/frontend
npm install
npm run dev
```

### Frontend Production Build

```bash
cd src/frontend
npm run build
```

---

## Environment Configuration

The backend supports mock and Azure provider modes.

Production uses Azure provider mode with environment variables configured in AWS Elastic Beanstalk.

Secrets are not committed to source control.

Required production configuration includes:

```text
Translation__Provider=Azure
AzureTranslator__Key
AzureTranslator__Region
AzureSpeech__Key
AzureSpeech__Region
AzureSpeech__Endpoint
AllowedCorsOrigins__0
```

See:

```text
docs/ENVIRONMENTS.md
docs/DEPLOYMENT.md
docs/AWS_DEPLOYMENT.md
```

---

## Testing Status

Current validation status:

```text
Backend tests: 133/133 passing
Frontend TypeScript: clean
Frontend production build: clean
Production deployment: validated
```

---

## Portfolio Notes

This project was developed using an AI Architect / Builder workflow.

The project includes:

- sprint planning
- requirements
- architecture documents
- acceptance criteria
- dry-run reviews
- completion reports
- risk tracking
- decision logs
- production deployment documentation

The project folder is the source of truth.

---

## Version 1.0 Summary

My Translation App v1.0 demonstrates:

- full-stack development
- cloud deployment
- Azure AI integration
- AWS hosting
- speech processing
- responsive frontend design
- production validation
- automated testing
- structured software delivery methodology

---

## Future Enhancements

Potential post-v1 improvements:

- Custom domain
- Public demo video
- User accounts
- Cloud-synced conversations
- Shared conversations
- Real-time streaming translation
- PDF export
- Analytics dashboard

These are intentionally deferred beyond v1.0.
