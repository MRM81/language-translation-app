// Types matching the backend DTOs exactly.
// Field names follow the camelCase JSON serialization of the .NET DTOs.

export interface LanguageOption {
  code: string;
  name: string;
}

export interface LanguageListResponse {
  languages: LanguageOption[];
  correlationId: string;
}

export interface TextTranslationRequest {
  sourceText: string;
  sourceLanguage?: string;
  targetLanguage: string;
}

export interface TextTranslationResponse {
  translatedText: string;
  sourceLanguage: string;
  targetLanguage: string;
  provider: string;
  correlationId: string;
}

export interface AudioTranslationResponse {
  transcribedText: string;
  translatedText: string;
  sourceLanguage: string;
  targetLanguage: string;
  provider: string;
  correlationId: string;
}

export interface ApiErrorDetail {
  field: string;
  message: string;
}

export interface ApiErrorResponse {
  errorCode: string;
  message: string;
  details: ApiErrorDetail[];
  correlationId: string;
}

export type TranslationResult =
  | { kind: 'text'; data: TextTranslationResponse }
  | { kind: 'audio'; data: AudioTranslationResponse };
