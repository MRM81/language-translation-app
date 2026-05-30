# Sprint 008 Blueprint — UX Modernization, Mobile Responsiveness & Portfolio Showcase

## Phase 1 — Planning & Baseline

- Update planning/STATE.md, DECISIONS.md, RISKS.md, QUESTIONS.md
- Create sprint folder and files
- Create design/ and portfolio/ folder structures
- **Capture baseline screenshots before any CSS changes**
- Target breakpoints: 320px, 375px, 768px, 1024px, 1440px

---

## Phase 2 — CSS Overhaul

File: `src/frontend/src/styles/app.css`

Changes:
- Add CSS custom properties at `:root` (color system, radius scale)
- Change body background from `#f5f5f5` to `var(--bg)` dark navy
- Update `.app-header` to gradient hero (dark navy → indigo)
- Update `.translation-form` cards to dark surface with border glow
- Update `.btn-primary` to indigo accent color
- Add `.lang-pair-row` layout (source + swap button + target in one row)
- Add `.btn-swap` styles
- Update `.btn-record` and `.btn-stop-recording` to circular (72×72px)
- Update `.panel-result` to dark surface with prominent translation hero
- Add `.result-hero` for large translation text display
- Add `.result-meta` for small de-emphasized footer (provider, correlationId)
- Update `.panel-error` for dark theme
- Add `:focus-visible` styles for all interactive elements
- Responsive improvements at 480px and 320px breakpoints

---

## Phase 3 — Component Updates

### App.tsx
- Add `aria-live="polite"` to `.results-area` div

### ResultPanel.tsx
- Hero layout: translation text displayed large and prominently
- Transcript shown as secondary with label
- Provider and correlationId moved to small meta footer row
- Play button gets ▶ icon prefix
- Idle play state still shows "Play" with icon; loading "Loading…"; playing "Playing…"

### PushToTalkButton.tsx
- Add inline SVG mic icon for idle/uploading state
- Keep stop square icon (&#9632;) for recording state
- No logic changes — button class names preserved

### TextTranslationForm.tsx
- Wrap both LanguageSelect calls in `.lang-pair-row` div
- Add swap ⇄ button with `handleSwap` function between them
- No other logic changes

### AudioTranslationForm.tsx
- Same lang-pair-row treatment as TextTranslationForm
- No other logic changes

### index.html
- Add `<meta name="description">` tag

---

## Phase 4 — Portfolio Documentation

- `portfolio/case-study.md` — full technical write-up
- `portfolio/architecture-overview.md` — system diagram and stack
- `portfolio/demo-script.md` — 2–3 minute walkthrough
- `portfolio/lessons-learned.md` — sprint-by-sprint learnings
- `design/style-guide.md` — color, typography, spacing reference
- `design/decisions.md` — design decision log

---

## Phase 5 — Verification

- `npx tsc --noEmit` — TypeScript check clean
- `npm run build` — frontend build clean
- `dotnet test` — 71/71 backend tests pass
- Manual responsive validation at 320px, 375px, 768px, 1024px, 1440px
- Final screenshots captured
- All Sprint 001–007 workflows verified functional
