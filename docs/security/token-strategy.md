# Token Strategy

Defines OAuth2/OIDC flows, token lifetimes, rotation, and revocation policies for the platform using Keycloak as the default IdP.

## Flows
- Browser/SPAs (via BFF): Authorization Code Flow with PKCE. Browser talks to BFF only; BFF holds tokens server-side and maintains a secure session cookie.
- Service-to-Service: Client Credentials Flow (services confidential clients).
- Optional: Token Exchange for BFF → Gateway/API audience alignment when needed (e.g., `aud=gateway`).

## Token Lifetimes
- Access Token: 5–15 minutes (short-lived, reduces blast radius).
- Refresh Token: 30–60 minutes with rotation enabled.
- Clock Skew: ≤ 2 minutes tolerance on validation.

## Rotation & Revocation
- Refresh Token Rotation: ON. Any reuse triggers revocation of the session.
- Logout Propagation: Revoke refresh tokens and invalidate server sessions.
- Token Blacklist: Use Keycloak session/jti checks; avoid long-lived access tokens.

## Claims
- Include `permissions` (array) and `sub` (subject), `client_id`/`azp`, `exp`, `iat`, `jti`.
- For S2S, ensure `scope`/`client_roles` mapped to `permissions` where possible.

## Validation
- Gateway: Validate JWT (issuer, audience, signature via JWKS, expiry) and enforce deny-by-default.
- Services: Re-validate JWT and apply policy-based authorization.
- BFF: Validates OIDC responses, stores tokens server-side, issues only session cookie to browser.
- Audience: Enable audience validation for tokens used at Gateway/Services (e.g., `gateway`, `orderservice`, `productservice`).
- mTLS: Consider for critical internal hops.

## Secrets & Storage
- Dev: Dotnet User Secrets + environment variables.
- Docker: Docker Secrets for containerized deployments.
- Prod: Vault/Azure Key Vault for tokens/credentials; never store secrets in repo.

## Observability & Auditing
- Log `traceId`, `correlationId`, `userId/clientId`, `permissions`, outcome (Allow/Deny), latency.
- BFF: log login/logout, refresh outcomes, CSRF validation results.
- Alert on auth failures, token reuse, rate limit breaches, abnormal 403/401 spikes.

## Cookie & CSRF (BFF)
- Cookie: HttpOnly, Secure (prod), `SameSite=Lax` (dev) or `SameSite=None` with HTTPS.
- CSRF: Protect state-changing endpoints; use double-submit or anti-forgery tokens.
- CORS: Restrict to SPA origins; prefer browser → BFF only.
