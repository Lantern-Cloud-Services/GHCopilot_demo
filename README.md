<div align="center">

# MTC GitHub Copilot Strategy Briefing & Demo Repository

Accelerated enablement assets and demo code illustrating practical GitHub Copilot usage across editors (VS Code, Visual Studio), modalities (inline, chat, CLI, Agent Mode, MCP), and domains (application code, infrastructure-as-code, scripting, documentation, PMO scenarios).

</div>

---

## Table of Contents
1. Project Name & Description  
2. Technology Stack  
3. Project Architecture  
4. Getting Started  
5. Project Structure  
6. Key Features  
7. Development Workflow  
8. Coding Standards  
9. Testing Approach  
10. Contributing  
11. License & Trademarks  

---

## 1. Project Name & Description
**Name:** MTC GitHub Copilot Strategy Briefing / Demo Assets

**Purpose:** Provide facilitators and engineers with curated demo flows, reference prompts, and lightweight sample applications to illustrate how GitHub Copilot and related capabilities improve productivity across the SDLC: ideation, coding, refactoring, infra generation, documentation, and test authoring.

**Audience:** Executive & technical decision makers (briefing) and technical practitioners (live demos/workshops).

---

## 2. Technology Stack
| Domain | Technology | Notes |
|--------|------------|-------|
| Application (API) | Azure Functions (.NET 6, v4) | Located in `src/demo/basic` |
| Application (UI) | Blazor Server (.NET 7) | Located in `src/demo/visualstudio/BlazorApp` |
| Languages | C#, Markdown, PowerShell, Bicep, Terraform | Infra & scripting are placeholders for prompting |
| Tooling | Azure Functions Core Tools v4 | Required only to run function locally |
| Build | `dotnet` CLI | No custom MSBuild props beyond defaults |
| Dependency Mgmt | NuGet | Single explicit package: `Microsoft.NET.Sdk.Functions` |

Version separation is intentional to show side‑by‑side SDK usage (net6.0 + net7.0).

---

## 3. Project Architecture
Two independent sample apps + documentation/prompt bundles:

```
┌─ Azure Function (stateless HTTP trigger)
│   └─ Demonstrates inline comment prompting, request parsing, response shaping
└─ Blazor Server App
		└─ Standard scaffold + Weather service for completion, chat, test gen demos

Supporting Layers:  
• Infra-as-code stubs (intentionally incomplete) for live generation examples.  
• Prompt blueprints & scenario markdown under `src/demo/prompts` & `.github/prompts`.  
• Copilot agent onboarding instructions in `.github/copilot-instructions.md` (authoritative for automation).  
```

No shared libraries. No cross-project references. CI pipeline intentionally absent (teachable moment for adding one). Infra files contain only “Prompt:” comments and are safe to ignore unless generating examples.

---

## 4. Getting Started
### Prerequisites
- .NET 6 SDK (for Functions) & .NET 7 SDK (for Blazor) installed side-by-side.
- Azure Functions Core Tools v4 (run function locally).  
- Optional: Visual Studio 2022 or VS Code + recommended extensions (`ms-azuretools.vscode-azurefunctions`, `ms-dotnettools.csharp`).

### Quick Start (VS Code Tasks Recommended)
Azure Function:
```powershell
dotnet clean src/demo/basic
dotnet build src/demo/basic
func host start --script-root src/demo/basic/bin/Debug/net6.0
```
Blazor App:
```powershell
dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj
dotnet run --project src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj
```

### Publish (Function)
```powershell
dotnet publish src/demo/basic --configuration Release
```
Output: `src/demo/basic/bin/Release/net6.0/publish`

### Local Endpoint (Function)
`GET http://localhost:7071/api/HttpTrigger1`

> NOTE: `HttpTrigger1.cs` purposefully contains undefined identifiers (`name`, `productID`, etc.) for demo. Replace with parsed values or a static placeholder before relying on it.

---

