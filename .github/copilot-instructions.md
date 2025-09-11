## GitHub Copilot Coding Agent Onboarding Instructions

These repo-wide instructions help an automated coding agent quickly understand, modify, build, run, and validate this repository with minimum exploratory searching. Trust these instructions first; only search the codebase when a fact you need is missing or proves incorrect.

---
### 1. Repository Summary
Purpose: Training / demo content for showing GitHub Copilot capabilities across: VS Code, Visual Studio, CLI, MCP, Infra-as-Code, prompts, and a minimal Azure Functions sample plus a Blazor Server app. Not production hardened; several infra/script files are intentionally placeholder “prompt:” stubs used during demonstrations.

Primary Components:
- Azure Functions demo (C#, .NET 6) at `src/demo/basic` (single HTTP trigger function stub `HttpTrigger1.cs`).
- Blazor Server demo app (.NET 7) at `src/demo/visualstudio/BlazorApp/` (standard scaffold with `Program.cs`).
- Infra demo stubs (Bicep, Terraform, policy XML, PowerShell) at `src/demo/infra/` (mostly empty placeholders for Copilot generation demos).
- Prompt & scenario markdown under `src/demo/prompts/` and root level supporting demo docs like `basicDemo.md`, `VisualStudioDemo.md`.

Languages / Runtimes:
- C# (.NET 6 Azure Functions v4, .NET 7 Blazor Server)
- Markdown, YAML, Bicep, Terraform, PowerShell (demo / placeholder)

Repo Size / Complexity: Small; only two real build targets. No custom test projects currently present.

---
### 2. High-Level Build & Run Workflows
There is NO central solution encompassing all demos. The root solution `src/src.sln` references a placeholder `src.csproj` (not shown in demos). Work directly inside each demo folder.

Azure Functions (`src/demo/basic`):
Prerequisites: .NET 6 SDK, Azure Functions Core Tools (v4), Node.js not required for current code, PowerShell/VS Code tasks available.
Always run a clean before rebuild to ensure deterministic demo outputs.
Canonical local workflow:
1. Clean: `dotnet clean` (or VS Code task: `clean (functions)`).
2. Build: `dotnet build` (task `build (functions)` depends on clean).
3. Run locally (after successful build): use Functions host from compiled output: `func host start` in `src/demo/basic/bin/Debug/net6.0` OR invoke VS Code task with type `func` (depends on build). The supplied tasks already orchestrate this; prefer tasks to manual commands.
4. (Optional publish) Release build + publish: `dotnet publish --configuration Release` (task `publish (functions)` depends on `clean release (functions)`). The publish output path used by VS Code deploy settings: `src\demo\basic/bin/Release/net6.0/publish`.

Blazor Server (`src/demo/visualstudio/BlazorApp/BlazorApp`):
Prerequisites: .NET 7 SDK.
Workflow:
1. From project folder: `dotnet build`.
2. Run: `dotnet run` (serves on a Kestrel port, typically shown in console). No extra config required.

Environment / Tool Versions (assumed minimal set):
- .NET 6.x SDK for Functions sample.
- .NET 7.x SDK for Blazor sample.
- Azure Functions Core Tools v4 (to run `func host start`).

No tests exist; if adding tests, place them in a new test project and ensure `dotnet test` passes before submitting PRs.

Known Issues / Placeholders:
- `HttpTrigger1.cs` references undeclared variables (`name`, `productID`, `quantity`, `orderID`). This is intentional for demo (to show Copilot fixing). Any productionizing task should add parsing logic or remove those references. Builds currently succeed because variables are only interpolated at runtime? (Actually will not compile: undefined identifiers). BEFORE shipping a PR that touches this file, define or remove them. A safe placeholder response: `return new OkObjectResult("Hello from HttpTrigger1.");`
- Infra files (`deploy.bicep`, `main.tf`, `deploy.yaml`, `policy.xml`, `script.ps1`) are comments only. Do not assume they provision resources without being populated.

---
### 3. VS Code Tasks & Settings (Preferred for Functions)
Defined in `.vscode/tasks.json`:
- `clean (functions)` → dotnet clean in `src/demo/basic`
- `build (functions)` → depends on clean; builds function.
- `clean release (functions)` / `publish (functions)` → release pipeline.
- Functions host task (type `func`) → runs after build from output folder.
Settings in `.vscode/settings.json` configure deploy subpath and preDeploy task.
Always prefer invoking the defined tasks instead of ad-hoc commands to avoid path mistakes.

Launch config: `.vscode/launch.json` offers attach to running Functions process.

---
### 4. Standard Sequences (Use As-Is)
Local Function run (recommended minimal):
1. Run task: `build (functions)` (auto cleans) → success expected quickly (<10s small project).
2. Run Functions host task (auto) or manually: `func host start` from output folder.

Local edit iteration:
1. Modify `HttpTrigger1.cs`.
2. Re-run `build (functions)` task (auto cleans) to surface compile errors early.
3. Restart Functions host if signature changed.

Blazor run sequence:
1. `dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj`.
2. `dotnet run --project src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj`.

Publishing Function (dry-run packaging):
1. Run `publish (functions)` task (triggers clean release + publish).
2. Output appears under `src/demo/basic/bin/Release/net6.0/publish`.

---
### 5. Validation Guidance
Because no automated tests or CI workflows (besides Dependabot) exist, validation is manual:
Checklist before opening a PR that changes code:
- Does `dotnet build` succeed for each modified project? (Functions: .NET 6, Blazor: .NET 7)
- If editing `HttpTrigger1.cs`, ensure compilation passes (add missing variables or remove them).
- (Optional) Run Functions host and invoke endpoint: `GET http://localhost:7071/api/HttpTrigger1` expecting 200 OK.
- For Blazor changes, confirm Kestrel launches and serves root page without runtime exceptions.

Add tests when implementing non-trivial logic; place them in `tests/` (create if absent) and update instructions if added.

---
### 6. Project Layout & Navigation Map
Root Important Files:
- `README.md` (demo overview & links)
- `basicDemo.md` (walkthrough of Functions + Copilot usage)
- `.vscode/` (tasks, settings controlling build/run)
- `.github/dependabot.yml` (weekly devcontainer update only) – no build workflow currently.
- `src/src.sln` (placeholder solution; not central to demos).

Key Source Paths:
- Azure Function: `src/demo/basic/HttpTrigger1.cs`, config `host.json`, project `basic.csproj`.
- Blazor App: `src/demo/visualstudio/BlazorApp/BlazorApp/Program.cs` + standard directories (`Pages`, `Shared`, etc.).
- Infra stubs: `src/demo/infra/*` (safe to ignore unless populating IaC examples).
- Prompts & docs: `src/demo/prompts/*.md`, `src/demo/cli/*.md`, `src/demo/MCP/*.md`.

Config / Tooling:
- No custom analyzers or lint rules specified; rely on default .NET compiler diagnostics.
- No NuGet package complexity: only `Microsoft.NET.Sdk.Functions` for the Functions project.

Dependencies (implicit):
- Azure Functions Core Tools required ONLY to run the function locally; build alone uses `dotnet`.
- Running Blazor requires .NET 7 runtime; mixing SDK versions is acceptable (side-by-side installs).

---
### 7. Common Pitfalls & Mitigations
- Undefined variables in `HttpTrigger1.cs`: Fix before committing if task is to “improve” or “add feature.”
- Forgetting clean can rarely matter here (small project) but ALWAYS follow task chain for consistency.
- Deploy attempts will fail if `deploy.yaml` (GitHub Actions) remains placeholder; create a real workflow before relying on automation.
- Infra code is non-functional; don’t spend cycles debugging empty stubs—populate or skip.

---
### 8. When Extending
- New Functions: add additional .cs files in `src/demo/basic`; update bindings via attributes; no central registration needed.
- Add tests: create `tests/<ProjectName>.Tests/` with a `.csproj` referencing target project; then run `dotnet test`.
- Add CI: create workflow under `.github/workflows/` running `dotnet build` (and `dotnet test` if tests added) for both frameworks.

---
### 9. Agent Operating Principles
ALWAYS:
1. Use provided tasks for the Functions project instead of crafting new commands unless new functionality requires it.
2. Ensure `HttpTrigger1.cs` compiles before pushing changes (define required variables or simplify response).
3. Document any new build/test commands you introduce inside PR description.
4. Prefer minimal, focused edits; don’t refactor demo code unless explicitly asked.

ONLY search the repo if: (a) you need a file not referenced here, or (b) a command fails contrary to these instructions.

If you create new workflows, update this file in the same PR if instructions become stale.

---
### 10. Quick Reference (Copy/Paste)
Functions build & run:
	dotnet clean src/demo/basic
	dotnet build src/demo/basic
	(cd src/demo/basic/bin/Debug/net6.0 && func host start)

Blazor build & run:
	dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj
	dotnet run --project src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj

Publish Functions (Release):
	dotnet publish src/demo/basic --configuration Release

---
End of instructions.
