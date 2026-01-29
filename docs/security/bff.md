# BFF Security Notes

## Cookies
- Name: session (HttpOnly, sliding expiration).
- Dev: SameSite=Lax, non-HTTPS. Prod: SameSite=None, Secure with HTTPS.
- No access/refresh tokens exposed to browser.

## CSRF
- Strategy: Double-submit header.
- Antiforgery cookie: bff.xsrf (readable by JS to set header).
- Header name: X-XSRF-TOKEN.
- Endpoint to fetch token: GET /bff/csrf returns { token } and sets antiforgery cookie.
- Apply [ValidateAntiForgeryToken] on state-changing endpoints (e.g., /bff/logout).

## CORS
- Prefer browser → BFF only. Restrict origins to SPA hosts.
- Gateway remains API-only for the browser (no direct calls).

## Audiences
- BFF obtains tokens targeted for gateway when calling backend via Gateway.
- Gateway and services validate audience lists accordingly.

## Observability
- Log login/logout, CSRF validation results, correlation IDs.
- Monitor 401/403 spikes and rate limiting.
