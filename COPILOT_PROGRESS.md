# ShopVerse Microservices - Progress Tracker

## 🎯 Project Goal
Build a production-style microservices e-commerce platform using .NET 10, learning industry best practices for:
- API Gateway pattern (YARP)
- Authentication/Authorization (Keycloak)
- Event-driven architecture (Kafka/RabbitMQ)
- Observability (Serilog, OpenTelemetry)
- Resilience (Polly Circuit Breaker)
- Caching (Redis)
- Data Persistence (PostgreSQL, MSSQL)
- Code Quality (SonarQube)
- Containerization (Docker)

---

## ✅ Completed Milestones

### 1. Solution Structure ✅
- [x] Created solution with service projects: `ApiGateway`, `OrderService`, `PaymentService`, `InventoryService`
- [x] Created shared libraries: `BuildingBlocks.Common`, `BuildingBlocks.Messaging`, `BuildingBlocks.Observability`
- [x] Dockerized all services with individual Dockerfiles

### 2. API Gateway (YARP) ✅
- [x] Configured YARP reverse proxy in `ApiGateway`
- [x] Routes: `/api/orders/*` → OrderService, `/api/payments/*` → PaymentService, `/api/inventory/*` → InventoryService
- [x] Environment-specific configs: `appsettings.Development.json`, `appsettings.Docker.json`
- [x] Docker networking: services communicate via container DNS names

### 3. Authentication with Keycloak ✅
- [x] Keycloak container in docker-compose with auto-import realm
- [x] Realm `shopverse` with client `shopverse-api` and user `testuser`
- [x] JWT Bearer authentication on gateway and all services
- [x] Audience mapper configured for `shopverse-api`
- [x] Realm export saved to `docker/keycloak/shopverse-realm.json`

### 4. Docker Compose Setup ✅
- [x] All services run in Docker with HTTP-only
- [x] Keycloak auto-imports realm on startup
- [x] Environment: `ASPNETCORE_ENVIRONMENT=Docker` for all services
- [x] Tested: GET `/api/orders` with valid JWT returns 200 OK

### 5. Kafka Event-Driven Communication (Basic) ✅
- [x] Added Redpanda (Kafka-compatible broker) to docker-compose
- [x] Added Redpanda Console UI at `http://localhost:8088`
- [x] Created shared event contracts in `BuildingBlocks.Messaging`:
  - `IIntegrationEvent` base interface
  - `OrderCreatedEvent` with order details
- [x] Created Kafka infrastructure:
  - `KafkaSettings`, `KafkaTopics`, `IEventPublisher`, `KafkaEventPublisher`
- [x] OrderService publishes `OrderCreatedEvent` on POST `/api/orders`
- [x] PaymentService consumes events via `OrderCreatedConsumer`
- [x] Tested end-to-end: Create order → Event published → Payment service logs processing

### 6. Event Chaining (Multiple Events) ✅ NEW
- [x] Created `PaymentProcessedEvent` in `BuildingBlocks.Messaging`
- [x] PaymentService publishes `PaymentProcessedEvent` after processing order payment
- [x] Simulated payment gateway (90% success, fails if amount > $10,000)
- [x] Created `StockReservedEvent` and `StockFailedEvent`
- [x] InventoryService consumes `OrderCreatedEvent` to check/reserve stock
- [x] InventoryService publishes `StockReservedEvent` on success
- [x] InventoryService publishes `StockFailedEvent` with rollback on insufficient stock
- [x] Build successful ✅

**Event Flow:**
```
OrderService                PaymentService              InventoryService
     │                            │                            │
     │  OrderCreatedEvent         │                            │
     ├───────────────────────────►├───────────────────────────►│
     │                            │                            │
     │                            │  PaymentProcessedEvent     │  StockReservedEvent
     │                            ├─────────────►              │  ─────────────►
     │                            │                            │  
     │                            │  (if failed)               │  StockFailedEvent
     │                            │                            │  ─────────────►
```

**Topics:**
- `shopverse.orders` - Order events (OrderCreatedEvent)
- `shopverse.payments` - Payment events (PaymentProcessedEvent)
- `shopverse.inventory` - Inventory events (StockReservedEvent, StockFailedEvent)

---

## 🚧 Current Status: Ready for Testing

