# Architect Pack: Sprint 017 — Conversation Search & Demo Polish

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 017 |
| Sprint Name | Conversation Search & Demo Polish |
| Status | Ready For Builder |
| Depends On | Sprint 016 |
| Created | 2026-05-31 |

---

# 1. Project Context

Sprint 016 completed browser-only multi-conversation management.

The project is now on the final path to v1.0 completion:

```text
017 Conversation Search & Demo Polish
018 UI/UX Redesign
019 Production Refresh & Portfolio Assets
```

Current capabilities:

- Live AWS production deployment
- Azure Translation
- Azure Speech-to-Text
- Azure Text-to-Speech
- 37 supported languages
- Conversation Mode
- Push-To-Talk
- Auto Playback
- Conversation persistence
- Multiple saved conversations
- TXT / JSON / Clipboard export

Current limitation:

Conversation management works, but the demo and management experience can be polished:

- Finding conversations becomes harder as saved conversations grow.
- Default conversation titles may not be descriptive enough.
- Conversation list entries lack useful preview information.
- Empty states and demo flow need final polish before the UI redesign sprint.

---

# 2. Sprint Goal

Improve the multi-conversation experience with search, better titles, preview snippets, empty states, and demo-focused polish while avoiding major new architecture or backend work.

---

# 3. Problem Being Solved

Current:

```text
Conversation List
    ↓
Many saved conversations
    ↓
Hard to find the right one
```

Target:

```text
Search conversations
    ↓
Preview matching conversations
    ↓
Open quickly
```

The sprint should make the app feel more finished and easier to demonstrate.

---

# 4. Requirements

| ID | Requirement | Priority | Notes |
|---|---|---|---|
| R-001 | Add conversation search | Must | Search by title and message text |
| R-002 | Add conversation preview snippets | Must | Show recent message or translated text |
| R-003 | Improve default conversation titles | Must | Use useful auto-title where possible |
| R-004 | Improve empty states | Must | Conversation list, no search results, empty active conversation |
| R-005 | Preserve all Sprint 016 functionality | Must | Create/switch/rename/delete/export |
| R-006 | Preserve Push-To-Talk workflow | Must | No regression |
| R-007 | Preserve Translation Mode | Must | No regression |
| R-008 | Keep browser-only storage | Must | No backend/database |
| R-009 | Mobile usability | Must | Search and list usable on small screens |
| R-010 | Prepare for Sprint 018 UI redesign | Should | Avoid over-styling; keep structure clean |

---

# 5. In Scope

Frontend only.

- Conversation search input
- Filtering conversations by title and content
- Conversation preview snippets
- Better default titles
- Empty states
- Minor demo polish
- Small CSS refinements
- Documentation/planning updates

---

# 6. Out Of Scope

Do not implement:

- Backend search
- Database indexing
- Cloud sync
- Authentication
- Conversation sharing
- Full UI redesign
- New translation features
- New AI services
- Analytics
- PDF export

These are either future work or intentionally excluded from v1.0.

---

# 7. Architecture Principle

Search and polish remain client-side only.

```text
ConversationManager
    ↓
ConversationStorageService
    ↓
localStorage ConversationStore
```

No backend involvement.

Search may be computed from the locally loaded conversation store.

---

# 8. Search Behaviour

Search should match:

- Conversation title
- Original text
- Translated text
- Language names or codes if simple to support

Minimum search:

```text
title + originalText + translatedText
```

Search should be:

- case-insensitive
- whitespace-trimmed
- local only
- instant for current localStorage data size

No debounce required unless implementation naturally benefits from it.

---

# 9. Conversation Preview Snippets

Conversation list should show useful context for each saved conversation.

Recommended preview priority:

1. Last translated text
2. Last original text
3. "No messages yet"

Example:

```text
Travel Spanish
Last: "Where is the train station?"
3 messages · English ↔ Spanish
```

Preview should remain short and avoid crowding mobile layouts.

---

# 10. Default Title Strategy

Current auto-title:

```text
Conversation N
```

Sprint 017 should improve this where practical.

Recommended approach:

- New empty conversations may still start as `Conversation N`.
- Once the first message is added, auto-title may become based on the first original text if the user has not manually renamed it.
- If user manually renamed the conversation, never overwrite it.

Example:

```text
Original: "Where is the nearest train station?"
Title: "Where is the nearest train..."
```

To support this, Builder may add a flag such as:

```typescript
isAutoTitle: boolean
```

to ConversationSession, or use a simple title convention if safer.

Builder should choose the least risky approach and explain it in the dry run.

---

# 11. Empty States

Add or improve empty states for:

## No Conversations

```text
No saved conversations yet.
Start a new conversation to begin.
```

## Empty Active Conversation

```text
No messages yet.
Use Record, Text, or Audio File to start.
```

## No Search Results

```text
No conversations match your search.
Try a different word or clear the search.
```

Empty states should be friendly, short, and mobile-safe.

---

# 12. Demo Polish

The sprint should improve demo readiness without overbuilding.

Suggested polish:

- Ensure the active conversation is visually clear.
- Show message counts in conversation list.
- Show updated time/date where simple.
- Make search easy to clear.
- Keep actions visually tidy on mobile.
- Avoid large layout changes before Sprint 018.

