import {
  CONVERSATION_STORAGE_VERSION,
  CONVERSATION_STORE_VERSION,
  type ConversationMessage,
  type ConversationSession,
  type ConversationStore,
  type ConversationSummary,
} from '../types/conversation';

const STORE_KEY = 'my-translation-app-conversations';
const LEGACY_KEY = 'my-translation-app-conversation';

function generateConvId(): string {
  return `conv-${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
}

function createEmptyStore(): ConversationStore {
  return {
    version: CONVERSATION_STORE_VERSION,
    activeConversationId: null,
    conversations: {},
  };
}

// ── Validators ────────────────────────────────────────────────────────────────

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
    typeof s.id === 'string' &&
    typeof s.title === 'string' &&
    s.version === CONVERSATION_STORAGE_VERSION &&
    typeof s.createdAt === 'string' &&
    typeof s.updatedAt === 'string' &&
    typeof s.languageA === 'string' &&
    typeof s.languageB === 'string' &&
    Array.isArray(s.messages) &&
    (s.messages as unknown[]).every(isValidMessage)
  );
}

function isValidStore(raw: unknown): raw is ConversationStore {
  if (!raw || typeof raw !== 'object') return false;
  const s = raw as Record<string, unknown>;
  return (
    s.version === CONVERSATION_STORE_VERSION &&
    (s.activeConversationId === null || typeof s.activeConversationId === 'string') &&
    typeof s.conversations === 'object' &&
    s.conversations !== null &&
    Object.values(s.conversations as Record<string, unknown>).every(isValidSession)
  );
}

// Sprint 015 schema — no id or title fields
function isLegacySession(raw: unknown): boolean {
  if (!raw || typeof raw !== 'object') return false;
  const s = raw as Record<string, unknown>;
  return (
    s.version === CONVERSATION_STORAGE_VERSION &&
    typeof s.createdAt === 'string' &&
    typeof s.updatedAt === 'string' &&
    typeof s.languageA === 'string' &&
    typeof s.languageB === 'string' &&
    Array.isArray(s.messages)
  );
}

// ── Core storage ──────────────────────────────────────────────────────────────

export function loadStore(): ConversationStore | null {
  try {
    const raw = localStorage.getItem(STORE_KEY);
    if (!raw) return null;
    const parsed: unknown = JSON.parse(raw);
    return isValidStore(parsed) ? parsed : null;
  } catch {
    return null;
  }
}

export function saveStore(store: ConversationStore): void {
  try {
    localStorage.setItem(STORE_KEY, JSON.stringify(store));
  } catch {
    // localStorage unavailable (private mode, quota exceeded) — fail silently
  }
}

// ── Migration and entry point ─────────────────────────────────────────────────

export function loadOrMigrateStore(): ConversationStore {
  const store = loadStore();
  if (store) return store;

  // Attempt one-time migration from Sprint 015 single-conversation storage
  try {
    const legacyRaw = localStorage.getItem(LEGACY_KEY);
    if (legacyRaw) {
      const legacy: unknown = JSON.parse(legacyRaw);
      if (isLegacySession(legacy)) {
        const s = legacy as Record<string, unknown>;
        const id = generateConvId();
        const session: ConversationSession = {
          id,
          title: 'Conversation 1',
          version: CONVERSATION_STORAGE_VERSION,
          createdAt: typeof s.createdAt === 'string' ? s.createdAt : new Date().toISOString(),
          updatedAt: typeof s.updatedAt === 'string' ? s.updatedAt : new Date().toISOString(),
          languageA: typeof s.languageA === 'string' ? s.languageA : '',
          languageB: typeof s.languageB === 'string' ? s.languageB : '',
          messages: Array.isArray(s.messages)
            ? (s.messages as unknown[]).filter(isValidMessage) as ConversationMessage[]
            : [],
        };
        const newStore: ConversationStore = {
          version: CONVERSATION_STORE_VERSION,
          activeConversationId: id,
          conversations: { [id]: session },
        };
        saveStore(newStore);
        // Remove legacy key only after successful save
        localStorage.removeItem(LEGACY_KEY);
        return newStore;
      }
    }
  } catch {
    // Migration failed — start fresh
  }

  return createEmptyStore();
}

// ── Derived helpers ───────────────────────────────────────────────────────────

export function getConversationSummaries(store: ConversationStore): ConversationSummary[] {
  return Object.values(store.conversations)
    .map((s) => {
      const lastMsg = s.messages[s.messages.length - 1];
      const previewText = lastMsg
        ? (lastMsg.translatedText || lastMsg.originalText) || undefined
        : undefined;
      return {
        id: s.id,
        title: s.title,
        createdAt: s.createdAt,
        updatedAt: s.updatedAt,
        messageCount: s.messages.length,
        languageA: s.languageA,
        languageB: s.languageB,
        previewText,
      };
    })
    .sort((a, b) => b.updatedAt.localeCompare(a.updatedAt));
}

export function getActiveSession(store: ConversationStore): ConversationSession | null {
  if (!store.activeConversationId) return null;
  return store.conversations[store.activeConversationId] ?? null;
}

// ── CRUD — pure functions returning updated store ─────────────────────────────

export function createConversation(
  store: ConversationStore,
  title: string,
): { store: ConversationStore; id: string } {
  const id = generateConvId();
  const now = new Date().toISOString();
  const session: ConversationSession = {
    id,
    title,
    isAutoTitle: true,
    version: CONVERSATION_STORAGE_VERSION,
    createdAt: now,
    updatedAt: now,
    languageA: '',
    languageB: '',
    messages: [],
  };
  return {
    id,
    store: {
      ...store,
      activeConversationId: id,
      conversations: { ...store.conversations, [id]: session },
    },
  };
}

export function updateConversation(
  store: ConversationStore,
  session: ConversationSession,
): ConversationStore {
  return {
    ...store,
    conversations: { ...store.conversations, [session.id]: session },
  };
}

export function renameConversation(
  store: ConversationStore,
  id: string,
  title: string,
): ConversationStore {
  const existing = store.conversations[id];
  if (!existing) return store;
  return {
    ...store,
    conversations: { ...store.conversations, [id]: { ...existing, title, isAutoTitle: false } },
  };
}

export function deleteConversation(
  store: ConversationStore,
  id: string,
): ConversationStore {
  const { [id]: _deleted, ...remaining } = store.conversations;
  const newActiveId =
    store.activeConversationId === id
      ? getMostRecentId(remaining)
      : store.activeConversationId;
  return {
    ...store,
    activeConversationId: newActiveId,
    conversations: remaining,
  };
}

export function switchConversation(
  store: ConversationStore,
  id: string,
): ConversationStore {
  if (!store.conversations[id]) return store;
  return { ...store, activeConversationId: id };
}

function getMostRecentId(conversations: Record<string, ConversationSession>): string | null {
  const sessions = Object.values(conversations);
  if (sessions.length === 0) return null;
  return sessions.reduce((a, b) => (a.updatedAt > b.updatedAt ? a : b)).id;
}
