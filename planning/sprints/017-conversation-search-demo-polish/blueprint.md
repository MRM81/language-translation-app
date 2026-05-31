# Sprint 017 — Blueprint

## Changes Made

### conversation.ts
- `ConversationSession` — added `isAutoTitle?: boolean`
- `ConversationSummary` — added `previewText?: string` (computed, not stored)

### ConversationStorageService.ts
- `createConversation` — sets `isAutoTitle: true` on new sessions
- `renameConversation` — sets `isAutoTitle: false` (manual rename protection)
- `getConversationSummaries` — computes `previewText` from last message (translatedText preferred, fallback originalText)

### ConversationManager.tsx
- Props added: `searchQuery`, `onSearchChange`, `allCount`
- Renders search input + clear button (shown when >1 conversation or search active)
- Enhanced `<select>` option labels: `"Title (N) · preview…"`
- "No conversations match your search" empty state with inline Clear Search link
- `truncate()` helper for preview text (max 35 chars)
- `buildOptionLabel()` helper for option text

### ConversationMode.tsx
- State added: `searchQuery`, `isAutoTitle`
- `filteredSummaries` computed via `useMemo` — full-text search across all message text via `loadStore()`
- Active conversation always included in filtered list (prevents select value mismatch)
- `applyAutoTitleIfNeeded()` — fires on first message if `isAutoTitle === true`; sets title from first 40 chars of `originalText`; sets `isAutoTitle = false`
- All handlers updated: `handleNew`, `handleSwitch`, `handleRename`, `handleDelete` restore/reset `isAutoTitle`
- `handleNew` clears `searchQuery`
- Save effect includes `isAutoTitle` in persisted session

### ConversationHistory.tsx
- Empty state text: "No messages yet. Use Record, Text, or Audio File to start."

### app.css
- `.conv-search-wrap` — flex row for search input + clear button
- `.conv-search-input` — styled text input (36px height, accent focus ring)
- `.conv-search-clear` — × clear button
- `.conv-empty-search` — muted italic no-results message
- `.conv-search-reset-link` — inline button styled as accent link
- Mobile override: `.conv-search-wrap { width: 100% }` at ≤480px

## Architecture Notes

- Search is client-side only. `loadStore()` is called during filtering — acceptable at localStorage scale.
- `isAutoTitle` is an optional field. Old sessions without it load correctly (`undefined` → treated as `false`).
- `isValidSession` validator does not check `isAutoTitle` — extra fields pass through safely.
- No CONVERSATION_STORE_VERSION bump needed.
- No migration needed.
