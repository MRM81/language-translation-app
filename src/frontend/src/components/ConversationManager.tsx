import { useRef, useState } from 'react';
import type { ConversationSummary } from '../types/conversation';

interface Props {
  summaries: ConversationSummary[];       // pre-filtered by parent
  allCount: number;                        // total before filtering (for no-results state)
  activeId: string | null;
  activeTitle: string;
  loading: boolean;
  searchQuery: string;
  onSearchChange: (query: string) => void;
  onNew: () => void;
  onSwitch: (id: string) => void;
  onRename: (id: string, title: string) => void;
  onDelete: (id: string) => void;
}

function truncate(text: string, max: number): string {
  return text.length > max ? text.slice(0, max).trimEnd() + '…' : text;
}

function buildOptionLabel(s: ConversationSummary): string {
  const count = s.messageCount > 0 ? ` (${s.messageCount})` : '';
  const preview = s.previewText ? ` · ${truncate(s.previewText, 35)}` : '';
  return `${s.title}${count}${preview}`;
}

export function ConversationManager({
  summaries,
  allCount,
  activeId,
  activeTitle,
  loading,
  searchQuery,
  onSearchChange,
  onNew,
  onSwitch,
  onRename,
  onDelete,
}: Props) {
  const [isRenaming, setIsRenaming] = useState(false);
  const [renameValue, setRenameValue] = useState('');
  const renameInputRef = useRef<HTMLInputElement>(null);
  const searchInputRef = useRef<HTMLInputElement>(null);

  const showSearch = allCount > 1 || searchQuery.length > 0;
  const noResults = searchQuery.trim() !== '' && summaries.length === 0 && allCount > 0;

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
    const title = summaries.find((s) => s.id === activeId)?.title ?? activeTitle;
    if (window.confirm(`Delete "${title}"? This cannot be undone.`)) {
      onDelete(activeId);
    }
  }

  function clearSearch() {
    onSearchChange('');
    searchInputRef.current?.focus();
  }

  return (
    <div className="conv-manager-row">
      {showSearch && (
        <div className="conv-search-wrap">
          <input
            ref={searchInputRef}
            type="search"
            className="conv-search-input"
            placeholder="Search conversations…"
            value={searchQuery}
            onChange={(e) => onSearchChange(e.target.value)}
            aria-label="Search conversations"
          />
          {searchQuery && (
            <button
              type="button"
              className="conv-search-clear"
              onClick={clearSearch}
              aria-label="Clear search"
            >
              ×
            </button>
          )}
        </div>
      )}

      {noResults ? (
        <p className="conv-empty-search" role="status">
          No conversations match your search.{' '}
          <button type="button" className="conv-search-reset-link" onClick={clearSearch}>
            Clear search
          </button>
        </p>
      ) : (
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
                  {buildOptionLabel(s)}
                </option>
              ))}
            </select>
          )}
        </div>
      )}

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
