# Design Decisions

**Project:** My Translation App
**Sprint:** 008 — UX Modernization

---

| ID | Decision | Rationale |
|---|---|---|
| DD-001 | Dark navy full-page theme adopted | Consistent with translation app design patterns (inspiration 1, 7, 8). Gives the app a modern, professional feel appropriate for a portfolio project. |
| DD-002 | Indigo (`#6366f1`) as primary CTA accent color | Distinct from the dark structural chrome. Creates clear visual hierarchy — dark surfaces are structure, indigo is action. |
| DD-003 | CSS custom properties at `:root` | Enables future dark/light mode toggle without a full stylesheet rewrite. Documents the design system in code. |
| DD-004 | Language pair row with ⇄ swap button | Standard UX pattern in translation apps (inspiration 4, 5). Reduces tap count for bidirectional translation. Implemented as trivial in-form state swap with no lift required. |
| DD-005 | Circular record button (72×72px) | Voice interaction affordance. Large target reduces mis-taps on mobile. Circular shape signals a different interaction type than rectangular translate buttons. |
| DD-006 | Translation result displayed as hero text | The translated text is the primary output. Displaying it large and bold makes the result immediately readable without scanning a label-value table. |
| DD-007 | Provider and correlation ID moved to collapsed meta footer | These are developer-visible fields. For non-technical portfolio reviewers they look like debug output. Footer placement keeps them accessible without cluttering the result. |
| DD-008 | `system-ui, -apple-system, sans-serif` retained as font stack | The dark theme update provides more visual impact than a web font swap. No additional network request or FOUT risk. |
| DD-009 | App name "My Translation App" retained | Q-033 (branding decision) is open. Builder does not hardcode a product name without owner direction. |
| DD-010 | No external CSS framework or icon library | Keeps the dependency surface minimal and consistent with the existing project convention (D-035). Inline SVG for the mic icon. Unicode symbols for other icons. |
