import { useRef, useState } from 'react';
import { synthesizeSpeech } from '../api/translationApi';
import type { ApiErrorResponse, TranslationResult } from '../types/api';

interface Props {
  result: TranslationResult | null;
  targetLangSupportsTts?: boolean;
}

type PlayState = 'idle' | 'loading' | 'playing' | 'error';

export function ResultPanel({ result, targetLangSupportsTts = true }: Props) {
  const [playState, setPlayState] = useState<PlayState>('idle');
  const [playError, setPlayError] = useState<string | null>(null);
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const objectUrlRef = useRef<string | null>(null);

  function stopCurrentAudio() {
    if (audioRef.current) {
      audioRef.current.pause();
      audioRef.current.onended = null;
      audioRef.current = null;
    }
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }
  }

  async function handlePlay() {
    if (!result) return;

    stopCurrentAudio();
    setPlayState('loading');
    setPlayError(null);

    try {
      const blob = await synthesizeSpeech(
        result.data.translatedText,
        result.data.targetLanguage,
      );

      const url = URL.createObjectURL(blob);
      objectUrlRef.current = url;

      const audio = new Audio(url);
      audioRef.current = audio;

      audio.onended = () => {
        setPlayState('idle');
        stopCurrentAudio();
      };

      audio.onerror = () => {
        setPlayState('error');
        setPlayError('Audio playback failed. Please try again.');
        stopCurrentAudio();
      };

      setPlayState('playing');
      await audio.play();
    } catch (err) {
      stopCurrentAudio();
      setPlayState('error');
      // Only show the message if err is a proper ApiErrorResponse (has errorCode).
      // Raw JS errors (SyntaxError, TypeError, etc.) must not reach the UI.
      const apiErr = err as ApiErrorResponse;
      if (apiErr?.errorCode !== undefined && apiErr?.message) {
        setPlayError(apiErr.message);
      } else {
        setPlayError('Speech synthesis failed. Please try again.');
      }
    }
  }

  if (!result) {
    return (
      <div className="panel panel-empty">
        <p>Translation result will appear here.</p>
      </div>
    );
  }

  const { data } = result;

  const playIcon = playState === 'playing' ? '◼' : '▶';
  const playLabel =
    !targetLangSupportsTts ? 'Audio unavailable' :
    playState === 'loading' ? 'Loading…' :
    playState === 'playing' ? 'Playing…' :
    'Play';

  return (
    <div className="panel panel-result">
      {result.kind === 'audio' && (
        <>
          <p className="result-section-label">Transcript</p>
          <p className="result-transcript">{result.data.transcribedText}</p>
        </>
      )}

      <p className="result-section-label">Translation</p>
      <p className="result-hero">{data.translatedText}</p>

      <div className="result-play-row">
        <button
          className={`play-button play-button--${targetLangSupportsTts ? playState : 'idle'}`}
          onClick={targetLangSupportsTts ? handlePlay : undefined}
          disabled={!targetLangSupportsTts || playState === 'loading' || playState === 'playing'}
          aria-label={targetLangSupportsTts ? 'Play translated speech' : 'Audio unavailable for this language'}
        >
          {targetLangSupportsTts && <span aria-hidden="true">{playIcon}</span>}
          {playLabel}
        </button>
        {playState === 'error' && playError && (
          <span className="play-error" role="alert">{playError}</span>
        )}
      </div>

      <div className="result-meta">
        <span className="result-meta-item">
          {result.kind === 'audio' ? 'Audio' : 'Text'} &rarr; <strong>{data.targetLanguage}</strong>
        </span>
        <span className="result-meta-item">
          Provider: <code>{data.provider}</code>
        </span>
        <span className="result-meta-item">
          ID: <code>{data.correlationId}</code>
        </span>
      </div>
    </div>
  );
}
