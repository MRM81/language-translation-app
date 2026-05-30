interface Props {
  seconds: number;
  maxSeconds: number;
}

export function RecordingTimer({ seconds, maxSeconds }: Props) {
  const m = Math.floor(seconds / 60);
  const s = (seconds % 60).toString().padStart(2, '0');
  const nearLimit = seconds >= maxSeconds - 10;

  return (
    <span
      className={`recording-timer${nearLimit ? ' near-limit' : ''}`}
      aria-live="off"
      aria-label={`${seconds} of ${maxSeconds} seconds recorded`}
    >
      {m}:{s}&nbsp;/&nbsp;{Math.floor(maxSeconds / 60)}:{(maxSeconds % 60).toString().padStart(2, '0')}
    </span>
  );
}
