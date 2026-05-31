import { useEffect, useRef, useState } from 'react';
import type { AudioCaptureResult } from '../services/AudioCaptureService';
import { AudioCaptureService } from '../services/AudioCaptureService';
import { PushToTalkButton } from './PushToTalkButton';
import { RecordingIndicator } from './RecordingIndicator';
import { RecordingTimer } from './RecordingTimer';

const MAX_CHARS = 5000;
const MAX_BYTES = 10 * 1024 * 1024;
const MAX_RECORD_SECONDS = 60;
const ACCEPTED_TYPES = ['audio/webm', 'audio/ogg', 'audio/mp4', 'audio/mpeg', 'audio/wav'];

type InputTab = 'record' | 'text' | 'audio';
type RecordState = 'idle' | 'recording' | 'uploading';

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
  onAudioSubmit: (speaker: 'A' | 'B', file: File) => Promise<void>;
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
  const [tab, setTab] = useState<InputTab>('record');

  // Text tab state
  const [text, setText] = useState('');
  const [textError, setTextError] = useState('');

  // Audio file tab state
  const [file, setFile] = useState<File | null>(null);
  const [fileError, setFileError] = useState('');
  const [mimeWarning, setMimeWarning] = useState('');

  // Record tab state
  const [recordState, setRecordState] = useState<RecordState>('idle');
  const [recordError, setRecordError] = useState('');
  const [elapsedSeconds, setElapsedSeconds] = useState(0);
  const captureRef = useRef<AudioCaptureService | null>(null);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const stoppingRef = useRef(false);

  useEffect(() => {
    return () => {
      captureRef.current?.abort();
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, []);

  const sourceName = activeSpeaker === 'A' ? langAName : langBName;
  const targetName = activeSpeaker === 'A' ? langBName : langAName;
  const isRecording = recordState !== 'idle';
  const isLocked = loading || isRecording;

  // ── Language validation ──────────────────────────────────────────────────

  function validateLanguages(): string {
    if (!langA || !langB) return 'Please select both speaker languages above.';
    if (langA === langB) return 'Speaker A and Speaker B must use different languages.';
    return '';
  }

  // ── Record tab ────────────────────────────────────────────────────────────

  function stopTimer() {
    if (timerRef.current) {
      clearInterval(timerRef.current);
      timerRef.current = null;
    }
  }

  async function handleStartRecording() {
    const langErr = validateLanguages();
    if (langErr) { setRecordError(langErr); return; }

    setRecordError('');
    stoppingRef.current = false;

    const service = new AudioCaptureService();
    captureRef.current = service;

    try {
      await service.start();
    } catch (e) {
      captureRef.current = null;
      const err = e as DOMException & { message: string };
      if (err.name === 'NotAllowedError' || err.name === 'PermissionDeniedError') {
        setRecordError(
          'Microphone access was denied. Please allow microphone access in your browser settings and try again.',
        );
      } else if (err.message === 'NO_MIME_SUPPORTED') {
        setRecordError(
          'Audio recording is not supported in this browser. Please use the Audio File tab instead.',
        );
      } else {
        setRecordError('Could not start recording. Please try again.');
      }
      return;
    }

    setRecordState('recording');
    setElapsedSeconds(0);

    timerRef.current = setInterval(() => {
      setElapsedSeconds((prev) => {
        const next = prev + 1;
        if (next >= MAX_RECORD_SECONDS && !stoppingRef.current) {
          stoppingRef.current = true;
          setTimeout(() => handleStopRecording(), 0);
        }
        return next;
      });
    }, 1000);
  }

  async function handleStopRecording() {
    stopTimer();
    const service = captureRef.current;
    if (!service) return;
    captureRef.current = null;

    setRecordState('uploading');

    let captured: AudioCaptureResult;
    try {
      captured = await service.stop();
    } catch {
      setRecordState('idle');
      setRecordError('Recording failed. Please try again.');
      return;
    }

    const audioFile = new File(
      [captured.blob],
      `recording.${captured.mimeType.split('/')[1]}`,
      { type: captured.mimeType },
    );

    try {
      await onAudioSubmit(activeSpeaker, audioFile);
    } finally {
      setRecordState('idle');
      setElapsedSeconds(0);
    }
  }

  // ── Text tab ──────────────────────────────────────────────────────────────

  function handleTextSubmit() {
    const langErr = validateLanguages();
    if (langErr) { setTextError(langErr); return; }
    if (!text.trim()) { setTextError('Please enter text to translate.'); return; }
    if (text.length > MAX_CHARS) { setTextError(`Text must be ${MAX_CHARS} characters or fewer.`); return; }
    setTextError('');
    onTextSubmit(activeSpeaker, text);
    setText('');
  }

  // ── Audio file tab ────────────────────────────────────────────────────────

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

  async function handleAudioFileSubmit() {
    const langErr = validateLanguages();
    if (langErr) { setFileError(langErr); return; }
    if (!file) { setFileError('Please select an audio file.'); return; }
    if (file.size > MAX_BYTES) { setFileError('File size must be 10 MB or less.'); return; }
    setFileError('');
    await onAudioSubmit(activeSpeaker, file);
    setFile(null);
  }

  const charsRemaining = MAX_CHARS - text.length;
  const overLimit = text.length > MAX_CHARS;

  // ── Render ────────────────────────────────────────────────────────────────

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
            disabled={isLocked}
            aria-label="Switch active speaker"
          >
            Switch speaker
          </button>
          <label className="conv-autoplay-toggle">
            <input
              type="checkbox"
              checked={autoPlay}
              onChange={(e) => onAutoPlayChange(e.target.checked)}
              disabled={isLocked}
            />
            Auto-play
          </label>
        </div>
      </div>

      <div className="audio-tabs" role="tablist" aria-label="Input type">
        <button
          role="tab"
          type="button"
          aria-selected={tab === 'record'}
          className={`audio-tab${tab === 'record' ? ' active' : ''}`}
          onClick={() => setTab('record')}
          disabled={isLocked}
        >
          Record
        </button>
        <button
          role="tab"
          type="button"
          aria-selected={tab === 'text'}
          className={`audio-tab${tab === 'text' ? ' active' : ''}`}
          onClick={() => setTab('text')}
          disabled={isLocked}
        >
          Text
        </button>
        <button
          role="tab"
          type="button"
          aria-selected={tab === 'audio'}
          className={`audio-tab${tab === 'audio' ? ' active' : ''}`}
          onClick={() => setTab('audio')}
          disabled={isLocked}
        >
          Audio File
        </button>
      </div>

      {tab === 'record' && (
        <div className="conv-tab-panel" role="tabpanel" aria-label="Record audio">
          <div className="record-controls">
            <PushToTalkButton
              state={recordState}
              onStart={handleStartRecording}
              onStop={handleStopRecording}
            />
            {recordState === 'recording' && (
              <>
                <RecordingIndicator active />
                <RecordingTimer seconds={elapsedSeconds} maxSeconds={MAX_RECORD_SECONDS} />
              </>
            )}
          </div>
          {recordError && (
            <p className="inline-error" role="alert">
              {recordError}
            </p>
          )}
          <p className="field-hint">
            Select languages above, press Record, speak, then press Stop.
            Maximum {MAX_RECORD_SECONDS} seconds.
          </p>
        </div>
      )}

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
            <span className="field-hint">Accepted: webm, ogg, mp3, wav, mp4 — max 10 MB</span>
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
            onClick={handleAudioFileSubmit}
            disabled={loading || !file}
          >
            {loading ? 'Translating…' : 'Translate & Speak'}
          </button>
        </div>
      )}
    </div>
  );
}
