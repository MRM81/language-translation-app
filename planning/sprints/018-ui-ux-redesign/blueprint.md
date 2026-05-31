# Sprint 018 — Blueprint

**Sprint:** 018 — UI/UX Redesign
**Status:** Completed
**Completed:** 2026-05-31

---

## State Model

```typescript
screen: 'landing' | 'workspace'   // 'landing' is default
mode: 'translate' | 'conversation' // existing — unchanged
translationInputMode: 'text' | 'audio' // new — 'text' is default
```

No routing library. Pure React state in App.tsx.

---

## App Flow

```
Landing Page (screen === 'landing')
    ↓ Start Translating
Workspace (screen === 'workspace')
    ├─ Translation Mode
    │   ├─ [ Text Translation ] [ Audio Translation ] toggle
    │   └─ Only selected form visible
    └─ Conversation Mode
```

---

## Files Created

- `src/frontend/public/logo.png` — copied from design/inspiration/logo.png
- `src/frontend/public/favicon.png` — copied from design/inspiration/favicon.png
- `src/frontend/src/components/LandingPage.tsx` — landing hero component

---

## Files Modified

- `src/frontend/index.html` — favicon link tag added
- `src/frontend/src/App.tsx` — screen state, translationInputMode state, LandingPage render, workspace header, translation tabs
- `src/frontend/src/styles/app.css` — landing styles, translation tabs, header refactor, mobile overrides

---

## CSS Strategy

- `.landing-page` / `.landing-hero` / `.landing-logo` / `.landing-description` / `.landing-cta` / `.landing-features` / `.landing-tech` — new landing page styles
- `.translation-tabs` / `.translation-tab` — new segmented toggle (mirrors `.audio-tabs` pattern)
- `.app-header-inner` / `.app-header-logo` / `.app-header-tagline` — workspace header logo layout
- `.forms-grid` — removed (no longer needed; one form displayed at a time)
