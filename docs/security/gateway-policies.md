# Gateway Security Policies

This document explains the security controls enforced by the Gateway and how to configure them per environment.

## Responsibilities
- Verify JWTs (Keycloak OIDC) and pass only valid requests upstream.
- Enforce deny-by-default; only configured routes are reachable.
- Apply rate limiting and basic request hygiene (size/IP filtering).
- Propagate correlation identifiers for observability.

## JWT Validation
- Authority: `Jwt:Authority` in `src/Gateway/appsettings.json` (dev default: `http://localhost:8080/realms/turkcell-ai`).
- Validation: issuer, signature (JWKS), lifetime with 2m clock skew.
- Audience: currently disabled for developer ergonomics; enable in hardened envs.

Recommended hardening:
```jsonc
// appsettings.Production.json
{
  "Jwt": {
    "Authority": "https://id.example.com/realms/turkcell-ai",
    "ValidAudiences": ["orderservice", "productservice", "gateway"]
  }
}
```
Then extend `TokenValidationParameters` to check audiences.

## Rate Limiting
- Global token-bucket per client IP (defaults):
  - `TokenLimit`: 100; `TokensPerPeriod`: 20 per 10s; `QueueLimit`: 50.
- Adjust values in `Program.cs` or move to config as needed.

## IP Allow/Deny & Correlation
- Options in `Security` section of `appsettings.json`:
  - `IpAllowList`: array of CIDR or `*`.
  - `IpDenyList`: array of CIDR.
  - `CorrelationHeaderName`: defaults to `X-Correlation-ID`.
- Middleware ensures every request has a correlation id and logs include it.

## YARP Routing
- Routes and clusters configured in `src/Gateway/yarp.json`:
  - `/api/v1/orders` → `http://localhost:5000/`
  - `/api/v1/products` → `http://localhost:5002/`
- Reference loaded via `$ref` from `appsettings.json`.

## Logging
- Log scope includes: `correlationId`, `clientIp`, and `userId` (if present).
- Surface 401/403/429/5xx metrics for alerts.

## Minimal WAF-Like Controls
- Request size limits (Kestrel) and header sanitization can be added in `Program.cs`.
- Deny requests with invalid/missing JWT by default.

## Testing
1) Obtain an access token from Keycloak with required permissions.
2) Call gateway route with `Authorization: Bearer <access_token>`.
3) Expect 401/403 when scope/permission is missing; 2xx when present.
