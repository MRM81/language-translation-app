# Demo Script — My Translation App v1.0

**Duration:** ~7–8 minutes
**Audience:** Recruiter, hiring manager, or technical reviewer
**URL:** https://d2ftspeokj49uq.cloudfront.net

---

## Introduction (30 seconds)

> "This is My Translation App — a browser-based translation tool I built across 19 sprints, from architecture design through to a live v1.0 production release on AWS.
>
> It supports text translation, push-to-talk audio translation, spoken playback, and a full Conversation Mode for live bilingual conversations. The backend is .NET 8 with Azure Translator and Azure Speech Services, hosted on AWS Elastic Beanstalk. The frontend is React and TypeScript, served through CloudFront.
>
> Let me walk you through it."

---

## 1. Landing Page (30 seconds)

1. Open the app at the production URL
2. Point out the landing page:
   - App logo and branding
   - Short description of capabilities
   - Feature highlights
   - "Start Translating" CTA

> "The landing page is the first thing users see — a clean entry point before they reach the workspace. Clicking the logo at any point brings them back here."

3. Click **Start Translating**

---

## 2. Text Translation (60 seconds)

1. The workspace opens in **Translation** mode, **Text Translation** selected by default
2. Set Source: **English**, Target: **Spanish**
3. Type: *"Hello, how are you today?"*
4. Click **Translate**
5. Point out the result panel:
   - Translated text displayed prominently
   - Source language, target language, and provider shown
6. Click **▶ Play** to synthesize and play the translation via Azure TTS

> "Text translation goes through Azure Translator. The Play button generates speech via Azure Speech Services — on demand, so we're not generating audio for every translation unnecessarily. Notice the result panel is below the form rather than next to it — one focused interaction at a time."

7. Click the **⇄ swap** button to reverse the language pair
8. Try a second translation in the opposite direction

---

## 3. Audio Translation (60 seconds)

1. Click **Audio Translation** in the segmented toggle
2. Point out that the Text Translation form disappears — only the relevant form is shown
3. Set Source: **English**, Target: **French**
4. Click **Record** (microphone permission prompt if first time)
5. Speak: *"What time is the next train to Paris?"*
6. Click **Stop**
7. Point out the result panel:
   - **Transcript:** what Azure Speech heard
   - **Translation:** the French translation
8. Click **▶ Play** to hear the French output

> "Push-to-talk uses the browser MediaRecorder API. The app detects the best audio format at runtime — Chrome uses WebM, Firefox uses OGG — and routes compressed audio through the Azure Speech Fast Transcription REST API, which handles codec decoding server-side. No native codec dependencies on the server."

---

## 4. Conversation Mode (90 seconds)

1. Click **Conversation** in the tab nav
2. Point out the layout:
   - Language pair selectors for Speaker A and Speaker B
   - Empty message history
   - Input section with Record / Text / Audio File tabs

> "Conversation Mode is one of the more interesting features architecturally. It reuses all three existing API endpoints — translate, audio, and TTS — to create a turn-based bilingual conversation. There are no new backend endpoints."

3. Set Speaker A: **English**, Speaker B: **French**
4. As Speaker A — type: *"Where is the nearest pharmacy?"*
5. Click **Translate**
6. Show the A-side message bubble appear — original text + translation
7. Auto-play fires (French audio plays)
8. Show the **Switch Speaker** button — click it to switch to B
9. As Speaker B — type the French response or record it
10. Show the B-side message bubble appear on the right

> "Each message bubble shows the speaker, the original text, the translation, and the timestamp. Speaker A appears on the left, Speaker B on the right — like a chat interface."

---

## 5. Multi-Conversation Management & Search (60 seconds)

1. Point out the conversation selector at the top of Conversation Mode
2. Click **New** to create a second conversation
3. Type a message in the new conversation
4. Switch back to the first conversation using the dropdown
5. Type a search query in the search box
6. Show the filtered results — matches title and message content

> "Each conversation has a name, is stored in localStorage, and survives page refresh. The auto-title feature names a new conversation from its first message. Search works across all conversations — title and full message text."

7. Show **Rename** — rename one conversation
8. Point out **Export** options: TXT, JSON, clipboard

---

## 6. Export (30 seconds)

1. Click **Export TXT** — show the downloaded file or describe the format
2. Click **Export JSON** — describe the machine-readable structure with version field
3. Click **Copy** — describe clipboard output

> "TXT export is human-readable — a plain transcript. JSON mirrors the internal storage schema including version, timestamps, language codes, and full message history. The version field is there so future migration scripts can detect schema changes."

---

## 7. Architecture (60 seconds)

> "A few architectural highlights worth calling out:
>
> The backend uses a **provider abstraction layer** — interfaces in the Application layer, Azure adapters in the Infrastructure layer. The Application layer has zero Azure SDK references. Switching from Azure to OpenAI or DeepL requires only a new Infrastructure adapter and a single config value change. No Application or API changes.
>
> The **AWS deployment** uses a same-origin CloudFront proxy pattern. All browser traffic goes through CloudFront at one HTTPS domain. CloudFront routes `/api/*` to Elastic Beanstalk and `/*` to S3. The EB URL is never exposed to the browser — this eliminates mixed-content errors (EB is HTTP-only) and avoids any CORS complexity.
>
> The **Conversation Mode** is a pure frontend orchestration layer. Three existing API endpoints — translate text, translate audio, synthesize speech — are composed into a full conversation experience. No new backend surface was added."

---

## 8. Closing (30 seconds)

> "The project is documented across 19 sprints in the planning folder — 119 decisions, risks, open questions, and acceptance criteria. The portfolio folder has the full case study, architecture overview, and lessons learned.
>
> v1.0 shipped with 133 automated tests, 37 languages, zero third-party UI libraries, and zero authentication required. The entire feature set from landing page to conversation export runs in a single browser tab with no install.
>
> Happy to dig into any specific sprint, the AWS deployment, or the Azure integration in more detail."
