# Architect Pack: Sprint 018 — UI/UX Redesign

## Pack Metadata

| Field | Value |
|---|---|
| Project Name | My Translation App |
| Sprint Number | 018 |
| Sprint Name | UI/UX Redesign |
| Status | Ready For Builder |
| Depends On | Sprint 017 |
| Created | 2026-05-31 |
| Sprint Type | Final UX / Visual Polish |

---

# 1. Project Context

The project is now on its final v1.0 completion path:

```text
017 Conversation Search & Demo Polish   ✅
018 UI/UX Redesign                      ← Current
019 Production Refresh & Portfolio Assets
```

Current capabilities:

- Live AWS production deployment
- Azure Translation
- Azure Speech-to-Text
- Azure Text-to-Speech
- 37 supported languages
- Translation Mode
- Conversation Mode
- Push-To-Talk
- Auto Playback
- Multi-conversation management
- Conversation search
- Persistence
- TXT / JSON / Clipboard export

Sprint 018 should focus on presentation, usability, navigation, and final product polish.

Do not add major new product features.

---

# 2. Sprint Goal

Redesign the user experience so the app feels like a polished portfolio-ready product rather than a feature-heavy prototype.

The redesign must introduce:

- Landing page
- Logo usage
- Favicon usage
- Cleaner navigation
- Improved Translation Mode layout
- Better mobile presentation
- Polished visual hierarchy

---

# 3. User Requirements From Project Owner

The project owner requested:

1. Add logo from:

```text
design/inspiration/logo.png
```

2. Add favicon from:

```text
design/inspiration/favicon.png
```

3. Add a landing page as the default first screen.

4. Landing page should include:
   - short description of the app
   - logo
   - button that takes the user to the translator

5. Translation section should not show text and audio translation at the same time.

6. Translation section should use a toggle between:
   - Text Translation
   - Audio Translation

7. Conversation Mode should remain accessible after entering the translator.

---

# 4. Product Naming Decision

The current logo says:

```text
My Translation App
```

For Sprint 018, keep this name.

Reason:

- The logo already uses this name.
- The project is a portfolio app.
- Renaming now may add unnecessary brand work.
- Sprint 018 should focus on polish, not brand strategy.

---

# 5. Recommended App Flow

## Default Route / First Screen

```text
Landing Page
    ↓
Start Translating Button
    ↓
Translator Workspace
```

The landing page should be the first experience users see.

## Translator Workspace

Once the user enters the app, users can switch between:

```text
Translation Mode
Conversation Mode
```

Conversation Mode remains prominent because it is one of the strongest portfolio features.

---

# 6. Recommended Navigation Model

Use simple frontend state unless the current app already uses routing.

Recommended state model:

```typescript
screen: 'landing' | 'workspace'
workspaceMode: 'translate' | 'conversation'
translationInputMode: 'text' | 'audio'
```

If the project already uses a routing library, Builder may use routes instead, but should not add routing solely for this sprint unless clearly justified.

---

# 7. Landing Page Requirements

The landing page must include:

- Logo image
- Product name
- Short description
- Primary CTA button
- Secondary feature highlights
- Responsive layout
- Accessible image alt text

Recommended landing page copy:

```text
Translate text and speech across 37 languages with fast conversation tools, push-to-talk input, and spoken playback.
```

CTA:

```text
Start Translating
```

Optional secondary line:

```text
Built with React, .NET, Azure AI Speech, Azure Translator, and AWS.
```

Keep the landing page concise.

Do not turn it into a marketing site.

---

# 8. Logo Requirements

Use:

```text
design/inspiration/logo.png
```

Builder should copy or reference this asset according to the frontend build structure.

Preferred frontend public asset location:

```text
src/frontend/public/logo.png
```

If the project has an existing asset convention, follow it.

Logo should appear:

- Landing page hero
- App header / workspace header

Logo must include appropriate alt text:

```text
My Translation App logo
```

---

# 9. Favicon Requirements

Use:

```text
design/inspiration/favicon.png
```

Preferred target:

