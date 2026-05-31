import { useEffect, useMemo, useRef, useState } from 'react';
import { synthesizeSpeech, translateAudio, translateText } from '../api/translationApi';
import { copyToClipboard, exportAsJson, exportAsTxt } from '../services/ConversationExportService';
import {
  createConversation,
  deleteConversation,
  getActiveSession,
  getConversationSummaries,
  loadOrMigrateStore,
  loadStore,
  renameConversation,
  saveStore,
  switchConversation,
  updateConversation,
} from '../services/ConversationStorageService';
import type { LanguageOption } from '../types/api';
import {
  CONVERSATION_STORAGE_VERSION,
  type ConversationMessage,
  type ConversationSession,
  type ConversationStore,
  type ConversationSummary,
} from '../types/conversation';
import { ConversationHistory } from './ConversationHistory';
import { ConversationInput } from './ConversationInput';
import { ConversationManager } from './ConversationManager';
import { LanguageSelect } from './LanguageSelect';

interface Props {
  languages: LanguageOption[];
}

type CopyStatus = 'idle' | 'copied' | 'error';

function generateId(): string {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
}

function defaultTitle(store: ConversationStore): string {
  return `Conversation ${Object.keys(store.conversations).length + 1}`;
}

function buildAutoTitle(text: string): string {
  const trimmed = text.trim();
  return trimmed.length > 40 ? trimmed.slice(0, 40).trimEnd() + '…' : trimmed;
}

