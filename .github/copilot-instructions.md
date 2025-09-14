## Copilot Coding Agent Onboarding (Authoritative)

Trust this document first. ONLY search the repo if (a) a command here fails or (b) you need a file not referenced below.

---
### 1. Overview & Stack
Purpose: Demo / briefing assets showcasing GitHub Copilot across editors (VS Code, Visual Studio), modes (inline, chat, CLI, Agent Mode, MCP) and domains (app code, infra prompts, docs). Not production-hardened; several files are intentional placeholders for live generation.

Active Build Targets:
| Component | Path | Framework | Notes |
|-----------|------|-----------|-------|
| Azure Function (HTTP trigger) | `src/demo/basic` | net6.0 / Functions v4 | Single `HttpTrigger1.cs` stub (intentionally broken variables) |
| Blazor Server App | `src/demo/visualstudio/BlazorApp/BlazorApp` | net7.0 | Standard template + Weather service |

Supporting (Non-Build) Assets: `src/demo/infra/*` (placeholders), prompt markdown (`src/demo/prompts`, `.github/prompts`), decks in `resources/`.

Required Tooling:
- .NET 6 SDK (Functions) + .NET 7 SDK (Blazor) side‑by‑side
- Azure Functions Core Tools v4 (run host only)
- VS Code tasks (preferred) or plain `dotnet` CLI

No tests or CI workflows presently (only Dependabot).

---
### 2. Quick Start Matrix
| Action | Function App | Blazor App |
|--------|--------------|------------|
| Clean + Build | `dotnet clean src/demo/basic && dotnet build src/demo/basic` (or task: `build (functions)` auto-cleans) | `dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj` |
| Run | From output: `(cd src/demo/basic/bin/Debug/net6.0 && func host start)` OR use func task | `dotnet run --project src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj` |
| Publish | `dotnet publish src/demo/basic --configuration Release` | (None; add if needed) |
| Endpoint | `GET http://localhost:7071/api/HttpTrigger1` | Kestrel port printed to console |

Always clean before building the Function (task already enforces this). Blazor clean is optional unless resolving stale build issues.

---
### 3. Detailed Build / Run
Function (authoritative sequence):
1. `dotnet clean src/demo/basic`
2. `dotnet build src/demo/basic`
3. Run host: `func host start --script-root src/demo/basic/bin/Debug/net6.0`
4. Smoke: curl/GET endpoint → expect 200 + body text.

Blazor:
1. `dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj`
2. `dotnet run --project src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj`
3. Load root page → no unhandled exceptions.

Release publish (Function):
`dotnet publish src/demo/basic --configuration Release` → artifacts in `src/demo/basic/bin/Release/net6.0/publish`.

---
### 4. Validation & Quality Gates (Manual)
Run BEFORE opening a PR that changes code:
- Build both projects (see sequences above)
- If touching `HttpTrigger1.cs`: ensure it compiles (replace undefined vars OR simplify to: `return new OkObjectResult("Hello from HttpTrigger1.");`)
- Optional HTTP smoke test (Function endpoint)
- Optional manual UI smoke (Blazor root page)

If you introduce tests:
1. Place test project under `tests/<Name>.Tests` referencing target project.
2. Add to instructions + run `dotnet test` (must pass) before PR.

Planned CI: Add a workflow under `.github/workflows/` then update this file (new “CI” subsection) in the SAME PR.

---
### 5. Layout Map (Paths You May Need)
| Category | Path(s) | Notes |
|----------|---------|-------|
| Function Code | `src/demo/basic/HttpTrigger1.cs`, `basic.csproj`, `host.json` | Single trigger |
| Blazor Code | `src/demo/visualstudio/BlazorApp/BlazorApp/Program.cs` + Pages/Shared/Data | Scaffold unchanged |
| Infra Placeholders | `src/demo/infra/*` | Only comments; safe to ignore unless generating |
| Prompts & Scenarios | `src/demo/prompts/*.md`, `src/demo/cli/*.md`, `src/demo/MCP/*.md` | Demo narration |
| Agent Guidance | `.github/copilot-instructions.md` | This file |
| VS Code Tasks | `.vscode/tasks.json` | Preferred execution for Functions |

No hidden build scripts, custom analyzers, or extra props files beyond defaults.

---
### 6. Pitfalls & Resolutions
| Pitfall | Cause | Resolution |
|---------|-------|------------|
| Compile error in `HttpTrigger1.cs` | Undefined identifiers (`name`, `productID`, etc.) | Replace with parsed request values or static placeholder response |
| Host fails: func not found | Missing Functions Core Tools | Install Core Tools v4; re-run host step |
| Wrong .NET SDK picked | PATH precedence | Explicitly run `dotnet --list-sdks`; install required versions |
| Infra confusion | Placeholder files | Ignore unless tasked with generating real IaC |

Add new entries only if reproducible & recurring.

---
### 7. Extending
- New Function: add .cs file (attribute `[FunctionName]`), update instructions ONLY if build/run sequence changes.
- New App/Service: document minimal build/run + validation in sections 2–4; keep table concise.
- Tests: keep fast; name project `<Component>.Tests`; update Validation section.
- CI: add workflow + short description (triggers, jobs) here.

Avoid: large refactors, dependency bloat, stylistic mass changes—this is a teaching repo.

---
### 8. Agent Operating Rules
ALWAYS:
1. Use VS Code tasks for Functions when available (`build (functions)` / func host) before crafting raw commands.
2. Keep edits minimal & scoped; explain reason in PR body.
3. Update THIS file if you change build, paths, or validation.
4. Provide a short “Validation Run” section in PR: which projects built & any smoke test results.

ONLY search the repo if a referenced path/command fails or a needed file is not listed here.

NEVER invent new tooling or restructure folders unless explicitly requested.

---
### 9. Quick Reference (Copy/Paste)
Functions (Debug):
```
dotnet clean src/demo/basic
dotnet build src/demo/basic
(cd src/demo/basic/bin/Debug/net6.0 && func host start)
```
Blazor:
```
dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj
dotnet run --project src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj
```
Publish (Function Release):
```
dotnet publish src/demo/basic --configuration Release
```

---
End of authoritative instructions.
