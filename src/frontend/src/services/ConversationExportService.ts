import type { ConversationMessage, ConversationSession } from '../types/conversation';

const SEPARATOR = '-'.repeat(20);

function dateStamp(): string {
  return new Date().toISOString().slice(0, 10);
}

function download(content: string, filename: string, mimeType: string): void {
  const blob = new Blob([content], { type: mimeType });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

function buildTxt(
  messages: ConversationMessage[],
  langAName: string,
  langBName: string,
): string {
  return messages
    .map((msg) => {
      const speakerName = msg.speaker === 'A' ? langAName : langBName;
      return [
        `Speaker ${msg.speaker} (${speakerName})`,
        msg.originalText,
        '',
        'Translation',
        msg.translatedText,
        '',
        SEPARATOR,
      ].join('\n');
    })
    .join('\n');
}

export function exportAsTxt(
  messages: ConversationMessage[],
  langAName: string,
  langBName: string,
): void {
  if (messages.length === 0) return;
  download(
    buildTxt(messages, langAName, langBName),
    `conversation-${dateStamp()}.txt`,
    'text/plain;charset=utf-8',
  );
}

export function exportAsJson(session: ConversationSession): void {
  if (session.messages.length === 0) return;
  download(
    JSON.stringify(session, null, 2),
    `conversation-${dateStamp()}.json`,
    'application/json',
  );
}

export async function copyToClipboard(
  messages: ConversationMessage[],
  langAName: string,
  langBName: string,
): Promise<void> {
  if (messages.length === 0) return;
  if (!navigator.clipboard) throw new Error('Clipboard API unavailable');
  await navigator.clipboard.writeText(buildTxt(messages, langAName, langBName));
}