```text
src/frontend/public/favicon.png
```

Update:

```text
src/frontend/index.html
```

or equivalent Vite HTML file to include the favicon.

Expected:

- Browser tab favicon displays
- App title remains readable

---

# 10. Translation Mode Redesign

Current problem:

Text translation and audio translation are visible at the same time.

New behavior:

```text
Translation Mode
    ↓
[ Text ] [ Audio ]
    ↓
Only selected form displays
```

Approved recommendation:

Use a segmented toggle:

```text
[ Text Translation ] [ Audio Translation ]
```

Default mode:

```text
Text Translation
```

Reason:

- Most users understand text first.
- Text is the lowest-friction demo path.
- Audio remains one click away.

Audio Translation mode should preserve existing upload and audio workflow.

No backend changes.

---

# 11. Conversation Mode Access

Conversation Mode must remain accessible from the workspace.

Recommended layout:

```text
Workspace Header
    [ Translation ] [ Conversation ]
```

or:

```text
Main Mode Toggle
    Translation | Conversation
```

Do not hide Conversation Mode behind the landing page only.

Reason:

Conversation Mode is now the strongest feature and should remain demo-accessible.

---

# 12. Visual Design Direction

Sprint 018 should refine the current UI rather than rebuild everything.

Design direction:

- Clean dark theme or existing theme refinement
- Stronger spacing
- Clear mode hierarchy
- Better CTA styling
- Consistent cards
- Consistent button styles
- Improved mobile layout
- Reduced visual clutter
- Portfolio-quality first impression

Avoid:

- Large animations
- Heavy design libraries
- Full design-system rewrite
- Unnecessary dependencies

---

# 13. Mobile UX Requirements

Mobile must be treated as first-class.

Validate:

- Landing page
- Translator workspace
- Text / Audio toggle
- Conversation Mode
- Push-To-Talk button
- Conversation manager/search
- Export action row

Minimum expectations:

- No horizontal overflow
- Buttons are touch-friendly
- Forms are readable
- Main CTAs are obvious
- Landing page fits small screens

---

# 14. Accessibility Requirements

Builder must preserve or improve:

- Keyboard navigation
- Visible focus states
- Button labels
- Image alt text
- aria-live regions already used in app
- Sufficient contrast
- Semantic headings

Do not remove existing accessibility improvements.

---

# 15. Files Expected To Change

Likely modified:

```text
src/frontend/src/App.tsx
src/frontend/src/styles/app.css
src/frontend/index.html
```

Likely created:

```text
src/frontend/src/components/LandingPage.tsx
```

Likely asset changes:

```text
src/frontend/public/logo.png
src/frontend/public/favicon.png
```

Possibly modified:

```text
src/frontend/src/components/TextTranslationForm.tsx
src/frontend/src/components/AudioTranslationForm.tsx
src/frontend/src/components/ConversationMode.tsx
src/frontend/src/components/ConversationManager.tsx
```

Only modify feature components if layout integration requires it.

---

# 16. Files Not Expected To Change

Backend should not change.

Do not modify:

```text
src/backend/
tests/backend/
```

unless the dry run discovers an unexpected deployment or build issue.

No API changes are expected.

---

# 17. Data / API Impact

None.

No new DTOs.

No new endpoints.

No backend validation changes.

No provider changes.

---

# 18. Validation Plan

Builder must validate:

## Build

```bash
cd src/frontend
npx tsc --noEmit
npm run build
```

## Backend Regression

```bash
dotnet test --configuration Release
```

## Manual UX

Validate:

- Landing page loads first
- Logo displays
- Favicon displays
- Start Translating enters workspace
- Translation Mode is accessible
- Conversation Mode is accessible
- Text / Audio toggle works
- Only one translation form displays at a time
- Existing text translation still works
- Existing audio translation still works
- Existing Conversation Mode still works
- Push-To-Talk still works
- Mobile layout works

---

# 19. Acceptance Criteria

Sprint complete when:

