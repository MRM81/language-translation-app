# Style Guide

**Project:** My Translation App
**Sprint:** 008 — UX Modernization

---

## Color Palette

| Token | Value | Usage |
|---|---|---|
| `--bg` | `#0d1117` | Page background |
| `--surface` | `#161b2e` | Card surfaces, form panels |
| `--surface-raised` | `#1e2540` | Elevated surfaces, active tab, swap button hover |
| `--border` | `rgba(255,255,255,0.08)` | Card borders, dividers |
| `--border-strong` | `rgba(255,255,255,0.15)` | Interactive element borders |
| `--accent` | `#6366f1` | Primary CTA buttons, active tabs, record button |
| `--accent-hover` | `#4f46e5` | Hovered primary buttons |
| `--accent-active` | `#4338ca` | Pressed/active primary buttons |
| `--success` | `#10b981` | Positive result panel border, playing state |
| `--error` | `#ef4444` | Error states, stop recording button |
| `--warning` | `#f59e0b` | Warning messages |
| `--text-primary` | `#f0f0f5` | Body text, headings, values |
| `--text-secondary` | `#94a3b8` | Labels, secondary text, form section headings |
| `--text-muted` | `#64748b` | Hints, meta text, correlation ID |

---

## Typography

| Element | Size | Weight | Color |
|---|---|---|---|
| App title (`h1`) | `1.75rem` | 700 | `#ffffff` |
| App tagline | `0.9375rem` | 400 | `var(--text-secondary)` |
| Section label (`h2`) | `0.8125rem` | 600 | `var(--text-secondary)` — uppercase, tracked |
| Form label | `0.875rem` | 500 | `var(--text-secondary)` |
| Body / input | `0.9375rem` | 400 | `var(--text-primary)` |
| Translation hero | `1.375rem` | 700 | `var(--text-primary)` |
| Transcript text | `0.9375rem` | 400 | `var(--text-secondary)` |
| Meta / hint | `0.8rem` | 400 | `var(--text-muted)` |
| Character count | `0.8rem` | 400 | `var(--text-muted)` |

Font stack: `system-ui, -apple-system, sans-serif`

---

## Spacing Scale

| Name | Value | Usage |
|---|---|---|
| xs | `0.25rem` (4px) | Tight gaps |
| sm | `0.5rem` (8px) | Input padding, small gaps |
| md | `0.75rem` (12px) | Field gaps, button internal |
| base | `1rem` (16px) | Standard gap, body padding |
| lg | `1.5rem` (24px) | Card padding, section gap |
| xl | `2rem` (32px) | Header padding, major sections |

---

## Border Radius Scale

| Token | Value | Usage |
|---|---|---|
| `--radius-sm` | `6px` | Tags, small chips |
| `--radius-md` | `10px` | Buttons, inputs, small cards |
| `--radius-lg` | `16px` | Cards, panels |
| `--radius-full` | `9999px` | Pills, circular buttons |

---

## Components

### Buttons

| Variant | Background | Hover | Use |
|---|---|---|---|
| Primary (`.btn-primary`) | `var(--accent)` | `var(--accent-hover)` | Translate, submit actions |
| Record (`.btn-record`) | `var(--accent)` | `var(--accent-hover)` | 72×72px circular, mic icon |
| Stop (`.btn-stop-recording`) | `var(--error)` | `#dc2626` | 72×72px circular, stop icon, pulse animation |
| Swap (`.btn-swap`) | `var(--surface-raised)` | `var(--accent)` | 36×36px circular, ⇄ |
| Play (`.play-button`) | `var(--surface-raised)` | — | ▶ icon + label |
| Tab (`.audio-tab.active`) | `var(--accent)` | — | Active tab indicator |

### Cards

`.translation-form`: `background: var(--surface)`, `border: 1px solid var(--border)`, `border-radius: var(--radius-lg)`, `box-shadow: 0 1px 3px rgba(0,0,0,0.3)`

### Result Panel

`.panel-result`: dark surface, `border-left: 3px solid var(--success)`

Translation text (`.result-hero`): `font-size: 1.375rem`, `font-weight: 700`, `color: var(--text-primary)`

---

## Responsive Breakpoints

| Breakpoint | Width | Layout |
|---|---|---|
| Mobile S | `< 480px` | Single column, circular record centered |
| Mobile L | `480–767px` | Single column, comfortable padding |
| Tablet | `768–1023px` | Two-column form grid |
| Desktop | `1024–1439px` | Centered max-width container |
| Desktop L | `1440px+` | Max 960px centered |
