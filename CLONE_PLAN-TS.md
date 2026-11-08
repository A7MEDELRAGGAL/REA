# CLONE_PLAN-TS.md — Clean-Room TS/Electron Reconstruction Plan

> **Note:** This document is the governing plan for the TypeScript/Electron track (Track B).
> The sibling plans remain authoritative for their own tracks:
> `CLONE_PLAN.md` governs the Muck Unity C# track (Track A).
> `CLONE_PLAN-FPS.md` governs the Combat Master FPS track (Track A2).

**Target Reference:** REA — Reverse Engineering Agent (compiled Electron/TypeScript application, entry point `src/index.ts` → `REACli` class; dependencies: 5 direct + 15 transitive).

**Project Objective:** Reconstruct the full TypeScript/Electron source tree (`src/`, root `package.json`, `tsconfig.json`, `vite.config.ts`, `vitest.config.ts`, `electron-builder.yml`) from decompiled in-memory analysis, persist it to disk, and verify with `npm install`, `tsc --noEmit`, `vitest`, `vite build`, and an Electron packaging smoke test.

**Strict Clean-Room Constraint:**
All reconstruction must be written from decompiled analysis observations only — never imported, never copied, never decompiled-then-pasted verbatim. Class and function logic must be re-expressed in idiomatic TypeScript that matches the original architecture and naming (e.g. `REACli`, `SessionManager`, `ConversationManager`, `TokenCounter`, `ToolRegistry`, `McpClient`, `PermissionManager`, `AuthManager`, `StorageManager`, `EventBus`, `ConfigLoader`, `Logger`, `PromptBuilder`, `SkillManager`, `AgentRunner`) while remaining independently authored code. Original identifiers are reused for public API surface only; internal helper names are free.

---

## 1. Compliance & Original-Only Rules

| Aspect | Boundary |
| --- | --- |
| **Code** | Decompile for logic reference only; rewrite in idiomatic TypeScript. Never import original `.js`/`.ts` bundles. |
| **Assets & Art** | No binary assets to carry over; UI is React components with plain CSS/Tailwind classes. |
| **Audio** | None. |
| **Names & Identifiers** | Public class names, method signatures, hook names, CLI verb names preserved. Internal helpers renamed freely. |
| **Tunables** | Default config values, permission prompts, prompt-builder template strings may be copied as data literals; no algorithmic logic copied. |

---

## 2. Implementation Roadmap

### Phase 0: Source Recovery & Persistence (Week 1)

- [ ] Re-decompile Electron main process: `main.ts`, `preload.ts`
- [ ] Re-decompile all 16 core classes: `REACli`, `SessionManager`, `ConversationManager`, `TokenCounter`, `ToolRegistry`, `Tool`, `McpClient`, `PermissionManager`, `AuthManager`, `StorageManager`, `EventBus`, `ConfigLoader`, `Logger`, `PromptBuilder`, `SkillManager`, `AgentRunner`
- [ ] Re-decompile 8 hooks: `useReact`, `useApprovalRequest`, `useApprovalResponse`, `useApprovalHandler`, `useToolExecution`, `useMessageProcessor`, `useErrorBoundary`, `usePerformance`
- [ ] Re-decompile 47 utility functions and 7 MCP tools
- [ ] Re-decompile UI components: `Chat`, `Sidebar`, `Settings`, `ApprovalPrompt`, `ThinkingIndicator`
- [ ] Re-decompile docs: `README.md`, `USAGE.md`, `INSTALL.md`
- [ ] Persist all decompiled output to root `src/` tree
- [ ] Write root `package.json` (dependencies: 5 direct + 15 transitive; engines: Node >=18)
- [ ] Write `tsconfig.json`, `vite.config.ts`, `vitest.config.ts`, `electron-builder.yml`
- [ ] Verify `WebGame/package.json` is untouched (belongs to `tidebreak` JS web game, not this track)

### Phase 1: Type-Check & Compile (Week 2)

