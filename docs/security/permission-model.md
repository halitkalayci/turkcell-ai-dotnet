# Permission Model (Operation Claims)

This document defines the platform-wide permission naming, sourcing from Keycloak, and how services consume permissions via ASP.NET Core policy-based authorization.

## Naming Convention
- Format: `service:resource:operation`
- Examples:
  - `orders:order:create`
  - `orders:order:read`
  - `orders:order:update_status`
  - `products:product:create`
  - `products:product:read`
  - `products:product:update`
  - `products:product:delete`

## Source of Truth (Keycloak)
- Permissions are modeled as realm-level roles or client-scopes mapped to a claim (preferred claim name: `permissions`).
- Users/clients receive permissions via role mappings or scope consent, governed by Keycloak policies.

## JWT Claims
- Access tokens must include a `permissions` array claim (string values matching the convention).
- For service-to-service flows (Client Credentials), `scope` or `client_roles` may be used; map to `permissions` via Keycloak protocol mappers when possible.

## Service Consumption
- ASP.NET Core policy-based authorization maps policies 1:1 with permissions.
- Each protected endpoint requires exactly one or more permission(s).
- Deny-by-default: any unspecified endpoints must explicitly opt-in to authorization.

## Endpoint × Permission Matrix
- OrderService
  - POST /api/v1/orders → `orders:order:create`
  - GET  /api/v1/orders → `orders:order:read`
  - GET  /api/v1/orders/{orderId} → `orders:order:read`
  - PATCH /api/v1/orders/{orderId}/status → `orders:order:update_status`
- ProductService
  - POST   /api/v1/products → `products:product:create`
  - GET    /api/v1/products → `products:product:read`
  - GET    /api/v1/products/{id} → `products:product:read`
  - PUT    /api/v1/products/{id} → `products:product:update`
  - DELETE /api/v1/products/{id} → `products:product:delete`

## Governance
- Contract-First: OpenAPI specs must reflect security requirements per endpoint.
- Shared Kernel: Policy names and claim names live in Core (Application layer) for consistency.
- Change Control: Any permission addition/removal must update Keycloak config, OpenAPI, and services in one PR.
