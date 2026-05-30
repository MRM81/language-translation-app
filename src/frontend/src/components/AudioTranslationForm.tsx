import { useEffect, useRef, useState } from 'react';
import { translateAudio } from '../api/translationApi';
import type { AudioCaptureResult } from '../services/AudioCaptureService';
import { AudioCaptureService } from '../services/AudioCaptureService';
import type { ApiErrorResponse, LanguageOption, TranslationResult } from '../types/api';
import { LanguageSelect } from './LanguageSelect';
import { PushToTalkButton } from './PushToTalkButton';
import { RecordingIndicator } from './RecordingIndicator';
import { RecordingTimer } from './RecordingTimer';

const MAX_BYTES = 10 * 1024 * 1024;
const MAX_RECORD_SECONDS = 60; // Q-027: confirm if this limit should change
const ACCEPTED_UPLOAD_TYPES = ['audio/webm', 'audio/ogg', 'audio/mp4', 'audio/mpeg', 'audio/wav'];

type RecordState = 'idle' | 'recording' | 'uploading';
type Tab = 'record' | 'upload';

interface Props {
  languages: LanguageOption[];
  onResult: (result: TranslationResult) => void;
  onError: (error: ApiErrorResponse | string) => void;
  onSubmitStart: () => void;
}

export function AudioTranslationForm({ languages, onResult, onError, onSubmitStart }: Props) {
  const [activeTab, setActiveTab] = useState<Tab>('record');

  // Shared language state — persists across tab switches
  const [sourceLanguage, setSourceLanguage] = useState('');
  const [targetLanguage, setTargetLanguage] = useState('');

  function handleSwap() {
    const prev = sourceLanguage;
    setSourceLanguage(targetLanguage);
    setTargetLanguage(prev);
  }

  // Record tab
  const [recordState, setRecordState] = useState<RecordState>('idle');
  const [recordError, setRecordError] = useState('');
  const [elapsedSeconds, setElapsedSeconds] = useState(0);
  const captureRef = useRef<AudioCaptureService | null>(null);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const stoppingRef = useRef(false);

  // Upload tab
  const [file, setFile] = useState<File | null>(null);
  const [uploadLoading, setUploadLoading] = useState(false);
  const [uploadError, setUploadError] = useState('');
  const [mimeWarning, setMimeWarning] = useState('');

  useEffect(() => {
    return () => {
      captureRef.current?.abort();
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, []);

  const isLocked = recordState !== 'idle' || uploadLoading;

  // ── Record tab ─────────────────────────────────────────────────────────────

  function stopTimer() {
    if (timerRef.current) {
      clearInterval(timerRef.current);
      timerRef.current = null;
    }
  }

  async function handleStartRecording() {
    if (!targetLanguage) {
      setRecordError('Please select a target language before recording.');
      return;
    }
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
          'Audio recording is not supported in this browser. Please use Chrome, Edge, or Firefox.',
        );
      } else {
        setRecordError('Could not start recording. Please try again.');
      }
      return;
    }

    setRecordState('recording');
    setElapsedSeconds(0);

    timerRef.current = setInterval(() => {
      setElapsedSeconds(prev => {
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
    onSubmitStart();

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
      const data = await translateAudio(audioFile, targetLanguage, sourceLanguage);
      onResult({ kind: 'audio', data });
    } catch (raw) {
      onError(raw as ApiErrorResponse | string);
    } finally {
      setRecordState('idle');
      setElapsedSeconds(0);
    }
  }

  // ── Upload tab ─────────────────────────────────────────────────────────────

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const selected = e.target.files?.[0] ?? null;
    setFile(selected);
    setUploadError('');
    if (selected) {
      const baseType = selected.type.split(';')[0].trim();
      setMimeWarning(
        selected.type && !ACCEPTED_UPLOAD_TYPES.includes(baseType)
          ? `Your file type (${selected.type}) may not be supported. Accepted: ${ACCEPTED_UPLOAD_TYPES.join(', ')}.`
          : '',
      );
    } else {
      setMimeWarning('');
    }
  }

  function validateUpload(): string {
    if (!file) return 'Please select an audio file.';
    if (file.size > MAX_BYTES) return 'File size must be 10 MB or less.';
    if (!targetLanguage) return 'Please select a target language.';
    if (sourceLanguage && sourceLanguage === targetLanguage)
      return 'Source and target languages must be different.';
    return '';
  }

  async function handleUploadSubmit(e: React.FormEvent) {
    e.preventDefault();
    const err = validateUpload();
    if (err) {
      setUploadError(err);
      return;
    }
    setUploadError('');
    setUploadLoading(true);
    onSubmitStart();
    try {
      const data = await translateAudio(file!, targetLanguage, sourceLanguage);
      onResult({ kind: 'audio', data });
    } catch (raw) {
      onError(raw as ApiErrorResponse | string);
    } finally {
      setUploadLoading(false);
    }
  }

  // ── Render ─────────────────────────────────────────────────────────────────

  return (
    <div className="translation-form">
      <h2>Audio Translation</h2>

      <div className="lang-pair-row">
        <LanguageSelect
          id="audio-source-lang"
          label="Source language"
          value={sourceLanguage}
          onChange={setSourceLanguage}
          languages={languages}
          includeAuto
          disabled={isLocked}
        />
        <button
          type="button"
          className="btn-swap"
          onClick={handleSwap}
          disabled={isLocked}
          aria-label="Swap source and target languages"
          title="Swap languages"
        >
          ⇄
        </button>
        <LanguageSelect
          id="audio-target-lang"
          label="Target language"
          value={targetLanguage}
          onChange={setTargetLanguage}
          languages={languages}
          disabled={isLocked}
          required
        />
      </div>

      <div className="audio-tabs" role="tablist" aria-label="Audio input mode">
        <button
          role="tab"
          type="button"
          aria-selected={activeTab === 'record'}
          className={`audio-tab${activeTab === 'record' ? ' active' : ''}`}
          onClick={() => setActiveTab('record')}
          disabled={isLocked}
        >
          Record
        </button>
        <button
          role="tab"
          type="button"
          aria-selected={activeTab === 'upload'}
          className={`audio-tab${activeTab === 'upload' ? ' active' : ''}`}
          onClick={() => setActiveTab('upload')}
          disabled={isLocked}
        >
          Upload File
        </button>
      </div>

      {activeTab === 'record' && (
        <div role="tabpanel" aria-label="Record audio" className="audio-tab-panel">
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
            Select a target language, press Record, speak, then press Stop. Maximum{' '}
            {MAX_RECORD_SECONDS} seconds.
          </p>
        </div>
      )}

      {activeTab === 'upload' && (
        <form
          role="tabpanel"
          aria-label="Upload audio file"
          className="audio-tab-panel"
          onSubmit={handleUploadSubmit}
          noValidate
        >
          <div className="field">
            <label htmlFor="audio-file">Audio file</label>
            <input
              id="audio-file"
              type="file"
              accept="audio/*"
              onChange={handleFileChange}
              disabled={uploadLoading}
            />
            <span className="field-hint">Accepted: webm, ogg, mp3, wav — max 10 MB</span>
            {mimeWarning && (
              <p className="inline-warning" role="status">
                {mimeWarning}
              </p>
            )}
          </div>

          {uploadError && (
            <p className="inline-error" role="alert">
              {uploadError}
            </p>
          )}

          <button type="submit" disabled={uploadLoading || !file} className="btn-primary">
            {uploadLoading ? 'Translating…' : 'Translate'}
          </button>
        </form>
      )}
    </div>
  );
}
