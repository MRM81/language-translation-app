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
