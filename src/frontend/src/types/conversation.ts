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
  version: number;
  createdAt: string;
  updatedAt: string;
  languageA: string;
  languageB: string;
  messages: ConversationMessage[];
}
