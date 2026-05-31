import {
  CONVERSATION_STORAGE_VERSION,
  type ConversationMessage,
  type ConversationSession,
} from '../types/conversation';

const STORAGE_KEY = 'my-translation-app-conversation';

function isValidMessage(raw: unknown): raw is ConversationMessage {
  if (!raw || typeof raw !== 'object') return false;
  const m = raw as Record<string, unknown>;
  return (
    typeof m.id === 'string' &&
    (m.speaker === 'A' || m.speaker === 'B') &&
    typeof m.originalText === 'string' &&
    typeof m.translatedText === 'string' &&
    typeof m.sourceLanguage === 'string' &&
    typeof m.targetLanguage === 'string' &&
    typeof m.timestamp === 'string' &&
    (m.inputType === 'text' || m.inputType === 'audio')
  );
}

function isValidSession(raw: unknown): raw is ConversationSession {
  if (!raw || typeof raw !== 'object') return false;
  const s = raw as Record<string, unknown>;
  return (
    s.version === CONVERSATION_STORAGE_VERSION &&
    typeof s.createdAt === 'string' &&
    typeof s.updatedAt === 'string' &&
    typeof s.languageA === 'string' &&
    typeof s.languageB === 'string' &&
    Array.isArray(s.messages) &&
    (s.messages as unknown[]).every(isValidMessage)
  );
}

export function loadSession(): ConversationSession | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const parsed: unknown = JSON.parse(raw);
    return isValidSession(parsed) ? parsed : null;
  } catch {
    return null;
  }
}

export function saveSession(session: ConversationSession): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
  } catch {
    // localStorage unavailable (private mode, quota exceeded) — fail silently
  }
}

export function clearSession(): void {
  try {
    localStorage.removeItem(STORAGE_KEY);
  } catch {
    // fail silently
  }
}
