import { useEffect, useState } from 'react';
import { fetchLanguages } from './api/translationApi';
import { AudioTranslationForm } from './components/AudioTranslationForm';
import { ConversationMode } from './components/ConversationMode';
import { ErrorPanel } from './components/ErrorPanel';
import { LandingPage } from './components/LandingPage';
import { ResultPanel } from './components/ResultPanel';
import { TextTranslationForm } from './components/TextTranslationForm';
import './styles/app.css';
import type { ApiErrorResponse, LanguageOption, TranslationResult } from './types/api';

type Screen = 'landing' | 'workspace';
type WorkspaceMode = 'translate' | 'conversation';
type TranslationInputMode = 'text' | 'audio';

export function App() {
  const [screen, setScreen] = useState<Screen>('landing');
  const [mode, setMode] = useState<WorkspaceMode>('translate');
  const [translationInputMode, setTranslationInputMode] = useState<TranslationInputMode>('text');

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

  function handleTranslationInputModeChange(next: TranslationInputMode) {
    setTranslationInputMode(next);
    setResult(null);
    setError(null);
  }

  if (screen === 'landing') {
    return <LandingPage onStart={() => setScreen('workspace')} />;
  }

  return (
    <>
      <header className="app-header">
        <div className="app-header-inner">
          <button
            type="button"
            className="app-header-logo-btn"
            onClick={() => setScreen('landing')}
            aria-label="Return to home"
          >
            <img src="/logo.png" alt="My Translation App logo" className="app-header-logo" />
          </button>
        </div>
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
          <>
            <nav className="app-mode-nav" aria-label="Application mode">
              <button
                type="button"
                className={`mode-nav-btn${mode === 'translate' ? ' active' : ''}`}
                onClick={() => setMode('translate')}
                aria-pressed={mode === 'translate'}
              >
                Translation
              </button>
              <button
                type="button"
                className={`mode-nav-btn${mode === 'conversation' ? ' active' : ''}`}
                onClick={() => setMode('conversation')}
                aria-pressed={mode === 'conversation'}
              >
                Conversation
              </button>
            </nav>

            {mode === 'translate' && (
              <>
                <div className="translation-tabs" role="group" aria-label="Translation input type">
                  <button
                    type="button"
                    className={`translation-tab${translationInputMode === 'text' ? ' active' : ''}`}
                    onClick={() => handleTranslationInputModeChange('text')}
                    aria-pressed={translationInputMode === 'text'}
                  >
                    Text Translation
                  </button>
                  <button
                    type="button"
                    className={`translation-tab${translationInputMode === 'audio' ? ' active' : ''}`}
                    onClick={() => handleTranslationInputModeChange('audio')}
                    aria-pressed={translationInputMode === 'audio'}
                  >
                    Audio Translation
                  </button>
                </div>

                {translationInputMode === 'text' && (
                  <TextTranslationForm
                    languages={languages}
                    onResult={handleResult}
                    onError={handleError}
                    onSubmitStart={handleSubmitStart}
                  />
                )}

                {translationInputMode === 'audio' && (
                  <AudioTranslationForm
                    languages={languages}
                    onResult={handleResult}
                    onError={handleError}
                    onSubmitStart={handleSubmitStart}
                  />
                )}

                <div className="results-area" aria-live="polite" aria-atomic="false">
                  {error ? (
                    <ErrorPanel error={error} />
                  ) : (
                    <ResultPanel
                      result={result}
                      targetLangSupportsTts={
                        result
                          ? (languages.find((l) => l.code === result.data.targetLanguage)?.supportsTextToSpeech ?? true)
                          : true
                      }
                    />
                  )}
                </div>
              </>
            )}

            {mode === 'conversation' && (
              <ConversationMode languages={languages} />
            )}
          </>
        )}
      </main>
    </>
  );
}
