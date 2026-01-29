User: Tüm projeyi incele, OpenTelemetry kullanarak Observability entegrasyonunu planla. Ne nerede ne yapmalı bana rapor olarak sun.
Hatta gerek gördüğün dökümantasyonları docs/observability altında oluşturabilirsin.

GitHub Copilot: I'll scan the repo to inventory services and any existing observability, then draft a cross-service OpenTelemetry plan.

Repo observability context research

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src)

Searched for regex `OpenTelemetry|Serilog|Prometheus|Jaeger|Grafana|Loki|OTLP|otel|HealthChecks|AspNetCore.Diagnostics.HealthChecks` (`**/src/**`), 106 results

Searched for files matching `**/src/**/Program.cs`, 5 matches

Searched for files matching `**/src/**/Middleware/**`, 2 matches

Searched for files matching `**/src/**/appsettings*.json`, 3 matches

Searched for files matching `**/src/**/**/*.csproj`, 7 matches

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docker-compose.yml)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Gateway/Program.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Identity.Bff/Program.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/IdentityService/Program.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/OrderService/Program.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/ProductService/Program.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Gateway/Middleware/RequestSecurityMiddleware.cs)

Searched for regex `UseCoreExceptionHandling|ExceptionHandlingMiddleware|ErrorResponse|ErrorCode|ValidationBehavior|TransactionBehavior` (`**/src/TurkcellAI.Core/**`), 18 results

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/TurkcellAI.Core/Infrastructure/ExceptionHandlingMiddleware.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/TurkcellAI.Core/Application/DTOs/ErrorResponse.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/TurkcellAI.Core/Application/Enums/ErrorCode.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/OrderService/Middleware/ExceptionHandlingMiddleware.cs)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Gateway/TurkcellAI.Gateway.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/OrderService/OrderService.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/ProductService/ProductService.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/IdentityService/IdentityService.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Identity.Bff/Identity.Bff.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/TurkcellAI.Core/TurkcellAI.Core.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/TurkcellAI.Contracts/TurkcellAI.Contracts.csproj)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/OrderService/appsettings.json)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/ProductService/appsettings.json)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Gateway/appsettings.json)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/architecture/01-messaging-governance.md)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/security/gateway-policies.md)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/architecture/contracts-readme.md)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/security/bff.md)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/openapi/orderservice-v1.yaml#1-1), lines 1 to 400

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/openapi/productservice-v1.yaml#1-1), lines 1 to 240

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/openapi/productservice-v1.yaml#240-240), lines 240 to 442

Searched for files matching `**/docs/openapi/bff-v1.yaml`, 1 match

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/openapi/bff-v1.yaml)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/Gateway/yarp.json)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/openapi/identityservice-v1.yaml)

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/docs/development/local-setup.md)

Searched for regex `AddHealthChecks|MapHealthChecks\(` (`**/src/**`), 2 results

Searched for files matching `**/src/OrderService/Infrastructure/Messaging/**`, 1 match

