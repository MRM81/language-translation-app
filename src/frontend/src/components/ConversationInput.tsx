import { useState } from 'react';

const MAX_CHARS = 5000;
const MAX_BYTES = 10 * 1024 * 1024;
const ACCEPTED_TYPES = ['audio/webm', 'audio/ogg', 'audio/mp4', 'audio/mpeg', 'audio/wav'];

type InputTab = 'text' | 'audio';

interface Props {
  activeSpeaker: 'A' | 'B';
  langAName: string;
  langBName: string;
  langA: string;
  langB: string;
  loading: boolean;
  autoPlay: boolean;
  onAutoPlayChange: (value: boolean) => void;
  onTextSubmit: (speaker: 'A' | 'B', text: string) => void;
  onAudioSubmit: (speaker: 'A' | 'B', file: File) => void;
  onSwitchSpeaker: () => void;
}

export function ConversationInput({
  activeSpeaker,
  langAName,
  langBName,
  langA,
  langB,
  loading,
  autoPlay,
  onAutoPlayChange,
  onTextSubmit,
  onAudioSubmit,
  onSwitchSpeaker,
}: Props) {
  const [tab, setTab] = useState<InputTab>('text');
  const [text, setText] = useState('');
  const [textError, setTextError] = useState('');
  const [file, setFile] = useState<File | null>(null);
  const [fileError, setFileError] = useState('');
  const [mimeWarning, setMimeWarning] = useState('');

  const sourceName = activeSpeaker === 'A' ? langAName : langBName;
  const targetName = activeSpeaker === 'A' ? langBName : langAName;

  function validateLanguages(): string {
    if (!langA || !langB) return 'Please select both speaker languages above.';
    if (langA === langB) return 'Speaker A and Speaker B must use different languages.';
    return '';
  }

  function handleTextSubmit() {
    const langErr = validateLanguages();
    if (langErr) { setTextError(langErr); return; }
    if (!text.trim()) { setTextError('Please enter text to translate.'); return; }
    if (text.length > MAX_CHARS) { setTextError(`Text must be ${MAX_CHARS} characters or fewer.`); return; }
    setTextError('');
    onTextSubmit(activeSpeaker, text);
    setText('');
  }

  function handleAudioSubmit() {
    const langErr = validateLanguages();
    if (langErr) { setFileError(langErr); return; }
    if (!file) { setFileError('Please select an audio file.'); return; }
    if (file.size > MAX_BYTES) { setFileError('File size must be 10 MB or less.'); return; }
    setFileError('');
    onAudioSubmit(activeSpeaker, file);
    setFile(null);
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const selected = e.target.files?.[0] ?? null;
    setFile(selected);
    setFileError('');
    if (selected) {
      const baseType = selected.type.split(';')[0].trim();
      setMimeWarning(
        selected.type && !ACCEPTED_TYPES.includes(baseType)
          ? `Your file type (${selected.type}) may not be supported. Accepted: ${ACCEPTED_TYPES.join(', ')}.`
          : '',
      );
    } else {
      setMimeWarning('');
    }
  }

  const charsRemaining = MAX_CHARS - text.length;
  const overLimit = text.length > MAX_CHARS;

  return (
    <div className="conv-input-section">
      <div className="conv-speaker-row">
        <div className="conv-speaker-label">
          <span className={`conv-speaker-badge conv-speaker-badge--${activeSpeaker.toLowerCase()}`}>
            {activeSpeaker}
          </span>
          <span className="conv-speaker-text">
            {sourceName || 'Speaker'} &rarr; {targetName || 'Target'}
          </span>
        </div>
        <div className="conv-controls-row">
          <button
            type="button"
            className="btn-switch-speaker"
            onClick={onSwitchSpeaker}
            disabled={loading}
            aria-label="Switch active speaker"
          >
            Switch speaker
          </button>
          <label className="conv-autoplay-toggle">
            <input
              type="checkbox"
              checked={autoPlay}
              onChange={(e) => onAutoPlayChange(e.target.checked)}
              disabled={loading}
            />
            Auto-play
          </label>
        </div>
      </div>

      <div className="audio-tabs" role="tablist" aria-label="Input type">
        <button
          role="tab"
          type="button"
          aria-selected={tab === 'text'}
          className={`audio-tab${tab === 'text' ? ' active' : ''}`}
          onClick={() => setTab('text')}
          disabled={loading}
        >
          Text
        </button>
        <button
          role="tab"
          type="button"
          aria-selected={tab === 'audio'}
          className={`audio-tab${tab === 'audio' ? ' active' : ''}`}
          onClick={() => setTab('audio')}
          disabled={loading}
        >
          Audio File
        </button>
      </div>

      {tab === 'text' && (
        <div className="conv-tab-panel" role="tabpanel" aria-label="Text input">
          <div className="field">
            <textarea
              value={text}
              onChange={(e) => setText(e.target.value)}
              rows={3}
              disabled={loading}
              placeholder={`Type in ${sourceName || 'the source language'}…`}
              aria-label="Text to translate"
            />
            <span className={`char-count ${overLimit ? 'over-limit' : ''}`}>
              {charsRemaining < 0
                ? `${Math.abs(charsRemaining)} characters over limit`
                : `${charsRemaining} characters remaining`}
            </span>
          </div>
          {textError && (
            <p className="inline-error" role="alert">
              {textError}
            </p>
          )}
          <button
            type="button"
            className="btn-primary"
            onClick={handleTextSubmit}
            disabled={loading || overLimit}
          >
            {loading ? 'Translating…' : 'Translate & Speak'}
          </button>
        </div>
      )}

      {tab === 'audio' && (
        <div className="conv-tab-panel" role="tabpanel" aria-label="Audio file input">
          <div className="field">
            <label htmlFor="conv-audio-file">Audio file</label>
            <input
              id="conv-audio-file"
              type="file"
              accept="audio/*"
              onChange={handleFileChange}
              disabled={loading}
            />
            <span className="field-hint">Accepted: webm, ogg, mp3, wav — max 10 MB</span>
            {mimeWarning && (
              <p className="inline-warning" role="status">
                {mimeWarning}
              </p>
            )}
          </div>
          {fileError && (
            <p className="inline-error" role="alert">
              {fileError}
            </p>
          )}
          <button
            type="button"
            className="btn-primary"
            onClick={handleAudioSubmit}
            disabled={loading || !file}
          >
            {loading ? 'Translating…' : 'Translate & Speak'}
          </button>
        </div>
      )}
    </div>
  );
}
