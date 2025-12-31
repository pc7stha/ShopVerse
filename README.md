# 🏬 ShopVerse Microservices (.NET 10)

A **beginner-friendly yet production-realistic** microservices learning project built using **.NET 10**, focused on modern enterprise architecture, event-driven communication, security, observability, and containerized deployment.

This project is designed for learners who want to **practice real-world microservice concepts** while keeping things understandable and structured.

---

## 🎯 Architecture Overview

**ShopVerse** simulates an e-commerce backend with independent services communicating asynchronously.

### 🧩 Services

| Service             | Responsibility     |
| ------------------- | ------------------ |
| 🛒 OrderService     | Handles orders     |
| 💳 PaymentService   | Processes payments |
| 📦 InventoryService | Manages stock      |

---

## 🚀 Technologies Used

### Core Platform

* ✅ **.NET 10 (LTS)**
* ✅ **Clean Architecture**

### Security

* 🔐 **Keycloak** (JWT Authentication)

### Communication

* 🔄 **Kafka** (Event Streaming — Order → Payment events)
* 📨 **RabbitMQ** (Background Task Queue — Inventory updates)

### API Access

* 🚪 **YARP API Gateway**

### Observability

* 📊 **OpenTelemetry** (Tracing)
* 📝 **Serilog** (Structured Logging)

### Performance

* ⚡ **Redis Cache**

### Quality & DevOps

* 🐳 **Docker & Docker Compose**
* 🧪 **SonarQube**

---

## 🏗️ Solution Structure

```
ShopVerse
 ├── ApiGateway
 ├── Services
 │    ├── OrderService
 │    ├── PaymentService
 │    └── InventoryService
 ├── BuildingBlocks
 │    ├── BuildingBlocks.Common
 │    ├── BuildingBlocks.Messaging
 │    └── BuildingBlocks.Observability
```

Each service follows **Clean Architecture style**:

```
Controllers
Application
Domain
Infrastructure
Persistence
```

---

## 🧰 Development Environment

| Tool                    | Version |
| ----------------------- | ------- |
| Visual Studio           | 2022+   |
| .NET                    | 10 LTS  |
| Docker Desktop          | Latest  |
| SQL Server / PostgreSQL | Any     |
| Kafka                   | Latest  |
| RabbitMQ                | Latest  |

---

## ✅ What This Project Teaches You

✔️ Microservices fundamentals
✔️ Communication patterns
✔️ Authentication in distributed systems
✔️ Event-driven architecture
✔️ Distributed tracing
✔️ Clean Architecture
✔️ Containerization with Docker
✔️ Resilience & reliability patterns

---

## 🛠️ Setup & Run

### 1️⃣ Clone Repository

```
git clone https://github.com/YOUR_USERNAME/ShopVerse-Microservices.git
```

```
cd ShopVerse-Microservices
```

---

### 2️⃣ Open Solution

Open `ShopVerse.sln` in Visual Studio.

---

### 3️⃣ Run Services (Development Mode)

Set startup projects to **Multiple Startup Projects**:

* ApiGateway
* OrderService
* PaymentService
* InventoryService

Run ✔️
You should see multiple Swagger UIs.

---

## 🔜 Upcoming Implementations

| Feature                          | Status |
| -------------------------------- | ------ |
| Add YARP Gateway Routing         | ⏳ Soon |
| Integrate Keycloak               | ⏳ Soon |
| Add Kafka Order → Payment Events | ⏳ Soon |
| Add RabbitMQ Inventory Queue     | ⏳ Soon |
| Add OpenTelemetry + Jaeger       | ⏳ Soon |
| Add Serilog Logging              | ⏳ Soon |
| Add Redis Cache                  | ⏳ Soon |
| Docker Compose Full Stack        | ⏳ Soon |

---

## 📚 Learning Approach

We’ll integrate components **step-by-step** instead of dumping everything at once to avoid overwhelm:

1️⃣ Create base microservices
2️⃣ Add YARP
3️⃣ Add Authentication
4️⃣ Add Kafka
5️⃣ Add RabbitMQ
6️⃣ Add Observability
7️⃣ Add Redis
8️⃣ Wrap in Docker Compose

---

## 🤝 Contributing

This is a learning project — contributions, improvements and suggestions are welcome 😊

1. Fork repo
2. Create branch
3. Commit changes
4. Open PR

---

## ⭐ Support & Motivation

If this helps you learn:

* Star ⭐ the repo
* Share it
* Follow for future updates

---

## 🧑‍💻 Author

Learning project created for educational purposes and real-world architecture practice.