### To Test Event Chain:
```sh
# 1. Rebuild and start
docker compose down
docker compose build
docker compose up -d

# 2. Get token
curl -X POST http://localhost:8080/realms/shopverse/protocol/openid-connect/token \
  -d "client_id=shopverse-api&client_secret=shopverse-api-secret&grant_type=password&username=testuser&password=password"

# 3. Create order (known product)
curl -X POST http://localhost:8085/api/orders \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"items":[{"productId":"laptop-001","productName":"Laptop","quantity":1,"unitPrice":999.99}]}'

# 4. Check logs
docker logs shopverse-orders-1 --tail 10      # Order created
docker logs shopverse-payments-1 --tail 10    # Payment processed
docker logs shopverse-inventory-1 --tail 10   # Stock reserved

# 5. View in Redpanda Console: http://localhost:8088
```

### Test Scenarios:
- ✅ **Happy path**: Order with known product → Payment success → Stock reserved
- ⚠️ **Payment failure**: Order > $10,000 → Payment failed event published
- ⚠️ **Stock failure**: Unknown product ID → Stock failed event published

---

## 📋 Learning Roadmap (Next Steps)

### 7. Polly - Resilience & Circuit Breaker 🔜 NEXT
**What you'll learn:** Handle failures gracefully in distributed systems
- [ ] Add Polly to HTTP clients
- [ ] Implement retry policies with exponential backoff
- [ ] Implement circuit breaker pattern
- [ ] Add timeout policies
- [ ] Combine policies (retry + circuit breaker + timeout)
- [ ] Add fallback strategies

**Key Concepts:**
- Transient fault handling
- Circuit breaker states (Closed, Open, Half-Open)
- Bulkhead isolation
- Fallback patterns

### 8. Redis Cache
**What you'll learn:** Distributed caching for performance
- [ ] Add Redis to docker-compose
- [ ] Implement distributed cache in services
- [ ] Cache frequently accessed data (products, user sessions)
- [ ] Implement cache invalidation strategies
- [ ] Use Redis for rate limiting

**Key Concepts:**
- Cache-aside pattern
- TTL (Time-To-Live)
- Cache invalidation
- Distributed session state

### 9. PostgreSQL - Database per Service
**What you'll learn:** Relational database with EF Core
- [ ] Add PostgreSQL to docker-compose
- [ ] Configure EF Core for OrderService
- [ ] Implement repository pattern
- [ ] Database migrations
- [ ] Seed data

**Key Concepts:**
- Database-per-service pattern
- Code-first migrations
- Connection pooling
- Transaction management

### 10. MSSQL - Alternative Database
**What you'll learn:** SQL Server in microservices
- [ ] Add MSSQL container for PaymentService
- [ ] Compare PostgreSQL vs MSSQL configuration
- [ ] Implement same patterns with different provider

**Key Concepts:**
- Multi-database architecture
- Database abstraction
- Provider-specific features

### 11. RabbitMQ - Alternative Message Broker
**What you'll learn:** Compare Kafka vs RabbitMQ
- [ ] Add RabbitMQ to docker-compose
- [ ] Implement MassTransit for abstraction
- [ ] Create exchanges and queues
- [ ] Implement pub/sub and request/response patterns
- [ ] Compare with Kafka implementation

**Key Concepts:**
- Exchanges (direct, topic, fanout)
- Queues and bindings
- Message acknowledgment
- Dead letter queues

### 12. SonarQube - Code Quality
**What you'll learn:** Static code analysis and quality gates
- [ ] Add SonarQube to docker-compose
- [ ] Configure SonarScanner for .NET
- [ ] Analyze code quality metrics
- [ ] Fix code smells and vulnerabilities
- [ ] Set up quality gates

**Key Concepts:**
- Code coverage
- Technical debt
- Security vulnerabilities
- Maintainability metrics

### 13. OpenTelemetry - Distributed Tracing
**What you'll learn:** Full observability with traces and metrics
- [ ] Add OpenTelemetry instrumentation
- [ ] Trace requests across services
- [ ] Export to Jaeger/Zipkin
- [ ] Add custom metrics
- [ ] Dashboard with Grafana

**Key Concepts:**
- Traces, spans, and context propagation
- Metrics (counters, gauges, histograms)
- Observability pillars (logs, traces, metrics)

---

## 🔧 Quick Commands

### Start all services
```sh
docker compose up --build
```

### Get Keycloak token
```sh
curl -X POST http://localhost:8080/realms/shopverse/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=shopverse-api" \
  -d "client_secret=shopverse-api-secret" \
  -d "grant_type=password" \
  -d "username=testuser" \
  -d "password=password"
```

