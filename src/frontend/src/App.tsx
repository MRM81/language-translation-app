import { useEffect, useState } from 'react';
import { fetchLanguages } from './api/translationApi';
import { AudioTranslationForm } from './components/AudioTranslationForm';
import { ErrorPanel } from './components/ErrorPanel';
import { ResultPanel } from './components/ResultPanel';
import { TextTranslationForm } from './components/TextTranslationForm';
import './styles/app.css';
import type { ApiErrorResponse, LanguageOption, TranslationResult } from './types/api';

export function App() {
  const [languages, setLanguages] = useState<LanguageOption[]>([]);
  const [languagesLoading, setLanguagesLoading] = useState(true);
  const [languagesError, setLanguagesError] = useState('');

  const [result, setResult] = useState<TranslationResult | null>(null);
  const [error, setError] = useState<ApiErrorResponse | string | null>(null);

  useEffect(() => {
    fetchLanguages()
      .then((res) => setLanguages(res.languages))
      .catch(() =>
        setLanguagesError(
          'Could not load the language list. Please check that the backend is running and refresh the page.',
        ),
      )
      .finally(() => setLanguagesLoading(false));
  }, []);

  function handleSubmitStart() {
    setResult(null);
    setError(null);
  }

  function handleResult(r: TranslationResult) {
    setResult(r);
    setError(null);
  }

  function handleError(e: ApiErrorResponse | string) {
    setError(e);
    setResult(null);
  }

  return (
    <>
      <header className="app-header">
        <h1>My Translation App</h1>
        <p>Translate text or audio between languages.</p>
      </header>

      <main className="app-main">
        {languagesLoading && (
          <div className="status-banner loading" role="status">
            Loading available languages…
          </div>
        )}
        {languagesError && (
          <div className="status-banner error" role="alert">
            {languagesError}
          </div>
        )}

        {!languagesLoading && (
          <div className="forms-grid">
            <TextTranslationForm
              languages={languages}
              onResult={handleResult}
              onError={handleError}
              onSubmitStart={handleSubmitStart}
            />
            <AudioTranslationForm
              languages={languages}
              onResult={handleResult}
              onError={handleError}
              onSubmitStart={handleSubmitStart}
            />
          </div>
        )}

        <div className="results-area" aria-live="polite" aria-atomic="false">
          {error ? (
            <ErrorPanel error={error} />
          ) : (
            <ResultPanel result={result} />
          )}
        </div>
      </main>
    </>
  );
}
