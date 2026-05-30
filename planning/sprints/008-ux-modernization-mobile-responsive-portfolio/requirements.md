# Sprint 008 Requirements — UX Modernization, Mobile Responsiveness & Portfolio Showcase

## In Scope

### Visual Modernization

- CSS custom properties at `:root` for full color and radius system
- Dark navy full-page theme (body, cards, header)
- Indigo accent color for all primary CTA buttons
- Improved card elevation and border treatment
- Improved typography hierarchy
- Improved loading and error states
- Visible focus-visible styles for keyboard navigation

### Mobile Responsiveness

- 320px — single column, circular record button, compact language pair row
- 375px — single column, comfortable padding
- 768px — two-column form grid
- 1024px — two-column, centered container
- 1440px+ — max-width 960px centered

### Translation Experience Improvements

- Language pair row with swap ⇄ button (source ↔ target, trivial state swap within each form)
- Large circular record button (min 72×72px) with mic icon
- Hero translation result display (translation text as primary visual element)
- De-emphasized meta rows (provider, correlation ID moved to small footer)
- Play button with ▶ icon and improved state feedback

### Accessibility

- `aria-live="polite"` on results area (screen reader notification on translation arrival)
- Explicit `:focus-visible` styles on all interactive elements
- `aria-required` on target language selects
- All interactive elements meet 44px minimum touch target height
- WCAG AA color contrast verified for all text/background combinations

### Portfolio Assets

- `portfolio/case-study.md`
- `portfolio/architecture-overview.md`
- `portfolio/demo-script.md`
- `portfolio/lessons-learned.md`

### Design Documentation

- `design/style-guide.md`
- `design/decisions.md`
- `design/screenshots/sprint-008-baseline/` — before screenshots
- `design/screenshots/sprint-008-final/` — after screenshots
- `design/screenshots/portfolio/` — hero shots for portfolio use

---

## Out Of Scope

- New translation providers
- Authentication or user accounts
- Persistent storage
- Conversation or history mode
- Dark mode toggle (future sprint — Q-032)
- App name / branding change (pending Q-033)
- Any backend changes
- New npm packages or CSS frameworks
