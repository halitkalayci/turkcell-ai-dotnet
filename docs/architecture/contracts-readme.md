## Contracts Repository Guide

This document governs versioned integration event contracts shared across services.

### Structure
- Namespace root: `TurkcellAI.Contracts`.
- Bounded contexts under folders (e.g., `Orders`), then version folders (`V1`, `V2`, ...).
- Event types implement `IIntegrationEvent` from `TurkcellAI.Core`.

### Versioning Rules
- Backwards-compatible changes (add optional fields) are allowed within the same version.
- Breaking changes require a new version folder (e.g., `V2`) and side-by-side publishing.
- Do not remove or repurpose fields in existing versions.

### Contribution Workflow
- Propose contract changes via PR with rationale and compatibility notes.
- Update consumers before deprecating old versions.
- Keep contracts free of service-internal dependencies.

### Naming Conventions
- Events: verb in past tense (e.g., `OrderCreated`, `OrderStatusChanged`).
- Enums exported in contracts must reflect stable, consumer-friendly values.
