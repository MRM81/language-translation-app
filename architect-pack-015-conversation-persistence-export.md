# Architect Pack: Sprint 015 — Conversation Persistence & Export

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 015 |
| Sprint Name | Conversation Persistence & Export |
| Status | Ready For Builder |
| Depends On | Sprint 014 |
| Created | 2026-05-31 |

---

# 1. Project Context

Sprint 014 delivered Push-To-Talk Conversation UX.

The application now supports:

- Production AWS deployment
- Azure Translation
- Azure Speech-to-Text
- Azure Text-to-Speech
- 37 languages
- Conversation Mode
- Push-To-Talk
- Auto Playback
- Mobile support
- iOS Safari support

Current limitation:

Refreshing the browser causes the active conversation to be lost.

This sprint introduces browser-only persistence and export capabilities.

---

# 2. Sprint Goal

Allow users to preserve, restore, copy, and export conversations while maintaining the project's zero-account, zero-database MVP architecture.

---

# 3. Problem Being Solved

Current:

Conversation
→ Refresh
→ Lost

Target:

Conversation
→ Auto Save
→ Refresh
→ Restored

Conversation
→ Export
→ Share / Archive

---

# 4. Requirements

| ID | Requirement | Priority |
|---|---|---|
| R-001 | Persist active conversation locally | Must |
| R-002 | Restore active conversation automatically | Must |
| R-003 | Export conversation as TXT | Must |
| R-004 | Export conversation as JSON | Must |
| R-005 | Copy conversation to clipboard | Must |
| R-006 | Preserve existing Conversation UX | Must |
| R-007 | Preserve existing Translation Mode | Must |
| R-008 | Mobile support | Must |
| R-009 | No backend changes | Must |
| R-010 | No database required | Must |

---

# 5. In Scope

Frontend only.

- localStorage persistence
- Session restoration
- TXT export
- JSON export
- Clipboard copy
- Conversation metadata
- Browser-only storage

---

# 6. Out Of Scope

Do not implement:

- User accounts
- Authentication
- Databases
- Cloud sync
- Shared conversations
- Multi-device sync
- Server-side persistence

---

# 7. Architecture Principle

Conversation persistence remains entirely client-side.

Conversation State
    ↓
localStorage
    ↓
Restore On Load

No backend changes permitted.

---

# 8. Data Model

```typescript
interface ConversationSession {
  version: number;

  createdAt: string;
  updatedAt: string;

  languageA: string;
  languageB: string;

  messages: ConversationMessage[];
}
```

Storage key:

my-translation-app-conversation

Versioning is required to support future schema changes.

---

# 9. Persistence Strategy

## Save

Any successful conversation update:

Message Added
    ↓
Update Session
    ↓
Persist localStorage

Builder may debounce writes.

## Restore

Application Start
    ↓
Load Session
    ↓
Validate Version
    ↓
Restore Conversation

Invalid data:

→ Ignore
→ Start clean conversation

---

# 10. Export Strategy

## TXT Export

Human-readable format.

Example:

Speaker A (English)
Hello

Translation
Hola

--------------------

Speaker B (Spanish)
¿Cómo estás?

Translation
How are you?

Filename:

conversation-YYYY-MM-DD.txt

---

## JSON Export

Pretty-printed JSON.

Filename:

conversation-YYYY-MM-DD.json

---

## Clipboard Copy

Copy TXT representation.

Success feedback required.

---

# 11. UI Additions

Conversation Header

[ Export TXT ]
[ Export JSON ]
[ Copy ]
[ Clear ]

Clear remains destructive.

Export actions must not clear conversation state.

---

# 12. Files Expected To Change

Modified:

src/frontend/src/components/ConversationMode.tsx
src/frontend/src/types/conversation.ts
src/frontend/src/styles/app.css

Optional:

src/frontend/src/services/ConversationStorageService.ts
src/frontend/src/services/ConversationExportService.ts

Builder may determine final structure.

---

# 13. Validation Rules

- Empty conversations should not export
- Corrupted storage should not crash the application
- Restore must preserve language selections
- Restore must preserve message ordering
- Export output must be deterministic
- Clipboard copy must provide feedback

---

# 14. Validation Plan

Builder must validate:

- Refresh restores conversation
- Browser restart restores conversation
- TXT export downloads correctly
- JSON export downloads correctly
- Clipboard copy works
- Clear removes local storage
- Mobile browsers function correctly
- Corrupted localStorage handled gracefully

---

# 15. Acceptance Criteria

Sprint complete when:

- Conversation survives page refresh
- Conversation survives browser restart
- TXT export works
- JSON export works
- Clipboard copy works
- Clear removes stored conversation
- Mobile verified
- Build passes
- Existing tests pass
- No backend changes introduced

---

# 16. Risks

| Risk | Impact | Mitigation |
|---|---|---|
| localStorage quota exceeded | Low | Single conversation only |
| Corrupted storage | Medium | Version validation |
| Export formatting drift | Low | Shared formatter |
| Browser private mode | Medium | Graceful fallback |

---

# 17. Decisions To Add

D-098 Browser-only persistence via localStorage

D-099 Single active conversation only

D-100 TXT is canonical export format

D-101 JSON export mirrors ConversationSession structure

---

# 18. Questions To Track

Q-055 Should multiple saved conversations be supported?

Q-056 Should conversations be named?

Q-057 Should PDF export be added later?

---

# 19. Builder Dry Run Instructions

1. Read Sprint 014 outputs.
2. Review ConversationMode state model.
3. Review current clear workflow.
4. Determine persistence approach.
5. Determine export approach.
6. Produce dry run report.
7. Wait for approval.

Do not implement immediately.

---

# 20. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 015 — Conversation Persistence & Export.

Read Sprint 014 outputs first.

Perform a dry run before implementation.

Implement:

- Browser persistence
- Session restore
- TXT export
- JSON export
- Clipboard copy

Do not implement:

- Databases
- Authentication
- Backend APIs
- Cloud storage

Focus on reliability, usability, and export quality.

Provide a dry run report before implementation.

---

# 21. Completion Report Requirements

Provide:

- Files created
- Files modified
- Build results
- Test results
- Persistence validation results
- Export validation results
- Decisions added
- Risks added
- Questions added

Recommended next sprint:

Sprint 016 — Multi-Conversation Management