- [ ] Run `npm install`
- [ ] Run `tsc --noEmit` — fix all type errors
- [ ] Confirm entry point resolves: `src/index.ts` → `REACli` class
- [ ] Confirm all 16 core classes compile without error
- [ ] Confirm all 8 hooks compile without error
- [ ] Confirm all 47 utilities and 7 MCP tools compile without error
- [ ] Confirm UI components compile without error

### Phase 2: Test & Verify (Week 3)

- [ ] Run `vitest` — all 8 recovered test suites must pass
- [ ] Fix failing tests until green
- [ ] Run `vite build` — confirm renderer bundle builds
- [ ] Confirm `electron-builder.yml` packaging config is valid
- [ ] Electron packaging smoke test: `electron-builder --dir` produces a runnable artifact

### Phase 3: Documentation & Handoff (Week 4)

- [ ] Confirm `README.md`, `USAGE.md`, `INSTALL.md` are accurate and consistent with reconstructed code
- [ ] Cross-check all public API surface (class names, method signatures, CLI verbs) against decompiled analysis
- [ ] Mark Track B complete; hand off remaining bandwidth to Track A (Muck) and Track A2 (FPS)

---

## 3. Reconstruction Table

| Artifact | Source | Status |
| --- | --- | --- |
| `src/index.ts` | Entry point → `REACli` | Pending Phase 0 |
| `src/core/` (16 classes) | `REACli`, `SessionManager`, `ConversationManager`, `TokenCounter`, `ToolRegistry`, `Tool`, `McpClient`, `PermissionManager`, `AuthManager`, `StorageManager`, `EventBus`, `ConfigLoader`, `Logger`, `PromptBuilder`, `SkillManager`, `AgentRunner` | Pending Phase 0 |
| `src/hooks/` (8 hooks) | `useReact`, `useApprovalRequest`, `useApprovalResponse`, `useApprovalHandler`, `useToolExecution`, `useMessageProcessor`, `useErrorBoundary`, `usePerformance` | Pending Phase 0 |
| `src/utils/` (47 functions) | Utility functions | Pending Phase 0 |
| `src/tools/` (7 MCP tools) | MCP tool implementations | Pending Phase 0 |
| `src/ui/` (5 components) | `Chat`, `Sidebar`, `Settings`, `ApprovalPrompt`, `ThinkingIndicator` | Pending Phase 0 |
| `src/main.ts` | Electron main process | Pending Phase 0 |
| `src/preload.ts` | Electron preload script | Pending Phase 0 |
| `src/tests/` (8 suites) | Recovered test suites | Pending Phase 0 |
| `package.json` | 5 direct + 15 transitive deps | Pending Phase 0 |
| `tsconfig.json` | Recovered config | Pending Phase 0 |
| `vite.config.ts` | Recovered config | Pending Phase 0 |
| `vitest.config.ts` | Recovered config | Pending Phase 0 |
| `electron-builder.yml` | Recovered config | Pending Phase 0 |
| `README.md`, `USAGE.md`, `INSTALL.md` | Recovered docs | Pending Phase 0 |

---

## 4. Evidence & Research Inputs

| File | Contents |
| --- | --- |
| Decompiled in-memory analysis (prior session) | 16 core classes, 8 hooks, 47 utils, 7 MCP tools, main.ts, preload.ts, 5 UI components, 3 docs, 8 test suites, 5 build configs |
| `package.json` (recovered) | 5 direct + 15 transitive dependencies; entry point `src/index.ts` |
| `WebGame/package.json` | `tidebreak` 0.1.0 — JS web game, NOT this track; do not overwrite |

---

## 5. Compliance & Original-Only Rules (Summary)

- Never import original compiled `.js`/`.ts` bundles.
- Never paste decompiled output verbatim into the reconstructed tree.
- Public API surface (class names, method signatures, CLI verbs) preserved.
- Internal helper names free to choose.
- Config data literals (defaults, prompts, templates) may be copied as data; no algorithmic logic copied.
