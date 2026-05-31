# Architect Pack: Sprint 016 — Multi-Conversation Management

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 016 |
| Sprint Name | Multi-Conversation Management |
| Status | Ready For Builder |
| Depends On | Sprint 015 |
| Created | 2026-05-31 |

---

# 1. Project Context

Sprint 015 introduced browser-only persistence and export for a single active conversation.

Current capabilities:

- Live AWS production deployment
- Azure Translation
- Azure Speech-to-Text
- Azure Text-to-Speech
- 37 supported languages
- Conversation Mode
- Push-To-Talk
- Auto Playback
- Single conversation persistence
- TXT export
- JSON export
- Clipboard copy

Current limitation:

Only one active conversation can be saved at a time.

This sprint introduces browser-only multi-conversation management.

---

# 2. Sprint Goal

Allow users to create, save, name, switch between, and delete multiple conversations in the browser without adding accounts, databases, or backend storage.

---

# 3. Problem Being Solved

Current:

Conversation A
    ↓
New conversation
    ↓
Conversation A overwritten

Target:

Conversation List
    ├── Conversation A
    ├── Conversation B
    └── Conversation C

User can switch between saved conversations locally.

---

# 4. Requirements

| ID | Requirement | Priority |
|---|---|---|
| R-001 | Support multiple locally saved conversations | Must |
| R-002 | Allow user to create a new conversation | Must |
| R-003 | Allow switching between conversations | Must |
| R-004 | Allow conversation deletion | Must |
| R-005 | Allow conversation renaming | Must |
| R-006 | Restore last active conversation on reload | Must |
| R-007 | Preserve existing export features | Must |
| R-008 | Preserve existing Push-To-Talk workflow | Must |
| R-009 | Preserve existing Translation Mode | Must |
| R-010 | No backend changes | Must |

---

# 5. In Scope

Frontend only.

- Local conversation index
- Multiple stored conversations
- Conversation create/switch/delete/rename
- Last active conversation restore
- Migration from Sprint 015 single-conversation storage if present
- Existing export functions continue to work for active conversation

---

# 6. Out Of Scope

Do not implement:

- User accounts
- Authentication
- Cloud sync
- Database storage
- Shared conversations
- Multi-device sync
- Real-time collaboration
- Server-side conversation APIs

---

# 7. Architecture Principle

Multi-conversation management remains browser-only.

ConversationMode
    ↓
ConversationStorageService
    ↓
localStorage

No backend involvement.

---

# 8. Data Model

## Conversation Summary

```typescript
interface ConversationSummary {
  id: string;
  title: string;
  createdAt: string;
  updatedAt: string;
  messageCount: number;
  languageA: string;
  languageB: string;
}
```

## Conversation Store

```typescript
interface ConversationStore {
  version: number;
  activeConversationId: string | null;
  conversations: Record<string, ConversationSession>;
}
```

Storage key:

```text
my-translation-app-conversations
```

Legacy key from Sprint 015:

```text
my-translation-app-conversation
```

must be considered for one-time migration.

---

# 9. Persistence Strategy

## Create Conversation

New Conversation
    ↓
Generate ID
    ↓
Create empty session
    ↓
Set activeConversationId
    ↓
Save store

## Switch Conversation

Select Conversation
    ↓
Load session from store
    ↓
Set activeConversationId
    ↓
Restore ConversationMode state

## Rename Conversation

Edit Title
    ↓
Update session metadata
    ↓
Save store

## Delete Conversation

Delete Conversation
    ↓
Remove from store
    ↓
If deleted active conversation:
        load most recent remaining conversation
        OR create blank conversation

---

# 10. Migration Strategy

If Sprint 015 single-conversation storage exists:

1. Read legacy key: `my-translation-app-conversation`
2. Convert it to first conversation in new store
3. Assign default title, such as `Conversation 1`
4. Set it as active
5. Remove legacy key after successful migration

If migration fails:

- Ignore corrupted legacy data
- Start with empty conversation store
- Do not crash the app

---

# 11. UI Design

Conversation Mode header should include:

```text
[ Conversation: Conversation 1 ▼ ] [ New ] [ Rename ] [ Delete ]
```

Alternative mobile layout:

```text
Conversation
[Dropdown]

[New] [Rename] [Delete]
```

Existing actions remain:

```text
[ Export TXT ] [ Export JSON ] [ Copy ] [ Clear ]
```

Important distinction:

- Clear = clears messages in current conversation
- Delete = removes entire conversation from local storage

