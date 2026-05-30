# Architect Pack Workflow

## Purpose

Architect Packs are structured sprint handoff bundles created by the Architect layer and consumed by the Builder layer.

The Architect Pack defines:

- Scope
- Constraints
- Acceptance criteria
- Risks
- Questions
- Implementation blueprint
- Builder instructions

---

## Workflow

1. Architect gathers discovery information.
2. Architect defines sprint scope.
3. Architect generates:
   - requirements
   - blueprint
   - acceptance
   - handoff-prompt
4. Builder performs dry run.
5. Builder summarizes understanding.
6. Architect validates understanding.
7. Builder executes implementation.
8. Builder updates project state files.

---

## Rules

- One sprint = one Builder chat.
- Keep context isolated.
- Do not mix sprint goals.
- Use durable files as source of truth.
- Avoid relying on chat history.