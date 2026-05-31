import { useEffect, useRef } from 'react';
import type { ConversationMessage } from '../types/conversation';
import { ConversationMessage as ConvMsg } from './ConversationMessage';

interface Props {
  messages: ConversationMessage[];
  langAName: string;
  langBName: string;
}

export function ConversationHistory({ messages, langAName, langBName }: Props) {
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  if (messages.length === 0) {
    return (
      <div className="conv-history conv-history--empty" aria-live="polite">
        <p>Conversation will appear here. Select languages above and start translating.</p>
      </div>
    );
  }

  return (
    <div className="conv-history" aria-live="polite" aria-atomic="false">
      {messages.map((msg) => (
        <ConvMsg key={msg.id} message={msg} langAName={langAName} langBName={langBName} />
      ))}
      <div ref={bottomRef} />
    </div>
  );
}
