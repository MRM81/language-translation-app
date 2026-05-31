export interface ConversationMessage {
  id: string;
  speaker: 'A' | 'B';
  originalText: string;
  translatedText: string;
  sourceLanguage: string;
  targetLanguage: string;
  timestamp: string;
  inputType: 'text' | 'audio';
}

export const CONVERSATION_STORAGE_VERSION = 1;

export interface ConversationSession {
  id: string;
  title: string;
  isAutoTitle?: boolean;
  version: number;
  createdAt: string;
  updatedAt: string;
  languageA: string;
  languageB: string;
  messages: ConversationMessage[];
}

export const CONVERSATION_STORE_VERSION = 1;

export interface ConversationStore {
  version: number;
  activeConversationId: string | null;
  conversations: Record<string, ConversationSession>;
}

export interface ConversationSummary {
  id: string;
  title: string;
  createdAt: string;
  updatedAt: string;
  messageCount: number;
  languageA: string;
  languageB: string;
  previewText?: string;
}