---

# 13. Files Expected To Change

Likely modified:

```text
src/frontend/src/components/ConversationManager.tsx
src/frontend/src/components/ConversationHistory.tsx
src/frontend/src/components/ConversationMode.tsx
src/frontend/src/services/ConversationStorageService.ts
src/frontend/src/types/conversation.ts
src/frontend/src/styles/app.css
```

Optional new helper:

```text
src/frontend/src/services/ConversationSearchService.ts
src/frontend/src/services/ConversationTitleService.ts
```

Builder should decide whether helpers are warranted.

Keep the implementation practical and not over-engineered.

---

# 14. Data Model Notes

Potential additions:

```typescript
interface ConversationSession {
  // existing fields...
  title: string;
  isAutoTitle?: boolean;
}
```

or:

```typescript
interface ConversationSummary {
  // existing fields...
  previewText?: string;
  matched?: boolean;
}
```

Builder should avoid data-model churn unless it improves clarity.

---

# 15. Validation Rules

- Search must not mutate stored conversations.
- Search must not break switching conversations.
- Search must not hide the active conversation unexpectedly if the query changes after switching.
- Manual conversation titles must not be overwritten.
- Empty conversations must still be visible unless filtered out by search.
- Existing Sprint 016 migration must remain valid.
- Existing export functions must continue to work.

---

# 16. Validation Plan

Builder must validate:

- Search by conversation title
- Search by original message text
- Search by translated message text
- Clear search
- No results state
- Empty conversation state
- Auto-title behavior
- Manual rename remains respected
- Conversation switching after search
- Delete after search
- Export after search
- Mobile layout
- Existing Translation Mode

---

# 17. Acceptance Criteria

Sprint complete when:

- [ ] Conversation search exists
- [ ] Search matches titles
- [ ] Search matches original message text
- [ ] Search matches translated message text
- [ ] Conversation preview snippets are shown
- [ ] Message counts are shown
- [ ] Empty states are improved
- [ ] Default titles are improved or documented as intentionally unchanged
- [ ] Manual renames are preserved
- [ ] Conversation switching works after filtering
- [ ] Delete works after filtering
- [ ] Export TXT still works
- [ ] Export JSON still works
- [ ] Clipboard copy still works
- [ ] Push-To-Talk workflow preserved
- [ ] Translation Mode preserved
- [ ] Mobile layout verified
- [ ] Build passes
- [ ] Existing tests pass
- [ ] No backend changes introduced

---

# 18. Risks

| Risk | Impact | Mitigation |
|---|---|---|
| Search logic complicates ConversationManager | Medium | Extract helper if component becomes too large |
| Auto-title overwrites user titles | Medium | Track manual rename or conservative title update |
| Mobile UI crowding | Medium | Stack controls and keep preview short |
| Filtering breaks active selection | Medium | Keep active state independent from filtered list |
| Demo polish turns into redesign | Medium | Defer full visual redesign to Sprint 018 |

---

# 19. Decisions To Add

D-106 Conversation search remains client-side over localStorage data.

D-107 Conversation search matches title, original text, and translated text.

D-108 Manual conversation titles are never overwritten by auto-title logic.

D-109 Full visual redesign is deferred to Sprint 018; Sprint 017 is polish only.

---

# 20. Questions To Track

Q-062 Should search include language names and language codes?

Q-063 Should conversation previews show original text, translated text, or both?

Q-064 Should auto-title use first message, latest message, or remain manual only?

Q-065 Should search results be highlighted in a future sprint?

---

# 21. State Updates Required

At completion, update:

```text
planning/STATE.md
planning/DECISIONS.md
planning/RISKS.md
planning/QUESTIONS.md
planning/FILE_INVENTORY.md
planning/sprints/017-conversation-search-demo-polish/
```

Create sprint files:

```text
requirements.md
blueprint.md
acceptance.md
handoff-prompt.md
```

---

# 22. Builder Dry Run Instructions

Before implementation:

1. Read Sprint 016 outputs.
2. Review ConversationManager.
3. Review ConversationStorageService.
4. Review ConversationMode.
5. Identify search strategy.
6. Identify title strategy.
7. Identify preview strategy.
8. Produce dry run report.
9. Wait for approval.

Do not implement immediately.

---

# 23. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 017 — Conversation Search & Demo Polish.

Read Sprint 016 outputs first.

Perform a dry run before implementation.

Implement frontend-only polish:

- Conversation search
- Conversation preview snippets
- Better default titles where safe
- Better empty states
- Demo usability polish

Do not implement:

- Backend changes
- Database search
- Authentication
- Cloud sync
- Full UI redesign

Preserve:

- Multi-conversation management
- Push-To-Talk
- Conversation persistence
- TXT export
- JSON export
- Clipboard copy
- Translation Mode

Provide a dry run report before implementation.

---

# 24. Completion Report Requirements

Provide:

- Files created
- Files modified
- Build results
- Test results
- Search validation results
- Title behavior validation
- Preview validation
- Mobile validation
- Decisions added
- Risks added
- Questions added

Recommended next sprint:

Sprint 018 — UI/UX Redesign
