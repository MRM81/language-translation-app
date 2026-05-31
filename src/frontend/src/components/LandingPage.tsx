interface LandingPageProps {
  onStart: () => void;
}

export function LandingPage({ onStart }: LandingPageProps) {
  return (
    <div className="landing-page">
      <div className="landing-hero">
        <img
          src="/logo.png"
          alt="My Translation App logo"
          className="landing-logo"
        />

        <p className="landing-description">
          Translate text and speech across 37 languages with fast conversation
          tools, push-to-talk input, and spoken playback.
        </p>

        <ul className="landing-features" aria-label="Key features">
          <li>Text Translation</li>
          <li>Audio Translation &amp; Push-To-Talk</li>
          <li>Conversation Mode</li>
          <li>37 Languages</li>
        </ul>

        <p className="landing-tech">
          Built with React · .NET · Azure AI Speech · Azure Translator · AWS
        </p>

        <button type="button" className="btn-primary landing-cta" onClick={onStart}>
          Start Translating
        </button>
      </div>
    </div>
  );
}
