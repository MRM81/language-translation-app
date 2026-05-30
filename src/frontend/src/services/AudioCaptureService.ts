// Preferred MIME type order for MediaRecorder (D-056).
// Chrome/Edge produce audio/webm;codecs=opus. Firefox produces audio/ogg;codecs=opus.
// Codec params are stripped before the blob Content-Type and the backend multipart part header.
const MIME_PREFERENCE = [
  'audio/webm;codecs=opus',
  'audio/webm',
  'audio/ogg;codecs=opus',
  'audio/ogg',
] as const;

export interface AudioCaptureResult {
  blob: Blob;
  /** Base MIME type without codec params, e.g. "audio/webm". Sent as the multipart Content-Type. */
  mimeType: string;
}

/** Returns the first MIME type from the preference list that this browser supports, or null. */
export function detectSupportedMimeType(): string | null {
  if (typeof MediaRecorder === 'undefined') return null;
  return MIME_PREFERENCE.find(t => MediaRecorder.isTypeSupported(t)) ?? null;
}

/**
 * Manages a single browser MediaRecorder session.
 *
 * Usage:
 *   const svc = new AudioCaptureService();
 *   await svc.start();          // may throw — see below
 *   const result = await svc.stop();
 *
 * start() throws:
 *   - Error('NO_MIME_SUPPORTED')  — no candidate MIME type is supported by this browser
 *   - DOMException(name: 'NotAllowedError') — user denied microphone permission
 *
 * Call abort() to cancel a recording without producing a result (e.g. on component unmount).
 */
export class AudioCaptureService {
  private recorder: MediaRecorder | null = null;
  private chunks: Blob[] = [];
  private stream: MediaStream | null = null;
  private negotiatedMime: string | null = null;

  async start(): Promise<void> {
    const mime = detectSupportedMimeType();
    if (!mime) throw new Error('NO_MIME_SUPPORTED');

    this.stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    this.negotiatedMime = mime;
    this.chunks = [];

    this.recorder = new MediaRecorder(this.stream, { mimeType: mime });
    this.recorder.ondataavailable = e => {
      if (e.data.size > 0) this.chunks.push(e.data);
    };
    this.recorder.start(100);
  }

  stop(): Promise<AudioCaptureResult> {
    return new Promise((resolve, reject) => {
      const { recorder, chunks, negotiatedMime } = this;
      if (!recorder || recorder.state === 'inactive' || !negotiatedMime) {
        reject(new Error('NOT_RECORDING'));
        return;
      }
      const baseMime = negotiatedMime.split(';')[0].trim();
      recorder.onstop = () =>
        resolve({ blob: new Blob(chunks, { type: baseMime }), mimeType: baseMime });
      recorder.onerror = () => reject(new Error('RECORDING_FAILED'));
      this.stream?.getTracks().forEach(t => t.stop());
      recorder.stop();
    });
  }

  abort(): void {
    this.stream?.getTracks().forEach(t => t.stop());
    const r = this.recorder;
    if (r && r.state !== 'inactive') {
      r.ondataavailable = null;
      r.onstop = null;
      r.onerror = null;
      r.stop();
    }
    this.recorder = null;
    this.stream = null;
    this.chunks = [];
    this.negotiatedMime = null;
  }
}
