# 🏬 ShopVerse Microservices (.NET 10)

> A **hands-on microservices e-commerce platform** built with **.NET 10**, designed for learning industry best practices in distributed systems.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://www.docker.com/)
[![Kafka](https://img.shields.io/badge/Kafka-Redpanda-E2231A?logo=apachekafka)](https://redpanda.com/)
[![Keycloak](https://img.shields.io/badge/Auth-Keycloak-4D4D4D?logo=keycloak)](https://www.keycloak.org/)

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Getting Started](#-getting-started)
- [Learning Modules](#-learning-modules)
- [Service URLs](#-service-urls)
- [Project Structure](#-project-structure)
- [Testing Guide](#-testing-guide)
- [Progress Tracker](#-progress-tracker)

---

## 🎯 Overview

ShopVerse is an educational microservices project that simulates an e-commerce platform. The goal is to learn and implement production-grade patterns including:

- **API Gateway Pattern** - Single entry point with YARP reverse proxy
- **Event-Driven Architecture** - Asynchronous communication with Kafka
- **Authentication & Authorization** - OAuth2/OIDC with Keycloak
- **Structured Logging** - Centralized logs with Serilog and Seq
- **Resilience Patterns** - Circuit breakers and retries with Polly (coming soon)
- **Distributed Caching** - Performance optimization with Redis (coming soon)
- **Database per Service** - PostgreSQL and MSSQL (coming soon)
- **Observability** - Distributed tracing with OpenTelemetry (coming soon)

---

## 🏗 Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              CLIENTS                                     │
│                    (Browser, Mobile, Postman)                           │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                         API GATEWAY (YARP)                              │
│                         localhost:8085                                   │
│                    ┌─────────────────────────┐                          │
│                    │  JWT Authentication     │                          │
│                    │  Reverse Proxy          │                          │
│                    │  Load Balancing         │                          │
│                    └─────────────────────────┘                          │
└─────────────────────────────────────────────────────────────────────────┘
          │                       │                       │
          ▼                       ▼                       ▼
┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐
│  OrderService   │   │ PaymentService  │   │InventoryService│
│   :8090         │   │    :8092        │   │    :8094        │
└────────┬────────┘   └────────┬────────┘   └────────┬────────┘
         │                     │                     │
         └──────────────┬──────┴──────────────┬──────┘
                        │                     │
                        ▼                     ▼
         ┌──────────────────────┐   ┌──────────────────────┐
         │   Kafka (Redpanda)   │   │      Keycloak        │
         │      :19092          │   │       :8080          │
         └──────────────────────┘   └──────────────────────┘
```

### Event Flow

```
OrderService                PaymentService              InventoryService
     │                            │                            │
     │  OrderCreatedEvent         │                            │
     ├───────────────────────────►├───────────────────────────►│
     │                            │                            │
     │                     PaymentProcessed            StockReserved
     │                            │                            │
     │                            ▼                            ▼
     │                   shopverse.payments          shopverse.inventory
```

---

## 🛠 Technology Stack

### Core Technologies

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 10.0 | Backend framework |
| C# | 14.0 | Programming language |
| Docker | Latest | Containerization |
| Docker Compose | v2 | Container orchestration |

### Infrastructure

| Technology | Purpose | Port |
|------------|---------|------|
| YARP | API Gateway / Reverse Proxy | 8085 |
| Keycloak | OAuth2/OIDC Authentication | 8080 |
| Redpanda | Kafka-compatible message broker | 19092 |
| Redpanda Console | Kafka message viewer | 8088 |
| Seq | Centralized log aggregation | 8089 |

### Libraries

| Library | Purpose | Status |
|---------|---------|--------|
| Serilog | Structured logging | ✅ Implemented |
| Confluent.Kafka | Kafka client | ✅ Implemented |
| JwtBearer | JWT validation | ✅ Implemented |
| Polly | Resilience patterns | 🔜 Coming soon |
| EF Core | Database ORM | 🔜 Coming soon |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/pc7stha/ShopVerse.git
   cd ShopVerse
   ```

2. **Start all services**
   ```bash
   docker compose up --build
   ```

3. **Get an access token**
   ```bash
   curl -X POST http://localhost:8080/realms/shopverse/protocol/openid-connect/token \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "client_id=shopverse-api" \
     -d "client_secret=shopverse-api-secret" \
     -d "grant_type=password" \
     -d "username=testuser" \
     -d "password=password"
   ```

4. **Create an order (triggers event chain)**
   ```bash
   curl -X POST http://localhost:8085/api/orders \
     -H "Authorization: Bearer <your-token>" \
     -H "Content-Type: application/json" \
     -d '{"items":[{"productId":"laptop-001","productName":"Gaming Laptop","quantity":1,"unitPrice":999.99}]}'
   ```

5. **View the results**
   - Kafka messages: http://localhost:8088
   - Centralized logs: http://localhost:8089 (admin / Admin123!)

---

## 📚 Learning Modules

### ✅ Module 1: Solution Structure
**Status: Complete**

Learn how to structure a microservices solution with shared libraries.

**Key Concepts:**
- Service separation (OrderService, PaymentService, InventoryService)
- Shared libraries (BuildingBlocks.Common, BuildingBlocks.Messaging, BuildingBlocks.Observability)
- Docker containerization

---

### ✅ Module 2: API Gateway with YARP
**Status: Complete**

Implement a reverse proxy as the single entry point for all services.

**Key Concepts:**
- Reverse proxy pattern
- Route configuration
- Load balancing
- Request forwarding

**Files to Study:**
- `ApiGateway/Program.cs`
- `ApiGateway/appsettings.json`

---

### ✅ Module 3: Authentication with Keycloak
**Status: Complete**

Secure your APIs with OAuth2/OIDC using Keycloak.

**Key Concepts:**
- JWT Bearer authentication
- Realm and client configuration
- Token validation
- Audience verification

**Files to Study:**
- `docker/keycloak/shopverse-realm.json`
- `*/Program.cs` - JWT Bearer setup

---

### ✅ Module 4: Event-Driven Architecture with Kafka
**Status: Complete**

Implement asynchronous communication between services using events.

**Key Concepts:**
- Event sourcing basics
- Publisher/Consumer pattern
- Topic organization
- Event contracts

**Events Implemented:**
| Event | Publisher | Consumers | Topic |
|-------|-----------|-----------|-------|
| `OrderCreatedEvent` | OrderService | PaymentService, InventoryService | shopverse.orders |
| `PaymentProcessedEvent` | PaymentService | - | shopverse.payments |
| `StockReservedEvent` | InventoryService | - | shopverse.inventory |
| `StockFailedEvent` | InventoryService | - | shopverse.inventory |

**Files to Study:**
- `BuildingBlocks.Messaging/Events/*.cs`
- `BuildingBlocks.Messaging/Kafka/*.cs`
- `*/Consumers/*.cs`

---

### ✅ Module 5: Structured Logging with Serilog
**Status: Complete**

Implement production-grade logging with correlation across services.

**Key Concepts:**
- Structured logging vs text logging
- Log enrichment (ServiceName, CorrelationId)
- Multiple sinks (Console, File, Seq)
- Request logging middleware

**Log Output Example:**
```
[04:58:41 INF] [OrderService] [0HNIDL5M2IT2J:00000001] Creating order 16f71cc3...
[04:58:41 INF] [PaymentService] [0HNIDL5M2IT2J:00000001] Processing payment...
[04:58:41 INF] [InventoryService] [0HNIDL5M2IT2J:00000001] Reserved 1 of laptop-001...
```

**Files to Study:**
- `BuildingBlocks.Observability/Logging/SerilogExtensions.cs`
- `BuildingBlocks.Observability/Logging/CorrelationIdMiddleware.cs`

---

### 🔜 Module 6: Resilience with Polly
**Status: Coming Next**

Handle failures gracefully with retry policies and circuit breakers.

**What You'll Learn:**
- Retry policies with exponential backoff
- Circuit breaker pattern (Closed → Open → Half-Open)
- Timeout policies
- Fallback strategies

---

### 🔜 Module 7: Distributed Caching with Redis
**Status: Planned**

Improve performance with distributed caching.

---

### 🔜 Module 8: Database per Service
**Status: Planned**

Implement the database-per-service pattern with EF Core.

---

### 🔜 Module 9: RabbitMQ Alternative
**Status: Planned**

Compare Kafka and RabbitMQ messaging patterns.

---

### 🔜 Module 10: Code Quality with SonarQube
**Status: Planned**

Maintain code quality with static analysis.

---

### 🔜 Module 11: Distributed Tracing with OpenTelemetry
**Status: Planned**

Full observability with traces and metrics.

---

## 🌐 Service URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| API Gateway | http://localhost:8085 | Bearer token |
| Keycloak Admin | http://localhost:8080 | admin / admin |
| Redpanda Console | http://localhost:8088 | - |
| Seq (Logs) | http://localhost:8089 | admin / Admin123! |
| OrderService (direct) | http://localhost:8090 | Bearer token |
| PaymentService (direct) | http://localhost:8092 | Bearer token |
| InventoryService (direct) | http://localhost:8094 | Bearer token |

---

## 📁 Project Structure

```
ShopVerse/
├── ApiGateway/                    # YARP reverse proxy
│   ├── Controllers/
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
│
├── OrderService/                  # Order management
│   ├── Controllers/
│   ├── Models/
│   └── Dockerfile
│
├── PaymentService/                # Payment processing
│   ├── Controllers/
│   ├── Consumers/                 # Kafka consumers
│   └── Dockerfile
│
├── InventoryService/              # Stock management
│   ├── Controllers/
│   ├── Consumers/
│   └── Dockerfile
│
├── BuildingBlocks.Common/         # Shared utilities
│
├── BuildingBlocks.Messaging/      # Kafka infrastructure
│   ├── Events/
│   └── Kafka/
│
├── BuildingBlocks.Observability/  # Logging infrastructure
│   └── Logging/
│
├── docker/
│   └── keycloak/
│       └── shopverse-realm.json
│
├── docker-compose.yml
├── COPILOT_PROGRESS.md           # Detailed progress tracker
└── README.md
```

---

## 🧪 Testing Guide

### Test Products (Inventory)

| Product ID | Name | Initial Stock |
|------------|------|---------------|
| `laptop-001` | Gaming Laptop | 100 |
| `mouse-001` | Wireless Mouse | 500 |
| `keyboard-001` | Mechanical Keyboard | 200 |
| `monitor-001` | 4K Monitor | 50 |
| `headphones-001` | Wireless Headphones | 150 |

### Test Scenarios

**1. Happy Path - Order with valid product:**
```bash
# Creates order → Payment processed → Stock reserved
```

**2. Payment Failure - Order over $10,000:**
```bash
# Creates order → Payment failed event
```

**3. Stock Failure - Unknown product:**
```bash
# Creates order → Stock failed event
```

### Viewing Results

```bash
# Check service logs
docker logs shopverse-orders-1 --tail 20
docker logs shopverse-payments-1 --tail 20
docker logs shopverse-inventory-1 --tail 20

# Or view all in Seq: http://localhost:8089
```

---

## 📊 Progress Tracker

| Module | Status | Completion Date |
|--------|--------|-----------------|
| Solution Structure | ✅ Complete | Jan 2026 |
| API Gateway (YARP) | ✅ Complete | Jan 2026 |
| Authentication (Keycloak) | ✅ Complete | Jan 2026 |
| Docker Compose | ✅ Complete | Jan 2026 |
| Event-Driven (Kafka) | ✅ Complete | Jan 2026 |
| Structured Logging (Serilog) | ✅ Complete | Jan 2026 |
| Resilience (Polly) | 🔜 Next | - |
| Caching (Redis) | 📋 Planned | - |
| Database (PostgreSQL/MSSQL) | 📋 Planned | - |
| RabbitMQ | 📋 Planned | - |
| Code Quality (SonarQube) | 📋 Planned | - |
| Distributed Tracing (OpenTelemetry) | 📋 Planned | - |

For detailed progress, see [COPILOT_PROGRESS.md](COPILOT_PROGRESS.md).

---

## 🤝 Contributing

This is a learning project — contributions, improvements and suggestions are welcome!

1. Fork repo
2. Create branch
3. Commit changes
4. Open PR

---

## ⭐ Support

If this helps you learn:
- Star ⭐ the repo
- Share it
- Follow for future updates

---

## 🧑‍💻 Author

Learning project created for educational purposes and real-world architecture practice.

Built with guidance from **GitHub Copilot** 🤖

---

*Happy Learning! 🚀*
