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


---

## 3) Shared Kernel (TurkcellAI.Core) - Mandatory

TurkcellAI.Core is a shared class library that standardizes foundational building blocks across all microservices. It enforces the Layering and DDD/CQRS rules defined in sections 1.1, 1.2 and 1.3, and the error envelope in section 2.2.

### 3.1 Core Library Scope

Namespaces: `TurkcellAI.Core.Domain`, `TurkcellAI.Core.Application`, `TurkcellAI.Core.Infrastructure`.

- Domain building blocks (pure):
    - `Entity<TId>` base
    - `AggregateRoot` marker
    - `ValueObject` base
- Application contracts:
    - `IUnitOfWork` abstraction
- Error handling:
    - `ErrorResponse` and `ValidationError` DTOs
    - Base `ErrorCode` enum with common codes (e.g., `VALIDATION_ERROR`, `INVALID_PARAMETER`, `NOT_FOUND`, `CONFLICT`, `UNAUTHORIZED`, `FORBIDDEN`, `INTERNAL_ERROR`)
- Cross-cutting behaviors:
    - `ValidationBehavior` (FluentValidation pipeline)
    - `TransactionBehavior` (wraps command handlers in unit-of-work transactions)
- Middleware:
    - `ExceptionHandlingMiddleware` (maps exceptions to standardized error envelope)

### 3.2 Dependency Rules

- Core must remain pure and reusable; NO service-specific domain logic.
- NO dependencies on EF Core, messaging, web frameworks inside `TurkcellAI.Core.Domain` and `TurkcellAI.Core.Application`.
- Implementations that require web hosting integrations (e.g., middleware) live under `TurkcellAI.Core.Infrastructure` and keep minimal dependencies.
- Aligns with Layering (1.1): Domain is pure, Application holds abstractions, Infrastructure provides implementations.

### 3.3 ErrorCode Extensibility

- Core defines base error codes shared across services.
- Services MAY extend with service-specific codes in their own `Domain/Enums/[ServiceName]ErrorCode.cs` (e.g., `ORDER_NOT_FOUND`, `INVALID_STATUS_TRANSITION`).
- All services MUST use an enum type for the error envelope `code` field (`ErrorCode` plus service-specific enum where applicable).
- Service-specific codes MUST NOT conflict with Core base codes.

### 3.4 Service Integration

- All microservices MUST reference `TurkcellAI.Core`.
- NEW services MUST start with Core from day one.
- Domain entities/aggregates/value objects MUST derive from Core base classes.
- Error responses MUST use Core `ErrorResponse` envelope and enum-based `code`.
- Where applicable, services SHOULD adopt Core MediatR behaviors and middleware.

### 3.5 Migration Strategy (Incremental)

Incremental migration in two phases: OrderService first, ProductService second.

Phase 1 — OrderService
- Create `TurkcellAI.Core` class library with the scope in 3.1.
- Extract Domain building blocks (Aggregate, Entity, ValueObject) from OrderService into `TurkcellAI.Core.Domain`.
- Move `ErrorResponse` and `ValidationError` DTOs into `TurkcellAI.Core.Application`.
- Define base `ErrorCode` in Core; keep OrderService-specific codes in `OrderService.Domain.Enums`.
- Move `ValidationBehavior` and `TransactionBehavior` into `TurkcellAI.Core.Application`.
- Move `ExceptionHandlingMiddleware` into `TurkcellAI.Core.Infrastructure`.
- Reference Core from OrderService; update namespaces and imports accordingly.

Phase 2 — ProductService
- Reference `TurkcellAI.Core`.
- Replace service-specific base entity with Core `Entity<Guid>`; adopt Core `AggregateRoot`/`ValueObject` where relevant.
- Adopt Core `ErrorResponse` and enum-based `ErrorCode`.
- Create `ProductErrorCode` in `ProductService.Domain.Enums` for service-specific codes (e.g., `PRODUCT_NOT_FOUND`).
- Replace local exception middleware with Core `ExceptionHandlingMiddleware`.
- Adopt Core `ValidationBehavior` and `TransactionBehavior` where applicable.




--- 

## 4) COMMUNICATION

For any communication rules you MUST follow [01-messaging-governance.md](docs/architecture/01-messaging-governance.md)