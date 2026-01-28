## Messaging Governance (RabbitMQ + MassTransit)

This document defines mandatory conventions and reliability policies for asynchronous messaging across all TurkcellAI microservices. It aligns with the architecture SSOT and Core layering rules.

### Technology Choice
- Preferred: MassTransit with RabbitMQ.
- Alternatives: NServiceBus (commercial, heavier), Rebus (lighter, fewer built-ins), raw RabbitMQ.Client (DIY, higher risk).
- Rationale: MassTransit offers first-class RabbitMQ support, EF Core Outbox/Inbox, retries/DLQ, topology management, observability hooks, and broad adoption.

### Topology Conventions
- Exchanges (topic): `ex.tai.{boundedContext}` → e.g., `ex.tai.orders`.
- Dead-letter exchange: `ex.tai.dlx`.
- Queues: `q.{service}.{message}.{v}.{purpose}` (+ `.dlq` for dead-letter; optional `.park` for parked).
	- Example: `q.product.order-created.v1.handler`, `q.product.order-created.v1.handler.dlq`.
- Routing keys: `{context}.{v}.{kind}.{name}` → e.g., `orders.v1.event.order-created`.
- Ownership: Producers own exchanges for their bounded context; consumers own their queues.
- Environments: suffix or vhost separation as needed (e.g., `/dev`, `/stg`, `/prod`).

### Message Contract & Envelope
- Integration events must implement `IIntegrationEvent` (Core) and be versioned (e.g., `Orders.V1.OrderCreated`).
- Mandatory headers:
	- `messageId`, `correlationId`, `causationId`, `traceId` (W3C traceparent), `type`, `version`, `source`, `occurredAt`, `tenantId` (if multi-tenant).
- Envelope abstraction (`IMessageEnvelope<T>` in Core) standardizes header access and payload.
- Versioning policy: never break existing consumers. New fields are optional; breaking changes require a new event version and side-by-side publishing.

### Reliability Policies (Mandatory)
- Transactional Outbox (producer): persist event and state changes in the same DB transaction; publish via outbox dispatcher.
- Idempotency (consumer): inbox/outbox persistence per message (`messageId` + `handlerKey`) to deduplicate and ensure exactly-once handler effects.
- Retries: maximum 5 attempts, exponential backoff; after max, route to DLQ.
- DLQ: all failed messages are dead-lettered to `.dlq` queues via `ex.tai.dlx` with failure details in headers.
- Poison/park: optional `.park` queues for manual inspection of permanently problematic messages.

### Configuration Schema (Options-only)
All settings are provided via configuration and bound through `IOptions<MessagingOptions>`; no static configuration in code.

```json
{
	"Messaging": {
		"Enabled": true,
		"Host": "rabbitmq.local",
		"VirtualHost": "/",
		"Username": "app",
		"Password": "secret",
		"Prefetch": 32,
		"PublisherConfirmEnabled": true,
		"OutboxEnabled": true,
		"InboxEnabled": true,
		"Retry": { "MaxAttempts": 5, "MinBackoff": "00:00:01", "MaxBackoff": "00:00:30", "IntervalFactor": 2.0 },
		"DeadLetter": { "Exchange": "ex.tai.dlx", "Suffix": ".dlq", "EnableParkQueue": true, "ParkSuffix": ".park" }
	}
}
```

### Observability & Tracing
- Propagate `traceId`/`correlationId` from HTTP to messaging and across services.
- Emit metrics: publish/consume counts, retries, DLQ counts, outbox backlog size.
- Log message types, versions, and handler outcomes consistently.

### Security & Environments
- Disable auto-create topology in production; pre-provision exchanges/queues with IaC.
- Use least-privilege RabbitMQ users per service and environment.
- Protect credentials via secret management; never hard-code.

### Ownership & Lifecycle
- Bounded contexts own their exchanges and event contracts.
- Consumers own queues and retry/DLQ configuration.
- Contract changes follow semantic versioning with separate `TurkcellAI.Contracts` project; PR review is required.

### Rollout Strategy
- Phase-in per service: producers enable outbox and publish integration events; consumers adopt idempotent handlers with configured retries/DLQ.
- Validate with integration tests and synthetic messages before production enablement.

