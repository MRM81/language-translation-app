# My Translation App — Frontend

React + TypeScript + Vite frontend. Sprint 004.

---

## Prerequisites

- Node.js 18 or later (`node --version`)
- npm 9 or later (`npm --version`)
- The backend must be running locally (see `src/backend/README.md`)

---

## Running Locally

### 1. Start the backend

In a separate terminal:

```bash
cd src/backend/MyTranslationApp.Api
dotnet run
```

The backend starts at `http://localhost:5074`.

### 2. Install frontend dependencies

```bash
cd src/frontend
npm install
```

### 3. Start the frontend dev server

```bash
npm run dev
```

Open `http://localhost:5173` in your browser.

The Vite dev server proxies all `/api/*` requests to `http://localhost:5074`. No manual CORS configuration is needed for local development.

---

## Configuration

The backend URL is configured via the Vite proxy in `vite.config.ts`. If your backend runs on a different port, update the `proxy.target` value there.

Copy `.env.example` to `.env.local` if you need to override environment variables:

```bash
cp .env.example .env.local
```

Do not commit `.env.local`.

---

## Commands

| Command | Description |
|---|---|
| `npm install` | Install dependencies |
| `npm run dev` | Start dev server at `http://localhost:5173` |
| `npm run build` | Production build (output: `dist/`) |
| `npm run typecheck` | Type-check without emitting output |
| `npm run preview` | Preview production build locally |

---

## Manual Validation Checklist

Run these with both the backend and frontend running.

| # | Check | Expected |
|---|---|---|
| 1 | Open `http://localhost:5173` | App loads, language selectors populate |
| 2 | Text form — enter text, select target language, click Translate | Mock result with correlation ID appears |
| 3 | Text form — submit empty text | Inline validation error, no request sent |
| 4 | Text form — submit without selecting target language | Inline validation error, no request sent |
| 5 | Text form — set source = target language | Inline validation error, no request sent |
| 6 | Text form — paste 5001 characters | Character count turns red, Translate button disabled |
| 7 | Audio form — select a small audio file, select target language, click Translate | Mock transcript and translation appear |
| 8 | Audio form — click Translate without selecting a file | Inline validation error |
| 9 | Audio form — select a file over 10 MB | Inline validation error |
| 10 | Kill the backend, try to translate | Network error state shown |
| 11 | DevTools Network/Console tab | No source text, transcripts, filenames, or audio data logged |

---

## Project Structure

```
src/frontend/
├── src/
│   ├── api/
│   │   └── translationApi.ts     # All backend HTTP calls
│   ├── components/
│   │   ├── LanguageSelect.tsx
│   │   ├── TextTranslationForm.tsx
│   │   ├── AudioTranslationForm.tsx
│   │   ├── ResultPanel.tsx
│   │   └── ErrorPanel.tsx
│   ├── types/
│   │   └── api.ts                # TypeScript types matching backend DTOs
│   ├── styles/
│   │   └── app.css
│   ├── App.tsx
│   └── main.tsx
├── index.html
├── vite.config.ts
├── tsconfig.json
├── package.json
└── .env.example
```

---

## Notes

- Audio upload uses a standard file input (`<input type="file">`). Browser microphone recording (push-to-talk) is planned for Sprint 006.
- All backend communication goes through `src/api/translationApi.ts`. Components do not call the backend directly.
- Error responses from the backend use `errorCode` (not `code`), and field-level details contain `field` and `message`.
- No user content (source text, transcripts, translations, audio filenames) is logged to the console.
