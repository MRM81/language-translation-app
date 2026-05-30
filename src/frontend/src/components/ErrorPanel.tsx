import type { ApiErrorResponse } from '../types/api';

interface Props {
  error: ApiErrorResponse | string | null;
}

export function ErrorPanel({ error }: Props) {
  if (!error) return null;

  if (typeof error === 'string') {
    return (
      <div className="panel panel-error" role="alert">
        <p>{error}</p>
      </div>
    );
  }

  return (
    <div className="panel panel-error" role="alert">
      <p>{error.message}</p>
      {error.details.length > 0 && (
        <ul className="error-details">
          {error.details.map((d, i) => (
            <li key={i}>
              {d.field && <strong>{d.field}: </strong>}
              {d.message}
            </li>
          ))}
        </ul>
      )}
      <p className="meta">
        Error code: <code>{error.errorCode}</code>
        {error.correlationId && (
          <>
            {' '}
            &mdash; Correlation ID: <code>{error.correlationId}</code>
          </>
        )}
      </p>
    </div>
  );
}
