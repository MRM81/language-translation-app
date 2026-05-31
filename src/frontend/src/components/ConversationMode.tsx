import { useEffect, useRef, useState } from 'react';
import { synthesizeSpeech, translateAudio, translateText } from '../api/translationApi';
import { copyToClipboard, exportAsJson, exportAsTxt } from '../services/ConversationExportService';
import { clearSession, loadSession, saveSession } from '../services/ConversationStorageService';
import type { LanguageOption } from '../types/api';
import {
  CONVERSATION_STORAGE_VERSION,
  type ConversationMessage,
  type ConversationSession,
} from '../types/conversation';
import { ConversationHistory } from './ConversationHistory';
import { ConversationInput } from './ConversationInput';
import { LanguageSelect } from './LanguageSelect';

interface Props {
  languages: LanguageOption[];
}

type CopyStatus = 'idle' | 'copied' | 'error';

function generateId(): string {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
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
  const copyTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const langAName = languages.find((l) => l.code === langA)?.name ?? langA;
  const langBName = languages.find((l) => l.code === langB)?.name ?? langB;

  // Restore session on mount
  useEffect(() => {
    const session = loadSession();
    if (session) {
      setLangA(session.languageA);
      setLangB(session.languageB);
      setMessages(session.messages);
    }
  }, []);

  // Persist session whenever conversation state changes
  useEffect(() => {
    if (messages.length === 0) return;
    const session: ConversationSession = {
      version: CONVERSATION_STORAGE_VERSION,
      createdAt: messages[0].timestamp,
      updatedAt: messages[messages.length - 1].timestamp,
      languageA: langA,
      languageB: langB,
      messages,
    };
    saveSession(session);
  }, [messages, langA, langB]);

  // Clean up copy feedback timer on unmount
  useEffect(() => {
    return () => {
      if (copyTimerRef.current) clearTimeout(copyTimerRef.current);
    };
  }, []);

  function handleSwap() {
    // History is immutable — messages retain their original language pairs.
    // Swap only affects future turns.
    setLangA(langB);
    setLangB(langA);
  }

  function handleClear() {
    setMessages([]);
    setActiveSpeaker('A');
    setError(null);
    setPlayError(null);
    clearSession();
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

      if (autoPlay) {
        await tryAutoPlay(data.translatedText, targetLang);
      }
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

      if (autoPlay) {
        await tryAutoPlay(data.translatedText, targetLang);
      }
    } catch (err: unknown) {
      const apiErr = err as { message?: string };
      setError(apiErr?.message ?? 'Translation failed. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  function handleExportTxt() {
    exportAsTxt(messages, langAName, langBName);
  }

  function handleExportJson() {
    const session: ConversationSession = {
      version: CONVERSATION_STORAGE_VERSION,
      createdAt: messages[0]?.timestamp ?? new Date().toISOString(),
      updatedAt: messages[messages.length - 1]?.timestamp ?? new Date().toISOString(),
      languageA: langA,
      languageB: langB,
      messages,
    };
    exportAsJson(session);
  }

  async function handleCopy() {
    if (copyTimerRef.current) clearTimeout(copyTimerRef.current);
    try {
      await copyToClipboard(messages, langAName, langBName);
      setCopyStatus('copied');
      copyTimerRef.current = setTimeout(() => setCopyStatus('idle'), 2000);
    } catch {
      setCopyStatus('error');
      copyTimerRef.current = setTimeout(() => setCopyStatus('idle'), 3000);
    }
  }

  const copyLabel =
    copyStatus === 'copied' ? 'Copied!' :
    copyStatus === 'error' ? 'Failed' :
    'Copy';

  return (
    <div className="conversation-mode">
      <div className="conv-lang-header">
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
