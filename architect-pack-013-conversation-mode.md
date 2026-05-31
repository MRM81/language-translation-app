# Architect Pack: Sprint 013 — Conversation Mode

## Pack Metadata

| Field         | Value              |
| ------------- | ------------------ |
| Project Name  | My Translation App |
| Sprint Number | 013                |
| Sprint Name   | Conversation Mode  |
| Status        | Ready For Builder  |
| Depends On    | Sprint 012         |
| Created       | 2026-05-31         |

---

# 1. Project Context

Sprint 012 successfully deployed the application to production.

The application now supports:

* Live AWS deployment
* Azure Translation
* Azure Speech-to-Text
* Azure Text-to-Speech
* 37 languages
* Health monitoring
* Production operations

Current workflow:

```text
Input
   ↓
Translate
   ↓
Result
   ↓
Optional Playback
```

The next major evolution is real-time conversational translation.

---

# 2. Sprint Goal

Introduce a Conversation Mode that enables two people speaking different languages to communicate naturally through the application.

The focus is MVP conversation workflows, not real-time streaming.

---

# 3. Problem Being Solved

Current translation workflow is optimized for one-off translations.

Real-world use cases require:

* turn-taking conversations
* repeated translations
* automatic playback
* faster interaction flow

Without Conversation Mode, users must manually repeat the translation workflow for every sentence.

---

# 4. Sprint Objectives

Create a dedicated Conversation Mode that supports:

```text
Person A speaks
      ↓
Translate
      ↓
Auto-play in Person B language

Person B replies
      ↓
Translate
      ↓
Auto-play in Person A language
```

---

# 5. Requirements

| ID    | Requirement                              | Priority |
| ----- | ---------------------------------------- | -------- |
| R-001 | Add Conversation Mode UI                 | Must     |
| R-002 | Support two participant languages        | Must     |
| R-003 | Support text conversation workflow       | Must     |
| R-004 | Support audio file conversation workflow | Must     |
| R-005 | Auto-play translated speech              | Must     |
| R-006 | Show conversation history                | Must     |
| R-007 | Support role swapping                    | Must     |
| R-008 | Preserve existing translation mode       | Must     |
| R-009 | Mobile responsive design                 | Must     |
| R-010 | Accessibility maintained                 | Must     |

---

# 6. In Scope

### Frontend

* Conversation Mode screen
* Participant language selectors
* Conversation history
* Auto-play workflow
* Swap languages button

### Backend

* Reuse existing APIs
* No new providers
* No new AI services

### UX

* Fast turn-taking workflow
* Mobile-first layout

---

# 7. Out Of Scope

Do not implement:

* Real-time streaming audio
* WebSockets
* Continuous microphone recording
* Live simultaneous translation
* User accounts
* Conversation persistence
* Multi-user sessions
* Voice cloning
* Speaker identification

These belong to future sprints.

---

# 8. Architecture Principle

Conversation Mode must be:

```text
UI Layer
      ↓
Existing Translation APIs
      ↓
Existing Azure Providers
```

No duplicate translation logic.

No duplicate TTS logic.

No duplicate STT logic.

Conversation Mode orchestrates existing capabilities.

---

# 9. User Workflow

## Text Conversation

```text
English Speaker
      ↓
Enter Text
      ↓
Translate
      ↓
Spanish Output
      ↓
Auto-play

Spanish Speaker
      ↓
Reply
      ↓
Translate
      ↓
English Output
      ↓
Auto-play
```

---

## Audio Conversation

```text
English Speaker
      ↓
Upload Audio
      ↓
STT
      ↓
Translation
      ↓
TTS
      ↓
Playback
```

---

# 10. Conversation Data Model

Frontend only.

No persistence.

```typescript
interface ConversationMessage {
  id: string;
  speaker: 'A' | 'B';

  originalText: string;
  translatedText: string;

  sourceLanguage: string;
  targetLanguage: string;

  timestamp: string;

  inputType: 'text' | 'audio';
}
```