export function ConversationMode({ languages }: Props) {
  const [langA, setLangA] = useState('');
  const [langB, setLangB] = useState('');
  const [messages, setMessages] = useState<ConversationMessage[]>([]);
  const [activeSpeaker, setActiveSpeaker] = useState<'A' | 'B'>('A');
  const [autoPlay, setAutoPlay] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [playError, setPlayError] = useState<string | null>(null);
  const [copyStatus, setCopyStatus] = useState<CopyStatus>('idle');
  const [activeConversationId, setActiveConversationId] = useState<string | null>(null);
  const [activeTitle, setActiveTitle] = useState('');
  const [isAutoTitle, setIsAutoTitle] = useState(false);
  const [summaries, setSummaries] = useState<ConversationSummary[]>([]);
  const [searchQuery, setSearchQuery] = useState('');

  const copyTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const langAName = languages.find((l) => l.code === langA)?.name ?? langA;
  const langBName = languages.find((l) => l.code === langB)?.name ?? langB;

  // ── Load / migrate on mount ────────────────────────────────────────────────

  useEffect(() => {
    const store = loadOrMigrateStore();
    const active = getActiveSession(store);

    if (active) {
      setActiveConversationId(active.id);
      setActiveTitle(active.title);
      setIsAutoTitle(active.isAutoTitle ?? false);
      setLangA(active.languageA);
      setLangB(active.languageB);
      setMessages(active.messages);
      setSummaries(getConversationSummaries(store));
    } else {
      // No conversations — create the first one automatically
      const { store: newStore, id } = createConversation(store, 'Conversation 1');
      saveStore(newStore);
      setActiveConversationId(id);
      setActiveTitle('Conversation 1');
      setIsAutoTitle(true);
      setSummaries(getConversationSummaries(newStore));
    }
  }, []);

  // ── Persist active session whenever conversation state changes ─────────────

  useEffect(() => {
    if (!activeConversationId) return;
    const currentStore = loadStore();
    if (!currentStore) return;

    const now = new Date().toISOString();
    const session: ConversationSession = {
      id: activeConversationId,
      title: activeTitle,
      isAutoTitle,
      version: CONVERSATION_STORAGE_VERSION,
      createdAt: messages[0]?.timestamp ?? currentStore.conversations[activeConversationId]?.createdAt ?? now,
      updatedAt: messages[messages.length - 1]?.timestamp ?? now,
      languageA: langA,
      languageB: langB,
      messages,
    };
    const updated = updateConversation(currentStore, session);
    saveStore(updated);
    setSummaries(getConversationSummaries(updated));
  }, [messages, langA, langB, activeConversationId, activeTitle, isAutoTitle]);

  // ── Clean up copy timer on unmount ─────────────────────────────────────────

  useEffect(() => {
    return () => {
      if (copyTimerRef.current) clearTimeout(copyTimerRef.current);
    };
  }, []);

  // ── Filtered summaries for search ─────────────────────────────────────────

  const filteredSummaries = useMemo(() => {
    const q = searchQuery.trim().toLowerCase();
    if (!q) return summaries;

    const store = loadStore();
    return summaries.filter((summary) => {
      // Always include the active conversation
      if (summary.id === activeConversationId) return true;
      if (summary.title.toLowerCase().includes(q)) return true;
      if (!store) return false;
      const session = store.conversations[summary.id];
      if (!session) return false;
      return session.messages.some(
        (m) =>
          m.originalText.toLowerCase().includes(q) ||
          m.translatedText.toLowerCase().includes(q),
      );
    });
  }, [searchQuery, summaries, activeConversationId]);

  // ── Conversation management handlers ──────────────────────────────────────

  function handleNew() {
    const currentStore = loadStore();
    if (!currentStore) return;
    const title = defaultTitle(currentStore);
    const { store: newStore, id } = createConversation(currentStore, title);
    saveStore(newStore);
    setActiveConversationId(id);
    setActiveTitle(title);
    setIsAutoTitle(true);
    setLangA('');
    setLangB('');
    setMessages([]);
    setActiveSpeaker('A');
    setError(null);
    setPlayError(null);
    setSearchQuery('');
    setSummaries(getConversationSummaries(newStore));
  }

  function handleSwitch(id: string) {
    const currentStore = loadStore();
    if (!currentStore) return;
    const session = currentStore.conversations[id];
    if (!session) return;
    const updatedStore = switchConversation(currentStore, id);
    saveStore(updatedStore);
    setActiveConversationId(session.id);
    setActiveTitle(session.title);
    setIsAutoTitle(session.isAutoTitle ?? false);
    setLangA(session.languageA);
    setLangB(session.languageB);
    setMessages(session.messages);
    setActiveSpeaker('A');
    setError(null);
    setPlayError(null);
    setSummaries(getConversationSummaries(updatedStore));
  }

  function handleRename(id: string, title: string) {
    const currentStore = loadStore();
    if (!currentStore) return;
    const updatedStore = renameConversation(currentStore, id, title);
    saveStore(updatedStore);
    if (id === activeConversationId) {
      setActiveTitle(title);
      setIsAutoTitle(false);
    }
    setSummaries(getConversationSummaries(updatedStore));
  }

  function handleDelete(id: string) {
    const currentStore = loadStore();
    if (!currentStore) return;
    const updatedStore = deleteConversation(currentStore, id);

    if (updatedStore.activeConversationId === null) {
      // Last conversation deleted — create a blank one
      const { store: freshStore, id: newId } = createConversation(updatedStore, 'Conversation 1');
      saveStore(freshStore);
      setActiveConversationId(newId);
      setActiveTitle('Conversation 1');
      setIsAutoTitle(true);
      setLangA('');
      setLangB('');
      setMessages([]);
      setActiveSpeaker('A');
      setSummaries(getConversationSummaries(freshStore));
    } else {
      saveStore(updatedStore);
      const newActive = getActiveSession(updatedStore);
      if (newActive) {
        setActiveConversationId(newActive.id);
        setActiveTitle(newActive.title);
        setIsAutoTitle(newActive.isAutoTitle ?? false);
        setLangA(newActive.languageA);
        setLangB(newActive.languageB);
        setMessages(newActive.messages);
        setActiveSpeaker('A');
      }
      setSummaries(getConversationSummaries(updatedStore));
    }
    setError(null);
    setPlayError(null);
  }

  // ── Auto-title helper ──────────────────────────────────────────────────────

  function applyAutoTitleIfNeeded(firstMessageText: string) {
    if (!isAutoTitle || messages.length !== 0) return;
    const autoTitle = buildAutoTitle(firstMessageText);
    setActiveTitle(autoTitle);
    setIsAutoTitle(false);
    // Persist the new title immediately (save effect will also fire, but set state first)
    const currentStore = loadStore();
    if (!currentStore || !activeConversationId) return;
    const existing = currentStore.conversations[activeConversationId];
    if (!existing) return;
    const updated = updateConversation(currentStore, { ...existing, title: autoTitle, isAutoTitle: false });
    saveStore(updated);
    setSummaries(getConversationSummaries(updated));
  }

  // ── Conversation workflow ──────────────────────────────────────────────────

  function handleSwap() {
    // History is immutable — messages retain their original language pairs.
    // Swap only affects future turns.
    setLangA(langB);
    setLangB(langA);
  }

  function handleClear() {
    // Clear = remove messages from active conversation. Conversation itself is preserved.
    setMessages([]);
    setActiveSpeaker('A');
    setError(null);
    setPlayError(null);
    // The save effect persists the empty message list to the store.
  }

  async function tryAutoPlay(translatedText: string, targetLanguage: string) {
    setPlayError(null);
    try {
      const blob = await synthesizeSpeech(translatedText, targetLanguage);
      const url = URL.createObjectURL(blob);
      const audio = new Audio(url);
      audio.onended = () => URL.revokeObjectURL(url);
      audio.onerror = () => {
        URL.revokeObjectURL(url);
        setPlayError('Audio playback failed. You can continue the conversation.');
      };
      await audio.play();
    } catch {
      setPlayError('Speech synthesis failed. You can continue the conversation.');
    }
  }

  async function handleTextSubmit(speaker: 'A' | 'B', text: string) {
    const sourceLang = speaker === 'A' ? langA : langB;
    const targetLang = speaker === 'A' ? langB : langA;

    setLoading(true);
    setError(null);
    setPlayError(null);

    try {
      const data = await translateText({
        sourceText: text,
        sourceLanguage: sourceLang,
        targetLanguage: targetLang,
      });

      applyAutoTitleIfNeeded(text);

      const msg: ConversationMessage = {
        id: generateId(),
        speaker,
        originalText: text,
        translatedText: data.translatedText,
        sourceLanguage: sourceLang,
        targetLanguage: targetLang,
        timestamp: new Date().toISOString(),
        inputType: 'text',
      };
      setMessages((prev) => [...prev, msg]);
      setActiveSpeaker(speaker === 'A' ? 'B' : 'A');

      if (autoPlay) await tryAutoPlay(data.translatedText, targetLang);
    } catch (err: unknown) {
      const apiErr = err as { message?: string };
      setError(apiErr?.message ?? 'Translation failed. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  async function handleAudioSubmit(speaker: 'A' | 'B', file: File) {
    const sourceLang = speaker === 'A' ? langA : langB;
    const targetLang = speaker === 'A' ? langB : langA;

    setLoading(true);
    setError(null);
    setPlayError(null);

    try {
      const data = await translateAudio(file, targetLang, sourceLang);

      applyAutoTitleIfNeeded(data.transcribedText);

      const msg: ConversationMessage = {
        id: generateId(),
        speaker,
        originalText: data.transcribedText,
        translatedText: data.translatedText,
        sourceLanguage: sourceLang,
        targetLanguage: targetLang,
        timestamp: new Date().toISOString(),
        inputType: 'audio',
      };
      setMessages((prev) => [...prev, msg]);
      setActiveSpeaker(speaker === 'A' ? 'B' : 'A');

      if (autoPlay) await tryAutoPlay(data.translatedText, targetLang);
    } catch (err: unknown) {
      const apiErr = err as { message?: string };
      setError(apiErr?.message ?? 'Translation failed. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  // ── Export handlers ────────────────────────────────────────────────────────

  function handleExportTxt() {
    exportAsTxt(messages, langAName, langBName, activeTitle);
  }

  function handleExportJson() {
    const now = new Date().toISOString();
    const session: ConversationSession = {
      id: activeConversationId ?? '',
      title: activeTitle,
      isAutoTitle,
      version: CONVERSATION_STORAGE_VERSION,
      createdAt: messages[0]?.timestamp ?? now,
      updatedAt: messages[messages.length - 1]?.timestamp ?? now,
      languageA: langA,
      languageB: langB,
      messages,
    };
    exportAsJson(session);
  }

  async function handleCopy() {
    if (copyTimerRef.current) clearTimeout(copyTimerRef.current);
    try {
      await copyToClipboard(messages, langAName, langBName, activeTitle);
      setCopyStatus('copied');
      copyTimerRef.current = setTimeout(() => setCopyStatus('idle'), 2000);
    } catch {
      setCopyStatus('error');
      copyTimerRef.current = setTimeout(() => setCopyStatus('idle'), 3000);
    }
  }

  const copyLabel =
    copyStatus === 'copied' ? 'Copied!' :
    copyStatus === 'error'  ? 'Failed'  :
    'Copy';

  // ── Render ─────────────────────────────────────────────────────────────────

  return (
    <div className="conversation-mode">
      <div className="conv-lang-header">
        <ConversationManager
          summaries={filteredSummaries}
          allCount={summaries.length}
          activeId={activeConversationId}
          activeTitle={activeTitle}
          loading={loading}
          searchQuery={searchQuery}
          onSearchChange={setSearchQuery}
          onNew={handleNew}
          onSwitch={handleSwitch}
          onRename={handleRename}
          onDelete={handleDelete}
        />

        <div className="lang-pair-row">
          <LanguageSelect
            id="conv-lang-a"
            label="Speaker A language"
            value={langA}
            onChange={setLangA}
            languages={languages}
            disabled={loading}
            required
          />
          <button
            type="button"
            className="btn-swap"
            onClick={handleSwap}
            disabled={loading}
            aria-label="Swap speaker languages"
            title="Swap languages (does not change existing history)"
          >
            ⇄
          </button>
          <LanguageSelect
            id="conv-lang-b"
            label="Speaker B language"
            value={langB}
            onChange={setLangB}
            languages={languages}
            disabled={loading}
            required
          />
        </div>

        {messages.length > 0 && (
          <div className="conv-action-row">
            <button
              type="button"
              className="btn-conv-action"
              onClick={handleExportTxt}
              disabled={loading}
            >
              Export TXT
            </button>
            <button
              type="button"
              className="btn-conv-action"
              onClick={handleExportJson}
              disabled={loading}
            >
              Export JSON
            </button>
            <button
              type="button"
              className={`btn-conv-action${copyStatus !== 'idle' ? ` btn-conv-action--${copyStatus}` : ''}`}
              onClick={handleCopy}
              disabled={loading}
              aria-live="polite"
            >
              {copyLabel}
            </button>
            <button
              type="button"
              className="btn-conv-action btn-conv-action--destructive"
              onClick={handleClear}
              disabled={loading}
            >
              Clear
            </button>
          </div>
        )}
      </div>

      {error && (
        <p className="inline-error conv-turn-error" role="alert">
          {error}
        </p>
      )}
      {playError && (
        <p className="conv-play-warning" role="status">
          {playError}
        </p>
      )}

      <ConversationHistory
        messages={messages}
        langAName={langAName}
        langBName={langBName}
      />

      <ConversationInput
        activeSpeaker={activeSpeaker}
        langAName={langAName}
        langBName={langBName}
        langA={langA}
        langB={langB}
        loading={loading}
        autoPlay={autoPlay}
        onAutoPlayChange={setAutoPlay}
        onTextSubmit={handleTextSubmit}
        onAudioSubmit={handleAudioSubmit}
        onSwitchSpeaker={() => setActiveSpeaker((s) => (s === 'A' ? 'B' : 'A'))}
      />
    </div>
  );
}
