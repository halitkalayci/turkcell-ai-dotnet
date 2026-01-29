# Claims Consistency

This document standardizes claim and policy naming across services.

## Claim Names
- `permissions` (array of strings): the single source of permission values issued by Keycloak via protocol mapper.
- Subject identifiers: `sub` (user id) or `client_id`/`azp` for client credentials.

## Permission Values
- Convention: `service:resource:operation`
  - Orders: `orders:order:create`, `orders:order:read`, `orders:order:update_status`
  - Products: `products:product:create`, `products:product:read`, `products:product:update`, `products:product:delete`

## Policy Names
- Keep policy names stable and service-agnostic in Core:
  - Orders: `orders:create`, `orders:read`, `orders:update_status`
  - Products: `products:create`, `products:read`, `products:update`, `products:delete`

Policies are mapped to permission values in each service’s startup.

## Service Startup Contract
- Add authentication with Keycloak authority `http://localhost:8080/realms/turkcell-ai` (configurable via `Jwt:Authority`).
- Define policies using `RequireClaim("permissions", <permission>)`.
- Use `[Authorize(Policy = <policy>)]` on endpoints as per the OpenAPI security section.

## Change Management
- Any new permission must be added in:
  1. Keycloak realm (role or scope + mapper)
  2. OpenAPI (`components.securitySchemes` scopes and per-operation `security`)
  3. Core (policy name) and service startup (policy mapping)
  4. Controller attributes

All changes must be synchronized in a single PR.