Read [](file:///c%3A/Users/PC1/Desktop/Projects/Education/.NET/turkcell-ai/src/OrderService/Infrastructure/Messaging/MessagingRegistration.cs)

Kısa özet: Tüm servisleri envanterledim; mevcutta OpenTelemetry yok, health check ve korelasyon scope’ları sınırlı. OTLP üzerinden toplayıcıya gönderim, ASP.NET Core/HttpClient/EF/MassTransit enstrümantasyonu, W3C TraceContext + `X-Correlation-ID` yayılımı ve Core hata zarfı `traceId` standardizasyonu ile uçtan uca izlenebilirlik planladım. Local geliştirme için `otel-collector`, Jaeger, Prometheus ve Grafana ekleri öneriyorum. İlk dokümantasyon paketi için 5 dosya önerdim, onayına göre oluşturayım.

## Plan: OpenTelemetry Observability Entegrasyonu

OpenTelemetry ile tüm HTTP ve mesajlaşma akışlarını izlemek, metrik toplamak ve logları izlerle zenginleştirmek için OTLP kolektöre bağlanma, servis başına enstrümantasyon ve korelasyon standartlarını belirleme planı. Core’da hata zarfındaki `traceId` alanını W3C izleriyle hizalayıp, Gateway’den servisler ve mesajlaşmaya header yayılımını garanti altına alacağız. Lokal geliştirmede tek bir collector ve görsellerle doğrulama sağlanacak.

### Steps
1. Core uyumu ve konvansiyonları belirle: Application, Infrastructure, `ExceptionHandlingMiddleware` ve `ErrorResponse` için `traceId=Activity.TraceId` standardı; OpenAPI’lerle hizayı gözden geçir: orderservice-v1.yaml, productservice-v1.yaml, identityservice-v1.yaml.
2. Gateway entegrasyonu: Program.cs içine OpenTelemetry Tracing+Metrics kaydı, OTLP exporter (`otel-collector:4317`), `service.resource` öznitelikleri; `X-Correlation-ID` yayılımını doğrula (RequestSecurityMiddleware.cs, yarp.json).
3. Servis entegrasyonları (BFF, Identity): Program.cs, Program.cs içine ASP.NET Core + HttpClient enstrümantasyonu, kaynak öznitelikleri ve OTLP; controller’larda varsa dış çağrılar için `X-Correlation-ID` yayılımı (`AuthController`, `UserController` gibi).
4. Servis entegrasyonları (Order, Product): Program.cs, Program.cs içine ASP.NET Core + HttpClient + EF Core enstrümantasyonu ve OTLP; mesajlaşmada Activity yayılımını etkinleştir (MassTransit/RabbitMQ), `X-Correlation-ID` ve W3C `traceparent` header’larını publish/consume zincirine dahil et.
5. Header yayılımı ve log zenginleştirme: Tüm servislerde gelen `X-Correlation-ID`’yi log scope’larına alın; HttpClient varsayılan header’larına ekleyin; loglara `trace_id`/`span_id` alanlarını ekleyecek yaklaşıma karar verin (Opsiyon: OTLP Logs veya mevcut sink ile enrichment).
6. Lokal gözlemleme yığını ve compose: docker-compose.yml’a `otel-collector`, Jaeger, Prometheus, Grafana ekleri; collector pipeline’ını `docs/observability/collector.yaml` altında tanımla; servislerin OTLP endpoint’ini ortam değişkeniyle ayarlamayı planla.

### Further Considerations
1. Örnekleme: Dev’de %100; prod’da `ParentBasedTraceIdRatio` (örn. %5–10) seçenekleri.
2. Güvenlik: Log/metrikte PII ve token içeriklerini maskeleme; header listesi beyaz liste.
3. Metrik kardinalitesi: Dinamik etiketleri sınırlama; SLO/SLI metriklerinin ortak şeması.

Önerilen ilk dokümantasyon paketi (Small Batch, max 5 dosya):
- docs/observability/README.md: Hedefler, kaynak öznitelikleri, örnekleme ve yayılım konvansiyonları.
- docs/observability/local-stack.md: Local yığının çalışma talimatları, portlar ve doğrulama adımları.
- docs/observability/collector.yaml: Collector OTLP alıcıları, işlemciler (batch, resource), Jaeger/Prometheus/Grafana çıkışları.
- docs/observability/integration-gateway.md: Gateway OTel kurulumu, YARP header yayılımı ve korelasyon scope’ları.
- docs/observability/integration-services.md: Servislerde ASP.NET Core/HttpClient/EF/MassTransit enstrümantasyonu ve hata zarfı hizası.

İkinci paket (onaydan sonra): [docs/observability/logging.md], [docs/observability/metrics.md], örnek dashboard ve sorgular.

Bu taslağı onaylar mısın? Onaylarsan ilk 5 dokümanı oluşturup, ardından docker-compose.yml genişletmesi ve collector konfigürasyon taslağını eklemeyi planlayayım.