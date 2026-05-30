import type { LanguageOption } from '../types/api';

interface Props {
  id: string;
  label: string;
  value: string;
  onChange: (value: string) => void;
  languages: LanguageOption[];
  includeAuto?: boolean;
  disabled?: boolean;
  required?: boolean;
}

export function LanguageSelect({
  id,
  label,
  value,
  onChange,
  languages,
  includeAuto = false,
  disabled = false,
  required = false,
}: Props) {
  return (
    <div className="field">
      <label htmlFor={id}>{label}</label>
      <select
        id={id}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        disabled={disabled}
        required={required}
        aria-required={required ? 'true' : undefined}
      >
        {includeAuto && <option value="">Auto-detect</option>}
        {!includeAuto && <option value="">— select —</option>}
        {languages.map((lang) => (
          <option key={lang.code} value={lang.code}>
            {lang.name}
          </option>
        ))}
      </select>
    </div>
  );
}
