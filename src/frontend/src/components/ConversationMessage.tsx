import type { ConversationMessage as Msg } from '../types/conversation';

interface Props {
  message: Msg;
  langAName: string;
  langBName: string;
}

export function ConversationMessage({ message, langAName, langBName }: Props) {
  const speakerName = message.speaker === 'A' ? langAName : langBName;
  const time = new Date(message.timestamp).toLocaleTimeString([], {
    hour: '2-digit',
    minute: '2-digit',
  });
  const side = message.speaker.toLowerCase() as 'a' | 'b';

  return (
    <div className={`conv-message conv-message--${side}`}>
      <div className="conv-message__header">
        <span className="conv-message__speaker">
          {message.speaker} · {speakerName}
        </span>
        <span className="conv-message__time">{time}</span>
      </div>
      <p className="conv-message__original">{message.originalText}</p>
      <p className="conv-message__translated">{message.translatedText}</p>
      {message.inputType === 'audio' && (
        <span className="conv-message__type-badge">Audio</span>
      )}
    </div>
  );
}
