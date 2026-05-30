import type {
  ApiErrorResponse,
  AudioTranslationResponse,
  LanguageListResponse,
  TextTranslationRequest,
  TextTranslationResponse,
} from '../types/api';

// All API calls route through the Vite dev server proxy (/api -> http://localhost:5074).
// In production builds, the backend base URL must be configured appropriately.
const API_BASE = '';

async function parseResponse<T>(res: Response): Promise<T> {
  const body = await res.json();
  if (!res.ok) {
    throw body as ApiErrorResponse;
  }
  return body as T;
}

export async function fetchLanguages(): Promise<LanguageListResponse> {
  const res = await fetch(`${API_BASE}/api/languages`);
  return parseResponse<LanguageListResponse>(res);
}

export async function translateText(
  req: TextTranslationRequest,
): Promise<TextTranslationResponse> {
  const res = await fetch(`${API_BASE}/api/translate/text`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(req),
  });
  return parseResponse<TextTranslationResponse>(res);
}

export async function translateAudio(
  file: File,
  targetLanguage: string,
  sourceLanguage: string,
): Promise<AudioTranslationResponse> {
  const form = new FormData();
  form.append('audio', file);
  form.append('targetLanguage', targetLanguage);
  if (sourceLanguage) {
    form.append('sourceLanguage', sourceLanguage);
  }

  const res = await fetch(`${API_BASE}/api/translate/audio`, {
    method: 'POST',
    body: form,
  });
  return parseResponse<AudioTranslationResponse>(res);
}

export async function synthesizeSpeech(
  text: string,
  language: string,
): Promise<Blob> {
  const res = await fetch(`${API_BASE}/api/translate/tts`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ text, language }),
  });

  if (!res.ok) {
    // Only parse JSON when the response body actually is JSON.
    // Non-JSON bodies (proxy errors, empty 502s) must not reach res.json() —
    // that call throws "Failed to execute 'json' on 'Response'" and exposes
    // a raw browser engine error in the UI.
    const contentType = res.headers.get('Content-Type') ?? '';
    if (contentType.includes('application/json')) {
      const body = await res.json();
      throw body as ApiErrorResponse;
    }
    throw {
      errorCode: 'PROVIDER_ERROR',
      message: 'Speech synthesis failed. Please try again.',
    } as ApiErrorResponse;
  }

  return res.blob();
}
