import type {
  ApiErrorResponse,
  AudioTranslationResponse,
  LanguageListResponse,
  TextTranslationRequest,
  TextTranslationResponse,
} from '../types/api';

// In development, VITE_API_BASE_URL is unset and the Vite proxy forwards /api/* to the backend.
// In production (S3+CloudFront + Elastic Beanstalk), set VITE_API_BASE_URL to the EB backend URL
// at build time so that API calls reach the correct domain. See src/frontend/.env.production.example.
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? '';

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
