import { useState } from 'react';
import { translateText } from '../api/translationApi';
import type { ApiErrorResponse, LanguageOption, TranslationResult } from '../types/api';
import { LanguageSelect } from './LanguageSelect';

const MAX_CHARS = 5000;

interface Props {
  languages: LanguageOption[];
  onResult: (result: TranslationResult) => void;
  onError: (error: ApiErrorResponse | string) => void;
  onSubmitStart: () => void;
}

export function TextTranslationForm({ languages, onResult, onError, onSubmitStart }: Props) {
  const [sourceText, setSourceText] = useState('');
  const [sourceLanguage, setSourceLanguage] = useState('');
  const [targetLanguage, setTargetLanguage] = useState('');
  const [loading, setLoading] = useState(false);
  const [validationError, setValidationError] = useState('');

  function handleSwap() {
    const prev = sourceLanguage;
    setSourceLanguage(targetLanguage);
    setTargetLanguage(prev);
  }

  function validate(): string {
    if (!sourceText.trim()) return 'Please enter text to translate.';
    if (sourceText.length > MAX_CHARS) return `Text must be ${MAX_CHARS} characters or fewer.`;
    if (!targetLanguage) return 'Please select a target language.';
    if (sourceLanguage && sourceLanguage === targetLanguage)
      return 'Source and target languages must be different.';
    return '';
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const err = validate();
    if (err) {
      setValidationError(err);
      return;
    }
    setValidationError('');
    setLoading(true);
    onSubmitStart();

    try {
      const data = await translateText({
        sourceText,
        sourceLanguage: sourceLanguage || undefined,
        targetLanguage,
      });
      onResult({ kind: 'text', data });
    } catch (raw) {
      onError(raw as ApiErrorResponse | string);
    } finally {
      setLoading(false);
    }
  }

  const charsRemaining = MAX_CHARS - sourceText.length;
  const overLimit = sourceText.length > MAX_CHARS;

  return (
    <form className="translation-form" onSubmit={handleSubmit} noValidate>
      <h2>Text Translation</h2>

      <div className="lang-pair-row">
        <LanguageSelect
          id="text-source-lang"
          label="Source language"
          value={sourceLanguage}
          onChange={setSourceLanguage}
          languages={languages}
          includeAuto
          disabled={loading}
        />
        <button
          type="button"
          className="btn-swap"
          onClick={handleSwap}
          disabled={loading}
          aria-label="Swap source and target languages"
          title="Swap languages"
        >
          ⇄
        </button>
        <LanguageSelect
          id="text-target-lang"
          label="Target language"
          value={targetLanguage}
          onChange={setTargetLanguage}
          languages={languages}
          disabled={loading}
          required
        />
      </div>

      <div className="field">
        <label htmlFor="source-text">Text to translate</label>
        <textarea
          id="source-text"
          value={sourceText}
          onChange={(e) => setSourceText(e.target.value)}
          rows={5}
          disabled={loading}
          placeholder="Enter text here…"
        />
        <span className={`char-count ${overLimit ? 'over-limit' : ''}`}>
          {charsRemaining < 0
            ? `${Math.abs(charsRemaining)} characters over limit`
            : `${charsRemaining} characters remaining`}
        </span>
      </div>

      {validationError && (
        <p className="inline-error" role="alert">
          {validationError}
        </p>
      )}

      <button type="submit" disabled={loading || overLimit} className="btn-primary">
        {loading ? 'Translating…' : 'Translate'}
      </button>
    </form>
  );
}