---

# 12. UX Rules

- Creating a new conversation starts empty with current language defaults.
- Switching conversations restores that conversation's messages and language pair.
- Renaming should not affect messages.
- Deleting should require confirmation.
- Last active conversation restores on reload.
- Empty conversations may exist, but should be titled clearly.

---

# 13. Files Expected To Change

Likely modified:

```text
src/frontend/src/components/ConversationMode.tsx
src/frontend/src/types/conversation.ts
src/frontend/src/services/ConversationStorageService.ts
src/frontend/src/styles/app.css
```

Optional new component:

```text
src/frontend/src/components/ConversationManager.tsx
```

Optional new helper:

```text
src/frontend/src/services/ConversationTitleService.ts
```

Builder should choose the simplest maintainable structure.

---

# 14. Export Behavior

Export functions continue to operate on the active conversation only.

TXT and JSON export should include conversation title if available.

JSON export should include full ConversationSession data.

---

# 15. Validation Rules

- Store version must be validated.
- Active conversation ID must exist in the store.
- Corrupted store must not crash app.
- Legacy single-conversation migration must be safe.
- Delete active conversation must leave UI in valid state.
- Rename cannot save empty title; fallback to previous title or default.

---

# 16. Validation Plan

Builder must validate:

- Create new conversation
- Rename conversation
- Switch between conversations
- Delete inactive conversation
- Delete active conversation
- Reload restores last active conversation
- Legacy Sprint 015 conversation migrates
- Export still works
- Copy still works
- Clear only clears active conversation
- Mobile layout remains usable

---

# 17. Acceptance Criteria

Sprint complete when:

- [ ] Multiple conversations can be stored locally
- [ ] User can create a new conversation
- [ ] User can switch between conversations
- [ ] User can rename conversations
- [ ] User can delete conversations
- [ ] Last active conversation restores on reload
- [ ] Legacy Sprint 015 storage migrates safely
- [ ] Export TXT still works
- [ ] Export JSON still works
- [ ] Copy still works
- [ ] Clear affects only active conversation
- [ ] Existing Translation Mode preserved
- [ ] Existing Push-To-Talk workflow preserved
- [ ] Build passes
- [ ] Existing tests pass
- [ ] No backend changes introduced

---

# 18. Risks

| Risk | Impact | Mitigation |
|---|---|---|
| localStorage size growth | Medium | Browser-only MVP; document limitation |
| Confusing Clear vs Delete | Medium | Clear labels and confirmation |
| Migration bug | Medium | Defensive parsing and validation |
| Mobile UI crowding | Medium | Responsive stacked controls |
| Corrupted conversation store | Medium | Version and shape validation |

---

# 19. Decisions To Add

D-102 Browser-only multi-conversation management remains localStorage-based.

D-103 One active conversation is restored on reload using activeConversationId.

D-104 Sprint 015 single-conversation storage is migrated once if present.

D-105 Clear and Delete are distinct actions: Clear removes messages, Delete removes conversation.

---

# 20. Questions To Track

Q-058 Should users be able to search conversations?

Q-059 Should conversations support folders or tags?

Q-060 Should cloud sync be considered after authentication exists?

Q-061 Should export all conversations be added later?

---

# 21. Builder Dry Run Instructions

Before implementation:

1. Read Sprint 015 outputs.
2. Review ConversationStorageService.
3. Review ConversationExportService.
4. Review ConversationMode state model.
5. Identify the least disruptive storage migration path.
6. Identify UI placement for conversation management controls.
7. Produce dry run report.
8. Wait for approval.

Do not implement immediately.

---

# 22. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 016 — Multi-Conversation Management.

Read Sprint 015 outputs first.

Perform a dry run before implementation.

Implement browser-only multi-conversation management:

- Create conversation
- Switch conversation
- Rename conversation
- Delete conversation
- Restore last active conversation
- Migrate Sprint 015 single-conversation storage

Do not add backend endpoints.

Do not add a database.

Do not add authentication.

Do not add cloud sync.

Preserve:

- Conversation Mode
- Push-To-Talk
- Export TXT
- Export JSON
- Clipboard copy
- Translation Mode

Provide a dry run report before implementation.

---

# 23. Completion Report Requirements

Provide:

- Files created
- Files modified
- Build results
- Test results
- Migration validation results
- Multi-conversation validation results
- Export regression validation
- Decisions added
- Risks added
- Questions added

Recommended next sprint:

Sprint 017 — Conversation Search, Titles & Demo Polish
