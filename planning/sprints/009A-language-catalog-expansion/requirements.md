# Sprint 009A Requirements — Language Catalog Expansion

## Goal

Expand available languages, especially European languages and Mandarin Chinese, while preserving all completed MVP workflows.

## In Scope

### Language Catalog

Add or verify support for:

#### Existing Core Languages (preserved)

- English (`en`)
- Spanish (`es`)
- French (`fr`)
- German (`de`)
- Italian (`it`)
- Portuguese (`pt`)
- Arabic (`ar`)
- Japanese (`ja`)
- Chinese Simplified (`zh`)
- Russian (`ru`)

#### European Expansion

- Dutch (`nl`)
- Polish (`pl`)
- Czech (`cs`)
- Slovak (`sk`)
- Romanian (`ro`)
- Hungarian (`hu`)
- Greek (`el`)
- Swedish (`sv`)
- Danish (`da`)
- Norwegian (`nb`) — Bokmål
- Finnish (`fi`)
- Ukrainian (`uk`)
- Turkish (`tr`)
- Croatian (`hr`)
- Serbian (`sr-Cyrl`) — Cyrillic script
- Slovenian (`sl`)
- Bulgarian (`bg`)
- Lithuanian (`lt`)
- Latvian (`lv`)
- Estonian (`et`)

#### Global Expansion

- Korean (`ko`)
- Vietnamese (`vi`)
- Thai (`th`)
- Indonesian (`id`)
- Malay (`ms`)
- Hindi (`hi`)

#### Bonus

- Chinese Traditional (`zh-Hant`) — included per D-068 and Q-035 recommendation

### Total: 37 languages

### Validation

- Backend language catalog expanded.
- Frontend language selectors updated automatically (catalog-driven).
- STT locale map updated for all 37 languages.
- TTS voice map updated for all 37 languages.
- Original language behavior preserved.
- Tests added for new catalog entries.

## Out Of Scope

- Auto language detection improvements.
- Region-specific dialect selection UI.
- Voice selection UI.
- Capability badges.
- Translation history.
- Deployment.
- Authentication.
- Database persistence.

## Constraints

- Do not hardcode unsupported Azure language codes without verification.
- Do not break existing provider behavior.
- Keep selectors mobile-friendly (alphabetical list, catalog-driven).