- [ ] Landing page exists and is first screen
- [ ] Landing page displays logo
- [ ] Landing page has short app description
- [ ] Landing page has Start Translating CTA
- [ ] Logo appears in workspace header
- [ ] Favicon is configured
- [ ] Translation Mode has Text / Audio toggle
- [ ] Text and Audio forms are not displayed at the same time
- [ ] Text Translation works
- [ ] Audio Translation works
- [ ] Conversation Mode remains accessible
- [ ] Push-To-Talk still works
- [ ] Multi-conversation management still works
- [ ] Conversation search still works
- [ ] Mobile layout verified
- [ ] TypeScript build passes
- [ ] Production build passes
- [ ] Backend tests still pass
- [ ] No backend changes introduced unless justified
- [ ] Planning files updated

---

# 20. Risks

| Risk | Impact | Mitigation |
|---|---|---|
| UI redesign breaks existing workflows | High | Preserve component contracts and test manually |
| Landing page hides main features | Medium | Clear CTA and workspace navigation |
| Asset paths fail in Vite build | Medium | Use public asset convention and validate production build |
| Translation toggle creates state bugs | Medium | Keep simple local state in App or Translation section |
| Mobile layout regression | Medium | Validate small viewport manually |
| Redesign scope creep | High | Defer production refresh and portfolio assets to Sprint 019 |

---

# 21. Decisions To Add

D-110 Landing page is the default first screen.

D-111 Product name remains "My Translation App" for v1 portfolio release.

D-112 Translation Mode uses a Text / Audio segmented toggle and displays one form at a time.

D-113 Conversation Mode remains accessible from the translator workspace.

D-114 Sprint 018 is a UX redesign sprint only; no new backend features.

---

# 22. Questions To Track

Q-066 Should the product be renamed after v1 portfolio release?

Q-067 Should the landing page later include testimonials, pricing, or public marketing content?

Q-068 Should the app use route-based navigation in a future sprint?

Q-069 Should a custom domain be added in Sprint 019?

---

# 23. State Updates Required

At completion, update:

```text
planning/STATE.md
planning/DECISIONS.md
planning/RISKS.md
planning/QUESTIONS.md
planning/FILE_INVENTORY.md
planning/sprints/018-ui-ux-redesign/
```

Create sprint files:

```text
requirements.md
blueprint.md
acceptance.md
handoff-prompt.md
```

---

# 24. Builder Dry Run Instructions

Before implementation:

1. Read Sprint 017 outputs.
2. Inspect current frontend structure.
3. Confirm asset paths exist:
   - design/inspiration/logo.png
   - design/inspiration/favicon.png
4. Inspect current Vite public asset pattern.
5. Identify current mode-switching implementation.
6. Propose landing/workspace state model.
7. Propose Text / Audio toggle implementation.
8. Identify mobile validation plan.
9. Produce dry run report.
10. Wait for approval.

Do not implement immediately.

---

# 25. Builder Handoff Prompt

You are the Builder for My Translation App.

Sprint 018 — UI/UX Redesign.

Read Sprint 017 outputs first.

Perform a dry run before implementation.

The project owner wants:

- Landing page as the first screen
- Logo from design/inspiration/logo.png
- Favicon from design/inspiration/favicon.png
- Short app description
- Start Translating button
- Translation Mode should show Text or Audio form, not both at once
- Use a Text Translation / Audio Translation segmented toggle
- Conversation Mode must remain accessible from the translator workspace

Do not add new backend features.

Do not add authentication.

Do not add databases.

Do not introduce a major design library.

Preserve:

- Text Translation
- Audio Translation
- Conversation Mode
- Push-To-Talk
- Multi-conversation management
- Conversation search
- Persistence
- Export features

Provide a dry run report before implementation.

---

# 26. Completion Report Requirements

Provide:

- Files created
- Files modified
- Asset handling summary
- Build results
- Backend test results
- Manual UX validation results
- Mobile validation results
- Decisions added
- Risks added
- Questions added
- Recommended next sprint

Recommended next sprint:

Sprint 019 — Production Refresh & Portfolio Assets