### Create Order (triggers full event chain)
```sh
curl -X POST http://localhost:8085/api/orders \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"items":[{"productId":"laptop-001","productName":"Gaming Laptop","quantity":1,"unitPrice":999.99}]}'
```

### Check Service Logs
```sh
docker logs shopverse-orders-1 --tail 20      # Order events
docker logs shopverse-payments-1 --tail 20    # Payment events
docker logs shopverse-inventory-1 --tail 20   # Inventory events
```

### View Centralized Logs (Seq)
Open `http://localhost:8089` → Login: `admin` / `Admin123!`

### View Kafka messages (Redpanda Console)
Open `http://localhost:8088` → Topics → Select topic → Messages

---

## 📁 Key Files

| File | Purpose |
|------|---------|
| **Docker** | |
| `docker-compose.yml` | Main compose file with all services |
| `docker-compose.override.yml` | Dev overrides (environment, volumes) |
| `docker/keycloak/shopverse-realm.json` | Keycloak realm export for auto-import |
| **Events** | |
| `BuildingBlocks.Messaging/Events/IIntegrationEvent.cs` | Base event interface |
| `BuildingBlocks.Messaging/Events/OrderCreatedEvent.cs` | Order created event |
| `BuildingBlocks.Messaging/Events/PaymentProcessedEvent.cs` | Payment processed event |
| `BuildingBlocks.Messaging/Events/StockReservedEvent.cs` | Stock reserved event |
| `BuildingBlocks.Messaging/Events/StockFailedEvent.cs` | Stock failed event |
| **Kafka Infrastructure** | |
| `BuildingBlocks.Messaging/Kafka/KafkaSettings.cs` | Configuration |
| `BuildingBlocks.Messaging/Kafka/KafkaTopics.cs` | Topic names |
| `BuildingBlocks.Messaging/Kafka/IEventPublisher.cs` | Publisher interface |
| `BuildingBlocks.Messaging/Kafka/KafkaEventPublisher.cs` | Publisher implementation |
| **Logging Infrastructure** | |
| `BuildingBlocks.Observability/Logging/SerilogExtensions.cs` | Serilog configuration |
| `BuildingBlocks.Observability/Logging/CorrelationIdMiddleware.cs` | Correlation ID propagation |
| **Service Consumers** | |
| `PaymentService/Consumers/OrderCreatedConsumer.cs` | Consumes orders, publishes payments |
| `InventoryService/Consumers/OrderCreatedConsumer.cs` | Consumes orders, publishes stock events |

---

## 📚 Technology Stack Summary

| Technology | Purpose | Status |
|------------|---------|--------|
| .NET 10 | Backend framework | ✅ In Use |
| YARP | API Gateway | ✅ Complete |
| Keycloak | Authentication | ✅ Complete |
| Docker | Containerization | ✅ Complete |
| Kafka (Redpanda) | Event streaming | ✅ Complete |
| Serilog | Structured logging | ✅ Complete |
| Seq | Log aggregation | ✅ Complete |
| Polly | Resilience/Circuit Breaker | 🔜 Next |
| Redis | Distributed cache | 🔜 Planned |
| PostgreSQL | Database (OrderService) | 🔜 Planned |
| MSSQL | Database (PaymentService) | 🔜 Planned |
| RabbitMQ | Message broker alternative | 🔜 Planned |
| SonarQube | Code quality | 🔜 Planned |
| OpenTelemetry | Distributed tracing | 🔜 Planned |

---

## 📊 Inventory Stock (For Testing)

The InventoryService has these products in simulated inventory:

| Product ID | Initial Stock |
|------------|---------------|
| `laptop-001` | 100 |
| `mouse-001` | 500 |
| `keyboard-001` | 200 |
| `monitor-001` | 50 |
| `headphones-001` | 150 |

**Test with unknown product** (e.g., `unknown-product`) to trigger `StockFailedEvent`.

---

## 🌐 Service URLs

| Service | URL | Purpose |
|---------|-----|---------|
| API Gateway | http://localhost:8085 | Main entry point |
| Keycloak | http://localhost:8080 | Auth server (admin: admin/admin) |
| Redpanda Console | http://localhost:8088 | Kafka message viewer |
| Seq | http://localhost:8089 | Centralized logs (admin/Admin123!) |
| OrderService | http://localhost:8090 | Direct access (for debugging) |
| PaymentService | http://localhost:8092 | Direct access (for debugging) |
| InventoryService | http://localhost:8094 | Direct access (for debugging) |

---

*Last updated: January 2026*
