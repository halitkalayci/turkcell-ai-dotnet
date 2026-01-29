# Local Setup & Quickstart

## Prerequisites
- .NET SDK 9
- Docker Desktop

## Start Infrastructure
```bash
docker compose up -d rabbitmq mssql keycloak
```

## Import Realm (Optional)
- Automatic on container start via mounted `infra/keycloak/turkcell-ai-realm.json`.
- Or follow [docs/security/keycloak-setup.md](../security/keycloak-setup.md) to import manually.

## Build & Run
```bash
# Add Gateway to solution once
dotnet sln turkcell-ai.sln add src/Gateway/TurkcellAI.Gateway.csproj

# Build solution
dotnet build

# Run services (separate terminals)
dotnet run --project src/OrderService/OrderService.csproj

dotnet run --project src/ProductService/ProductService.csproj

dotnet run --project src/Gateway/TurkcellAI.Gateway.csproj
```

## Test
1. Get a token from Keycloak (client credentials or user flow).
2. Call the Gateway:
```bash
# Replace {PORT} with the Gateway HTTP port
curl -H "Authorization: Bearer $ACCESS_TOKEN" http://localhost:{PORT}/api/v1/orders
```

Expect 401/403 without proper permissions; 2xx with valid tokens and scopes.