## 5. Project Structure
```
root
├─ README.md (this file)
├─ basicDemo.md (step-by-step VS Code + Copilot flow)
├─ src/demo/basic (Azure Function)
│  ├─ basic.csproj
│  ├─ HttpTrigger1.cs
│  └─ host.json
├─ src/demo/visualstudio/BlazorApp/BlazorApp (Blazor Server)
│  ├─ Program.cs
│  └─ standard scaffold (Pages, Shared, Data)
├─ src/demo/infra (IaC stubs: bicep, terraform, yaml, policy, ps1)
├─ src/demo/prompts / cli / MCP (prompt & scenario markdown)
├─ .github
│  ├─ copilot-instructions.md (authoritative automation guidance)
│  └─ dependabot.yml (devcontainer updates only)
└─ .vscode (tasks, settings, launch)
```

---

## 6. Key Features
- Executive briefing collateral (agenda, decks, offerings).
- Live demo scripts for:
	- Inline code completions
	- Chat-assisted refactoring & documentation
	- Test generation & explanation
	- Agent Mode / MCP examples
	- Infra-as-code prompt scaffolding
- Multi-environment sample (.NET 6 + .NET 7) for side-by-side Copilot behavior.
- Purposefully flawed function file for showcasing “fix” workflows.

---

## 7. Development Workflow
| Step | Functions | Blazor |
|------|-----------|--------|
| Clean | `dotnet clean src/demo/basic` | Optional (`dotnet clean`) |
| Build | VS Code task `build (functions)` | `dotnet build` |
| Run | VS Code func task (after build) | `dotnet run` |
| Publish | `dotnet publish --configuration Release` | (Add if needed) |

Guidelines:
1. Always run the build task after editing function code (ensures clean first).
2. Keep demo edits minimal & purposeful (show Copilot, not architecture overhauls).
3. Add tests only when illustrating test generation; place under `tests/` (new project) if introduced.

Branching: No enforced strategy; default `main` contains demo assets. Use feature branches for additions and open PRs referencing rationale & any new build steps.

---

## 8. Coding Standards
Minimal by design; rely on default .NET conventions:
- Use explicit `async`/`await` where network or IO operations are added.
- Keep sample methods short & instructional.
- Replace intentionally broken code only when shifting from “demo” to “illustrative fixed state.”
- Avoid adding heavy dependencies—dilutes focus on Copilot scenarios.

For any new code: maintain clarity over cleverness; include a brief XML summary if the method is part of a teaching example.

---

## 9. Testing Approach
Current state: No committed test projects (kept intentionally lean).  
Demonstrated approach (via Copilot Chat / inline):
1. Generate a test class referencing a service or function logic snippet.
2. If persisting: create `tests/<Sample>.Tests/<Sample>.Tests.csproj` referencing target project.
3. Run with `dotnet test` at repo (solution) or test project level.

Edge Cases to Illustrate (suggestions):
- Null / empty payload handling in the function.
- Weather service deterministic seed vs random values for assertion.

---

## 10. Contributing
1. Fork / branch from `main`.
2. Make focused changes (one theme per PR: docs, function improvement, Blazor enhancement, infra example, etc.).
3. Ensure both sample projects still build:  
	 ```powershell
	 dotnet build src/demo/basic
	 dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj
	 ```
4. If you add automation (CI), update `copilot-instructions.md`.
5. Include a short “Demo Impact” note in PR description (what scenario it improves).

CLA & Code of Conduct enforcement handled automatically via Microsoft OSS processes.

---

## 11. License & Trademarks
Licensed under the [MIT License](./LICENSE).  
See also: [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).  
Trademark usage must follow [Microsoft Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).

---

## 12. Additional Demo Walkthrough Links
- [Basic VS Code + Copilot Demo](./basicDemo.md)
- [Visual Studio Blazor Demo](./src/demo/visualstudio/VisualStudioDemo.md)
- [Agent Mode Examples](./src/demo/prompts/agent_mode_examples.md)
- [CLI Demo](./src/demo/cli/Copilot_CLI_Demo.md)
- [MCP WebApp Demo](./src/demo/MCP/mcp_webapp.md)
- [PMO / Operations Prompting](./src/demo/prompts/pmo.md)

---

## 13. Automation Guidance
Automated agents should first consult `.github/copilot-instructions.md` (authoritative). Only perform repo-wide searches if instructions are incomplete or contradictory.

---

Happy demoing and iterating with Copilot!