Session lifetime only.

---

# 11. UI Layout

## Desktop

```text
+----------------------------------+
| Conversation Mode               |
+----------------------------------+

Speaker A Language [English]

Speaker B Language [Spanish]

[ Swap Languages ]

------------------------------------

Conversation History

A: Hello
   Hola

B: ¿Cómo estás?
   How are you?

------------------------------------

Text Input

[ Message ]

[ Translate & Speak ]
```

---

## Mobile

```text
Language A
Language B

[ Swap ]

History

Input

Translate
```

Single-column responsive layout.

---

# 12. Auto Playback Rules

When translation succeeds:

```text
Translation
      ↓
TTS
      ↓
Auto Play
```

Playback should be:

* enabled by default
* toggleable

Settings:

```text
☑ Auto Play Responses
```

---

# 13. Files Expected To Change

Frontend:

```text
src/frontend/src/App.tsx

src/frontend/src/components/
    ConversationMode.tsx
    ConversationHistory.tsx
    ConversationInput.tsx
    ConversationMessage.tsx
```

Optional:

```text
src/frontend/src/types/
    conversation.ts
```

Backend:

None expected.

Reuse existing endpoints.

---

# 14. API Usage

Reuse existing APIs:

```text
POST /api/translate/text

POST /api/translate/speech-to-text

POST /api/translate/tts

GET /api/languages
```

No new controllers.

No new endpoints.

---

# 15. Validation Plan

Builder must validate:

### Text Workflow

```text
English → Spanish
Spanish → English
```

### Audio Workflow

```text
Audio Upload
      ↓
Transcript
      ↓
Translation
      ↓
Playback
```

### Swap Workflow

```text
English ↔ Spanish
```

### Mobile

Responsive layout verified.

---

# 16. Acceptance Criteria

Sprint complete when:

* [ ] Conversation Mode screen exists
* [ ] Two language selectors exist
* [ ] Swap language button exists
* [ ] Text workflow functions
* [ ] Audio workflow functions
* [ ] Conversation history displayed
* [ ] Auto-play works
* [ ] Existing translation mode preserved
* [ ] Mobile layout verified
* [ ] Build passes
* [ ] Tests pass

---

# 17. Risks

| Risk                       | Impact | Mitigation           |
| -------------------------- | ------ | -------------------- |
| Conversation UI complexity | Medium | Keep MVP simple      |
| Excessive state management | Medium | Session-only state   |
| Mobile layout crowding     | Medium | Single-column design |
| TTS delays affect UX       | Low    | Show loading state   |

---

# 18. Decisions To Add

Add:

```text
D-090
D-091
D-092
D-093
```

Conversation Mode decisions.

---

# 19. Questions To Track

Add:

```text
Q-049
```

Should Conversation Mode support microphone recording in a future sprint?

```text
Q-050
```

Should conversations be saved?

```text
Q-051
```

Should conversations support multiple participants?

---

# 20. Builder Dry Run Instructions

Before implementation:

1. Read Sprint 012 outputs.
2. Review existing translation UI.
3. Review TTS workflow.
4. Review STT workflow.
5. Identify reusable components.
6. Identify state management approach.
7. Produce dry run report.
8. Wait for approval.

Do not implement immediately.

---

# 21. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 013 — Conversation Mode.

Read all current architecture and Sprint 012 deployment outputs first.

Perform a dry run before implementation.

Reuse existing:

* Translation APIs
* STT APIs
* TTS APIs
* Language APIs

Do not create new providers.

Do not create new backend endpoints.

Conversation Mode must be a frontend orchestration layer over existing functionality.

Focus on:

* usability
* mobile responsiveness
* turn-taking conversation workflow

Produce a dry run report before implementation.

---

# 22. Completion Report Requirements

Provide:

* files created
* files modified
* build results
* test results
* mobile validation results
* conversation workflow validation
* decisions added
* risks added
* questions added

Recommended next sprint:

```text
Sprint 014 — Push-To-Talk Conversation UX
```
