---
title: Copilot Demo Architecture & Operational Specification
version: 1.0
date_created: 2025-09-14
last_updated: 2025-09-14
owner: Demo Maintainers
tags: [architecture, process, demo, copilot, dotnet, azure-functions, blazor]
---

# Introduction

This specification defines the architecture, operational requirements, quality expectations, and extension guidelines for the GitHub Copilot Strategy Briefing / Demo repository. It ensures contributors and automated agents can evolve the repository in a consistent, verifiable, and low-risk manner while preserving its instructional intent.

## 1. Purpose & Scope

Purpose: Provide a structured reference for current and planned demo components (Azure Function, Blazor Server app, prompt assets, infrastructure placeholders) and define rules for adding tests, CI, deployment artifacts, and educational extensions.

Scope Includes:
- Runtime architecture of existing sample applications.
- Build, validation, and extension requirements.
- Naming, directory, and component interaction conventions.
- Acceptance and validation criteria for future enhancements.

Out of Scope:
- Production security hardening beyond minimal demo hygiene.
- Performance tuning guidelines for large-scale traffic.
- Enterprise-grade deployment automation.

Audience: Maintainers, contributors, and automated coding agents generating or modifying code, documentation, tests, or infrastructure for the repository.

Assumptions:
- Contributors have .NET 6 & .NET 7 SDKs available.
- Azure Functions Core Tools v4 installed for local function hosting.
- Repository intentionally minimalist to highlight Copilot capabilities rather than full-stack engineering maturity.

## 2. Definitions

| Term | Definition |
|------|------------|
| Azure Function | Serverless compute unit (HTTP-triggered) using Azure Functions v4 and .NET 6. |
| Blazor Server | ASP.NET Core server-side UI framework (.NET 7). |
| IaC | Infrastructure as Code (Bicep, Terraform) placeholder artifacts for demo prompt usage. |
| MCP | Model Context Protocol; used for structured tool integration scenarios in demos. |
| Prompt Asset | Markdown or instruction file used to drive Copilot behavior or showcase prompt engineering. |
| Validation Run | Manual or automated sequence verifying build + (optional) smoke execution before PR submission. |
| Smoke Test | Minimal runtime check (HTTP 200 from Function, Blazor root loads). |
| Placeholder File | File containing comments or “Prompt:” lines only; not functionally executable until implemented. |
| Agent | Automated coding assistant executing edits under guidance of `.github/copilot-instructions.md`. |

## 3. Requirements, Constraints & Guidelines

Functional Requirements:
- **REQ-001**: The repository SHALL provide at least one HTTP-trigger Azure Function sample.
- **REQ-002**: The repository SHALL provide at least one Blazor Server sample application.
- **REQ-003**: Each buildable component SHALL build independently via `dotnet build` without additional scripts.
- **REQ-004**: A Quick Start command set SHALL be maintained in `.github/copilot-instructions.md`.
- **REQ-005**: Placeholder infra files SHALL remain non-executable until explicitly implemented and reclassified.

Quality & Validation:
- **VAL-001**: A validation checklist SHALL be executed (build both components) before merging changes to main.
- **VAL-002**: If a test project is added, `dotnet test` MUST pass with zero failed tests for PR acceptance.
- **VAL-003**: Function endpoint smoke test SHOULD return HTTP 200 when code touches function logic.

Extensibility:
- **EXT-001**: New demo components MUST document build + run steps in instructions in the same PR.
- **EXT-002**: Shared DTOs (if introduced) MUST reside in a new `src/shared` folder to avoid circular references.

Security & Hygiene:
- **SEC-001**: No secrets SHALL be committed (local.settings.json excluded by default patterns).
- **SEC-002**: Demo code SHALL avoid dynamic code execution patterns not needed for instruction.

Documentation:
- **DOC-001**: README MUST link to new major demo assets within one PR of their addition.
- **DOC-002**: `.github/copilot-instructions.md` MUST remain ≤ 2 pages (enforced manually).

Constraints:
- **CON-001**: No external build system (e.g., Make, Nuke) SHALL be required.
- **CON-002**: Dependencies beyond `Microsoft.NET.Sdk.Functions` MUST justify teaching value.
- **CON-003**: Infrastructure artifacts SHALL remain isolated (no accidental deployment side-effects) unless a CI deployment phase is deliberately introduced.

Guidelines:
- **GUD-001**: Prefer minimal, readable examples over feature-complete implementations.
- **GUD-002**: Mark intentional breakage or incompleteness with a top-of-file comment: `// INTENTIONAL: <rationale>`.
- **GUD-003**: Use explicit method summaries in new demo code blocks for clarity.

Patterns:
- **PAT-001**: Build pattern: clean → build → (optional) run → (optional) publish.
- **PAT-002**: Validation pattern: build all → smoke test changed runtime component(s) → document result.

## 4. Interfaces & Data Contracts

Current Data Contracts: None formalized (demo stubs). Future enhancements may add simple DTOs.

Suggested DTO (example) for future Order processing demonstration:
```csharp
public sealed record OrderRequest(string ProductId, string OrderId, int Quantity);
public sealed record OrderResponse(string OrderId, string ProductId, int Quantity, string Status, DateTime UtcProcessedAt);
```

