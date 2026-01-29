# Keycloak Setup (Admin Console)

This guide walks through configuring the realm and clients manually in Keycloak Admin Console. Alternatively, you can import the provided realm export: `infra/keycloak/turkcell-ai-realm.json`.

## Option A — Import Realm (Recommended)
1. Start Keycloak (dev):
   - `docker compose up -d keycloak`
   - Admin user: `admin / admin` (dev only)
2. Open `http://localhost:8080/admin/` and sign in.
3. Go to `Realm selector` → `Add realm` → `Import` tab.
4. Choose file: `infra/keycloak/turkcell-ai-realm.json`.
5. Confirm and create. Realm name should be `turkcell-ai`.

## Option B — Manual Configuration

### 1) Create Realm
- Realms → `Create realm`
  - Name: `turkcell-ai`
  - Login settings: `Remember Me = On` (optional dev), SSL required = external.

### 2) Realm Roles (Permissions)
- Realm Roles → `Create role` (repeat for each):
  - `orders:order:create`
  - `orders:order:read`
  - `orders:order:update_status`
  - `products:product:create`
  - `products:product:read`
  - `products:product:update`
  - `products:product:delete`

### 3) Client Scope: `permissions`
- Client Scopes → `Create` → Name: `permissions`, Protocol: `openid-connect`.
- `permissions` scope → `Mappers` → `Create mapper`:
  - Mapper type: `User Realm Role`
  - Name: `realm-roles-to-permissions`
  - Token Claim Name: `permissions`
  - Add to ID token: On; Add to access token: On; Add to userinfo: Off
  - Multivalued: On; Realm Role prefix: empty

Set `permissions` as a default client scope:
- Realm Settings → Client Scopes → Default Client Scopes → Add `permissions`.

### 4) Clients

Gateway (Public — Authorization Code + PKCE):
- Clients → `Create client`
  - Client ID: `gateway`
  - Client type: `OpenID Connect`
  - Access type: `Public`
  - Valid redirect URIs: `http://localhost:5000/*`, `http://localhost:5173/*`, `http://localhost:8081/*`
  - Web origins: `*` (dev); restrict per host in prod
  - Standard flow: On; Direct access grants: Off; Service accounts: Off
  - Default client scopes: ensure `permissions` is listed

OrderService (Confidential — Client Credentials):
- Clients → `Create client`
  - Client ID: `orderservice`
  - Access type: `Confidential`
  - Standard flow: Off; Service accounts: On; Direct access grants: Off
  - Save, then `Credentials` tab → Copy secret (use Docker/Key Vault in prod)
  - `Service Account Roles` → Assign the minimal realm roles (permissions) needed for S2S calls

ProductService (Confidential — Client Credentials):
- Clients → `Create client`
  - Client ID: `productservice`
  - Access type: `Confidential`
  - Standard flow: Off; Service accounts: On; Direct access grants: Off
  - Save, then `Credentials` tab → Copy secret
  - `Service Account Roles` → Assign minimal permissions

### 5) Users (for interactive testing)
- Users → `Add user` → Set username/email.
- Credentials → Set password; Temporary = Off (dev only)
- Role Mappings → Assign the necessary realm roles (permissions), e.g., `orders:order:read`.

### 6) Token Settings (Optional Hardening)
- Realm Settings → Tokens:
  - Access token lifespan: 5–15 minutes
  - SSO session idle: 30 minutes
  - Revoke refresh token: On, Max reuse = 0

## Testing Tokens

Authorization Code (browser app):
- Use a simple SPA or gateway callback to do PKCE. For quick tests, use a REST client that supports auth code + PKCE.

Client Credentials (service-to-service):
```bash
# OrderService token (confidential client)
curl -s -X POST \
  -d "client_id=orderservice" \
  -d "client_secret=<orderservice-secret>" \
  -d "grant_type=client_credentials" \
  http://localhost:8080/realms/turkcell-ai/protocol/openid-connect/token
```

Add the returned `access_token` to `Authorization: Bearer <token>` when calling services through the Gateway.

## Troubleshooting
- 401 Unauthorized at Gateway: check token issuer/audience, expiry, signature (JWKS reachable), and `Jwt:Authority`.
- 403 Forbidden: token lacks the required `permissions`. Assign corresponding realm roles to the user or service account.
- CORS issues (when testing from a browser): restrict Web Origins to exact origins and configure the Gateway CORS accordingly.
