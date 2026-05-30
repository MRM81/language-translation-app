interface Props {
  active: boolean;
}

export function RecordingIndicator({ active }: Props) {
  if (!active) return null;
  return (
    <div className="recording-indicator" role="status" aria-live="polite">
      <span className="recording-dot" aria-hidden="true" />
      Recording
    </div>
  );
}
