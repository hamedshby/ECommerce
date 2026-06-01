# ADR 001: Use Clean Architecture per Service

## Status
Accepted

## Context
Each microservice needs a consistent internal structure that separates business logic from infrastructure concerns.

## Decision
Each service follows Clean Architecture with 4 layers:
- **Domain** — Entities, Value Objects, Domain Events (no dependencies)
- **Application** — Use Cases, Interfaces, DTOs (depends on Domain only)
- **Infrastructure** — EF Core, Redis, HTTP clients (implements Application interfaces)
- **API** — Controllers, Middleware (depends on Application)

## Consequences
- Business logic is fully testable without spinning up a DB
- Easy to swap infrastructure (e.g., change ORM or cache provider)
- Slightly more boilerplate, but pays off at scale