Function Invocation Interface (planned refinement):
| Aspect | Current | Target (Recommended) |
|--------|---------|----------------------|
| Input Source | Unparsed request + undefined vars | Parse query + JSON body into `OrderRequest` |
| Response | String interpolation with undefined symbols | JSON serialized `OrderResponse` |
| Error Handling | None | 400 for validation errors, 500 for unexpected exceptions |

## 5. Acceptance Criteria

- **AC-001** (Core Build): Given a clean repository, when `dotnet build` is run on each active project, then the build completes successfully without errors.
- **AC-002** (Function Smoke): Given the function project is built, when the local host runs and `/api/HttpTrigger1` is invoked, then a 200 response is returned.
- **AC-003** (Instruction Sync): Given a new component is added, when the PR is opened, then `.github/copilot-instructions.md` reflects its build & run steps.
- **AC-004** (Readability): Given code generation adds a new sample class, when reviewed, then it contains comments or summaries clarifying demo intent.
- **AC-005** (Placeholder Clarity): Given a placeholder file remains unimplemented, when opened, then it clearly communicates its instructional purpose.

## 6. Test Automation Strategy

- **Test Levels**: (Future) Unit tests for parsing & service logic; optional component tests (Blazor) using bUnit; integration (Function invocation) via HTTP in-memory host or local process.
- **Frameworks**: MSTest or xUnit (xUnit recommended for ecosystem familiarity), FluentAssertions for expressive checks, Moq (if interfaces introduced).
- **Test Data Management**: Use inline object builders; avoid external fixtures.
- **CI/CD Integration**: Add `ci.yml` workflow: steps = checkout → setup .NET (6 & 7) → restore → build → test.
- **Coverage Requirements**: Initial target 60% for any new logic project; not enforced on placeholder-only components.
- **Performance Testing**: Optional script-driven k6 or Bombardier run documented in a separate `perf.md` if introduced.

## 7. Rationale & Context

Minimalism preserves clarity for Copilot demonstrations. Undefined variables in the function illustrate “debug/fix with Copilot” flows. Separation of .NET versions highlights multi-target environment reasoning. Placeholder IaC enables live prompting rather than static pre-baked infrastructure scripts, reinforcing generative co-creation.

## 8. Dependencies & External Integrations

### External Systems
- **EXT-001**: (Optional/Future) Azure Storage (for queue or table demo) – not presently provisioned.

### Third-Party Services
- **SVC-001**: (Future) Application Insights for telemetry if added to illustrate observability scaffolding.

### Infrastructure Dependencies
- **INF-001**: Azure Functions Core Tools – required only for local function runtime host.

### Data Dependencies
- **DAT-001**: None currently. Future Order examples may simulate persistence using in-memory collections.

### Technology Platform Dependencies
- **PLT-001**: .NET 6 SDK – Functions build compatibility.
- **PLT-002**: .NET 7 SDK – Blazor Server runtime.

### Compliance Dependencies
- **COM-001**: MIT License must remain intact in derivative code.

## 9. Examples & Edge Cases

```csharp
// Example: Future improved Function handler (illustrative)
[FunctionName("HttpTrigger1")]
public static IActionResult Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    ILogger log)
{
    // Parse body
    using var reader = new StreamReader(req.Body);
    var json = reader.ReadToEnd();
    if (string.IsNullOrWhiteSpace(json))
        return new BadRequestObjectResult(new { error = "Body required" });

    var dto = JsonConvert.DeserializeObject<OrderRequest>(json);
    if (dto is null || string.IsNullOrWhiteSpace(dto.ProductId))
        return new BadRequestObjectResult(new { error = "Invalid order payload" });

    var response = new OrderResponse(
        dto.OrderId ?? Guid.NewGuid().ToString(),
        dto.ProductId,
        dto.Quantity,
        Status: "Accepted",
        UtcProcessedAt: DateTime.UtcNow);

    return new OkObjectResult(response);
}
```

Edge Cases (anticipated):
- Empty request body → 400 Bad Request.
- Negative quantity (if later validated) → 422 Unprocessable Entity (future enhancement).
- Malformed JSON → 400 with parse error descriptor.

## 10. Validation Criteria

| ID | Validation Step | Tool/Command | Expected Result |
|----|-----------------|--------------|-----------------|
| V-001 | Build Function | `dotnet build src/demo/basic` | Success, no errors |
| V-002 | Build Blazor | `dotnet build src/demo/visualstudio/BlazorApp/BlazorApp/BlazorApp.csproj` | Success |
| V-003 | Run Function Host | `func host start` (after build) | Host listening on 7071 |
| V-004 | Invoke Endpoint | HTTP GET `/api/HttpTrigger1` | 200 (after placeholder fix) |
| V-005 | Instructions Sync | Compare diff | Updated when structural changes occur |

## 11. Related Specifications / Further Reading

- `.github/copilot-instructions.md` – Operational quick guide (authoritative runtime instructions)
- `README.md` – High-level overview and navigation
- `basicDemo.md` – Function + Copilot step demo
- `src/demo/visualstudio/VisualStudioDemo.md` – Blazor + Visual Studio Copilot demo

---
End of specification.
