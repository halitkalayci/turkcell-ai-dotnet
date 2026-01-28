## AGENTS.MD - AI-Assisted .NET Microservices Engineering Rules

> Purpose: Establish a single source of truth for how humans + AI collaborate to design and implement a production-grade .NET Microservices.

> Scope: .NET 9, ASP.NET Core, DDD, CQRS

> Non-Goal: This is not a tutorial. 


---

## 0) Operation Mode

### 0.1 Code Generation Policy

- Always plan first, generation second.

- Always propose a file breakdown (what files will be added/changed) and wait for an approval.

### 0.2 Small Batch Rule

- Generate **max of 5 files** if strongly coupled. Do not proceed with next batches without explict approval for current batch.

- Every file MUST include:
    - clear namespace
    - minimal public surface
    - comments only where intent is non-obvious

---

## 1) Architecture SSOT (Mandatory)

### 1.1 Layering (Onion Alignment)

**Domain** (pure)
- Entites, Value Object, Aggregates, Domain Events, Specs
- NO dependencies on Infrastructure, Web, EF Core, Messaging, Logging

**Application**  
- Use cases: Commands/Queries + Handlers
- Ports/Interfaces: repositories, external services
- Validation + policies
- NO EF Core DbContext usage here (only abstractions)

**Infrastructure**  
- EF Core, repositories implementations
- Message bus implementations, outbox publisher, external API clients
- Observability plumbing

**API**  
- Controllers (thin)
- AuthZ/AuthN, request/response models (DTO)
- Contract-first bindings

### 1.2 DDD Invariants
- External world interacts only via **Aggregate Roots**
- State changes only via methods on aggregate (no anemic setters)
- Domain rules must be enforced inside domain model, not only in controllers.

### 1.3 CQRS Rules
- Commands mutate state; Queries never mutate state.
- Write model and Read model may diverge.
- Separate DTOs for read vs write.
- MediatR is default for orchestration (unless explicitly changed).

---

## 2) Contract-First (OpenAPI) - Non-Negotiable

We are using Contract-First development for the entire project. If you are developing against contracts the output is WRONG.

If any contract is missing DO NOT PROCEED, ask.

### 2.1 Contract Rules

- **No extra fields** not in OpenAPI

- All enums, formats, nullable, required constraints must match spec.

- Errors follow a standardized envelope (see 2.2).

### 2.2 Error Model Standard

- Use a consistent error envelope across services:
    - `traceId`
    - `code`
    - `message`
    - `details` (optional)
    - `errors` (field-level validation list)