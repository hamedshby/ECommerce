# 🛒 ECommerce Platform

A production-grade e-commerce backend built with **.NET 8** and **Microservices Architecture** — inspired by platforms like Digikala.

## Architecture Overview

```
                        ┌─────────────────┐
                        │   API Gateway   │
                        └────────┬────────┘
                                 │
          ┌──────────────────────┼───────────────────────┐
          │                      │                       │
   ┌──────▼──────┐      ┌────────▼──────┐      ┌────────▼──────┐
   │  Identity   │      │    Product    │      │     Order     │
   │  Service    │      │    Service    │      │    Service    │
   └─────────────┘      └───────────────┘      └───────────────┘
          │                      │                       │
          └──────────────────────┼───────────────────────┘
                                 │
                     ┌───────────▼──────────┐
                     │   RabbitMQ (Events)  │
                     └──────────────────────┘
```

## Services

| Service | Description | Port |
|---|---|---|
| API Gateway | Routing, Rate Limiting, Auth middleware | 5000 |
| Identity Service | Register, Login, JWT, Refresh Token | 5001 |
| Product Service | Catalog, Search, Inventory | 5002 |
| Order Service | Orders, Status, History | 5003 |
| Payment Service | Payments, Refunds, Wallet | 5004 |
| Notification Service | Email, SMS | 5005 |

## Tech Stack

- **Framework:** .NET 8, ASP.NET Core
- **Architecture:** Clean Architecture + DDD
- **Messaging:** RabbitMQ + MassTransit
- **Cache:** Redis
- **Search:** Elasticsearch
- **Auth:** JWT + Refresh Token Rotation
- **Database:** PostgreSQL + EF Core 8
- **Testing:** xUnit, FluentAssertions, TestContainers
- **Containers:** Docker + Docker Compose

## Getting Started

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/ecommerce-platform.git

# Run with Docker Compose
docker-compose up -d

# Apply migrations
dotnet ef database update --project src/Services/Identity/ECommerce.Identity.Infrastructure
```

## Project Structure

```
/src
  /ApiGateway
  /SharedKernel
  /Services
    /Identity
      /ECommerce.Identity.API
      /ECommerce.Identity.Application
      /ECommerce.Identity.Domain
      /ECommerce.Identity.Infrastructure
    /Product    (coming soon)
    /Order      (coming soon)
    /Payment    (coming soon)
/tests
  /Unit
  /Integration
/docs
  /adr          # Architecture Decision Records
```

## Architecture Decision Records (ADR)

Design decisions and trade-offs are documented in [`/docs/adr`](./docs/adr).

## Roadmap

- [x] Solution structure & SharedKernel
- [x] Identity Service — Auth & JWT
- [ ] Product Service — Catalog & Search
- [ ] Order Service — Orders & State Machine
- [ ] Payment Service
- [ ] Notification Service
- [ ] Kubernetes deployment config

## License

MIT
