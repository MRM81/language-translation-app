import { useRef, useState } from 'react';
import type { ConversationSummary } from '../types/conversation';

interface Props {
  summaries: ConversationSummary[];
  activeId: string | null;
  activeTitle: string;
  loading: boolean;
  onNew: () => void;
  onSwitch: (id: string) => void;
  onRename: (id: string, title: string) => void;
  onDelete: (id: string) => void;
}

export function ConversationManager({
  summaries,
  activeId,
  activeTitle,
  loading,
  onNew,
  onSwitch,
  onRename,
  onDelete,
}: Props) {
  const [isRenaming, setIsRenaming] = useState(false);
  const [renameValue, setRenameValue] = useState('');
  const renameInputRef = useRef<HTMLInputElement>(null);

  function startRename() {
    setRenameValue(activeTitle);
    setIsRenaming(true);
    setTimeout(() => renameInputRef.current?.focus(), 0);
  }

  function commitRename() {
    const trimmed = renameValue.trim();
    if (trimmed && activeId) {
      onRename(activeId, trimmed);
    }
    setIsRenaming(false);
  }

  function cancelRename() {
    setIsRenaming(false);
  }

  function handleRenameKeyDown(e: React.KeyboardEvent<HTMLInputElement>) {
    if (e.key === 'Enter') commitRename();
    if (e.key === 'Escape') cancelRename();
  }

  function handleDelete() {
    if (!activeId) return;
    const title = summaries.find((s) => s.id === activeId)?.title ?? 'this conversation';
    if (window.confirm(`Delete "${title}"? This cannot be undone.`)) {
      onDelete(activeId);
    }
  }

  return (
    <div className="conv-manager-row">
      <div className="conv-manager-selector">
        <span className="conv-manager-label">Conversation</span>
        {isRenaming ? (
          <input
            ref={renameInputRef}
            type="text"
            className="conv-manager-rename-input"
            value={renameValue}
            onChange={(e) => setRenameValue(e.target.value)}
            onKeyDown={handleRenameKeyDown}
            aria-label="Conversation name"
            maxLength={60}
          />
        ) : (
          <select
            className="conv-manager-select"
            value={activeId ?? ''}
            onChange={(e) => onSwitch(e.target.value)}
            disabled={loading}
            aria-label="Select conversation"
          >
            {summaries.map((s) => (
              <option key={s.id} value={s.id}>
                {s.title}{s.messageCount > 0 ? ` (${s.messageCount})` : ''}
              </option>
            ))}
          </select>
        )}
      </div>

      <div className="conv-manager-actions">
        {isRenaming ? (
          <>
            <button
              type="button"
              className="btn-conv-action"
              onClick={commitRename}
            >
              Save
            </button>
            <button
              type="button"
              className="btn-conv-action"
              onClick={cancelRename}
            >
              Cancel
            </button>
          </>
        ) : (
          <>
            <button
              type="button"
              className="btn-conv-action"
              onClick={onNew}
              disabled={loading}
            >
              New
            </button>
            <button
              type="button"
              className="btn-conv-action"
              onClick={startRename}
              disabled={loading || !activeId}
            >
              Rename
            </button>
            <button
              type="button"
              className="btn-conv-action btn-conv-action--destructive"
              onClick={handleDelete}
              disabled={loading || !activeId}
            >
              Delete
            </button>
          </>
        )}
      </div>
    </div>
  );
}
