# Scripts

Utility scripts for the project live here.

---

## Architect Pack Importer

Use apply-architect-pack.js to import a single Architect Pack Markdown file into project-relative files.

Dry run first:

node scripts/apply-architect-pack.js path/to/architect-pack.md --dry-run

Apply after review:

node scripts/apply-architect-pack.js path/to/architect-pack.md

The importer refuses:

- absolute paths
- paths outside the project root
- duplicate FILE sections
- protected starter files