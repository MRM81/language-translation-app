# Demo Script — My Translation App

**Duration:** ~2–3 minutes  
**Audience:** Recruiter, hiring manager, or technical reviewer

---

## Introduction (30 seconds)

> "This is My Translation App — a browser-based translation tool I built across eight sprints, from architecture design through to a polished MVP.
>
> It supports text translation, push-to-talk audio translation, and speech playback. The backend is .NET 8 with Azure Translator and Azure Speech Services. The frontend is React and TypeScript. Let me show you how it works."

---

## Text Translation (40 seconds)

1. Open the app at `http://localhost:5173`
2. In the **Text Translation** panel:
   - Source language: **English** (or Auto-detect)
   - Target language: **Spanish**
   - Type: *"Hello, how are you today?"*
3. Click **Translate**
4. Point out the result panel:
   - The translated text appears prominently: *"Hola, ¿cómo estás hoy?"*
   - Source language, target language, and provider are shown
5. Click the **▶ Play** button to synthesize and play the translation via Azure TTS
   - Audio plays in the browser

> "Text translation goes through Azure Translator. The play button generates speech via Azure Speech Services — on demand, so we're not generating audio for every translation unnecessarily."

---

## Push-to-Talk Translation (50 seconds)

1. Switch to the **Audio Translation** panel
2. Set Source language: **English**, Target language: **French**
3. Click **Record** — browser will request microphone permission if not yet granted
4. Speak: *"What time is the next train to Paris?"*
5. Click **Stop**
6. Point out the result panel:
   - **Transcript:** *"What time is the next train to Paris?"* — what Azure Speech heard
   - **Translation:** the French translation
7. Click **▶ Play** to hear the French output spoken aloud

> "Push-to-talk uses the browser MediaRecorder API. The app detects the best audio format at runtime — Chrome uses WebM, Firefox uses OGG — and routes compressed audio through the Azure Speech Fast Transcription REST API, which handles codec decoding server-side. No native codec dependencies on the server."

---

## Architecture Callout (30 seconds)

Navigate to [portfolio/architecture-overview.md](architecture-overview.md) or describe verbally:

> "The backend uses a provider abstraction layer — interfaces in the Application layer, Azure adapters in Infrastructure. Switching from Azure to OpenAI or DeepL only requires a new adapter and a config change. No credentials ever reach the browser. The frontend never calls Azure directly."

---

## Closing (20 seconds)

> "The project is documented across eight sprints in the planning folder — decisions, risks, open questions, and acceptance criteria. The portfolio folder has the full case study and lessons learned.
>
> If you'd like to dig into the architecture or any specific sprint, I'm happy to walk through it."
