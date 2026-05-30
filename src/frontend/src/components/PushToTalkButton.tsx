interface Props {
  state: 'idle' | 'recording' | 'uploading';
  onStart: () => void;
  onStop: () => void;
}

function MicIcon() {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      width="22"
      height="22"
      viewBox="0 0 24 24"
      fill="currentColor"
      aria-hidden="true"
    >
      <path d="M12 14a3 3 0 0 0 3-3V5a3 3 0 0 0-6 0v6a3 3 0 0 0 3 3z" />
      <path d="M19 11a1 1 0 0 0-2 0 5 5 0 0 1-10 0 1 1 0 0 0-2 0 7 7 0 0 0 6 6.92V20H9a1 1 0 0 0 0 2h6a1 1 0 0 0 0-2h-2v-2.08A7 7 0 0 0 19 11z" />
    </svg>
  );
}

export function PushToTalkButton({ state, onStart, onStop }: Props) {
  if (state === 'recording') {
    return (
      <button
        type="button"
        className="btn-stop-recording"
        onClick={onStop}
        aria-label="Stop recording"
      >
        <span aria-hidden="true" style={{ fontSize: '1.25rem', lineHeight: 1 }}>&#9632;</span>
        <span className="btn-record-label">Stop</span>
      </button>
    );
  }

  return (
    <button
      type="button"
      className="btn-record"
      onClick={onStart}
      disabled={state === 'uploading'}
      aria-label={state === 'uploading' ? 'Translating audio' : 'Start recording'}
    >
      {state === 'uploading' ? (
        <>
          <span aria-hidden="true" style={{ fontSize: '1.25rem', lineHeight: 1 }}>&#8987;</span>
          <span className="btn-record-label">Wait</span>
        </>
      ) : (
        <>
          <MicIcon />
          <span className="btn-record-label">Record</span>
        </>
      )}
    </button>
  );
}
