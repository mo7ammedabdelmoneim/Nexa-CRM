# NexaCRM — Sales Pipeline & Lead Management API

> A production-grade backend API built with **ASP.NET Core 8**, **Domain-Driven Design**, and **Clean Architecture**. Designed to demonstrate enterprise-level backend engineering across a focused vertical of a real CRM system.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Resources](#resources)
- [Getting Started](#getting-started)
- [API Reference](#api-reference)
  - [Auth](#auth)
  - [Leads](#leads)
  - [Pipeline / Deals](#pipeline--deals)
  - [Activities](#activities)
  - [Tasks](#tasks)
  - [Contacts](#contacts)
  - [Notifications](#notifications)
  - [Reports](#reports)
  - [Audit Logs](#audit-logs)
- [Full Walkthrough](#full-walkthrough)
- [Real-Time Events (SignalR)](#real-time-events-signalr)
- [Background Jobs (Hangfire)](#background-jobs-hangfire)
- [Response Contracts](#response-contracts)
- [Frontend Integration Guide](#frontend-integration-guide)

---

## Overview

**NexaCRM** is a backend API that models the core sales workflow of a CRM system — from capturing a raw lead all the way through pipeline management, deal closure, activity tracking, and reporting.

### What it demonstrates

| Concern | Implementation |
|---|---|
| Domain modeling | DDD Aggregates, Value Objects, Domain Events, State Machine |
| Application layer | CQRS with MediatR, FluentValidation, Pipeline Behaviors |
| Data access | EF Core 8, Repository Pattern, Unit of Work, Soft Delete |
| Authentication | Custom JWT + Rotating Refresh Tokens, BCrypt |
| Real-time | SignalR hub with per-user group delivery |
| Background jobs | Hangfire with persistent SQL Server queue |
| Observability | Serilog structured logging, Audit trail on every command |
| API design | RESTful, paginated, consistent error envelopes |

### What it covers

```
Lead Management      → Capture and advance leads through a strict state machine
Pipeline Management  → Track deals across stages with aggregate value views
Activity Tracking    → Immutable log of all interactions (calls, emails, meetings)
Task Management      → Assignable tasks with due dates and overdue detection
Contact Management   → Rich contact profiles linked to leads and deals
Notifications        → Real-time + async email on every significant event
Reporting            → Conversion rates, pipeline value, sales rep performance
Audit Logs           → Complete history of every write operation
```

---

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                    NexaCRM.API                      │
│          Controllers · Middleware · Program          │
└────────────────────┬────────────────────────────────┘
                     │ uses
┌────────────────────▼────────────────────────────────┐
│                NexaCRM.Application                  │
│   Commands · Queries · Handlers · Validators        │
│   Behaviors · DTOs · Contracts · Event Handlers     │
└────────────────────┬────────────────────────────────┘
          uses       │       uses
       ┌─────────────┘         └──────────────┐
       ▼                                      ▼
┌──────────────────┐              ┌───────────────────────┐
│  NexaCRM.Domain  │              │ NexaCRM.Infrastructure │
│  Aggregates      │◄─implements──│ Repositories           │
│  Value Objects   │              │ EF Core · JWT · Jobs   │
│  Domain Events   │              │ SignalR · Serilog       │
│  Interfaces      │              └───────────────────────┘
└──────────────────┘
```

### Key Patterns

- **Clean Architecture** — dependencies point strictly inward; Domain has zero NuGet dependencies
- **DDD** — Aggregates enforce all business invariants; state transitions validated inside the domain
- **CQRS** — every feature is a Command or Query; no shared read/write models
- **MediatR Pipeline** — Logging → Validation → Audit → Handler (in that order)
- **Result Pattern** — handlers return `Result<T>` instead of throwing for business failures
- **Domain Events** — published after `SaveChanges` via `UnitOfWork`; handlers are fully decoupled

---

## Technology Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Language | C# 12 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Messaging | MediatR 12 |
| Validation | FluentValidation |
| Auth | JWT Bearer + BCrypt.Net |
| Real-time | ASP.NET Core SignalR |
| Background Jobs | Hangfire (SQL Server backed) |
| Logging | Serilog (Console + File sinks) |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit + Moq |

---

## Project Structure

```
NexaCRM/
├── NexaCRM.Domain/
│   ├── Aggregates/
│   │   ├── Leads/          Lead, LeadNote, LeadStatus
│   │   ├── Deals/          Deal, DealStage
│   │   ├── Activities/     Activity, ActivityType
│   │   ├── Tasks/          CrmTask, TaskPriority
│   │   ├── Contacts/       Contact
│   │   ├── Notifications/  Notification, NotificationType
│   │   ├── Users/          User, RefreshToken, UserRole
│   │   └── Audit/          AuditLog, AuditAction
│   ├── Common/             BaseEntity, IDomainEvent
│   ├── Events/             All domain events (INotification wrappers)
│   ├── Exceptions/         DomainException, InvalidStatusTransitionException
│   ├── Repositories/       All repository interfaces + IUnitOfWork
│   ├── Services/           Domain service interfaces
│   └── ValueObjects/       ContactInfo, MonetaryValue
│
├── NexaCRM.Application/
│   ├── Behaviors/          LoggingBehavior, ValidationBehavior, AuditBehavior
│   ├── Common/             Result<T>, PaginatedResult<T>, DomainEventNotification
│   ├── Contracts/          IJwtService, ICurrentUserService, INotificationService ...
│   ├── DTOs/               All response DTOs
│   ├── EventHandlers/      Domain event → Notification handlers
│   ├── Features/           CQRS organized by feature
│   │   ├── Auth/
│   │   ├── Leads/
│   │   ├── Deals/
│   │   ├── Activities/
│   │   ├── Tasks/
│   │   ├── Contacts/
│   │   ├── Notifications/
│   │   ├── Reports/
│   │   └── AuditLogs/
│   └── Mappings/           Extension methods: entity → DTO
│
├── NexaCRM.Infrastructure/
│   ├── Auth/               JwtService, PasswordHasher, CurrentUserService
│   ├── Hubs/               NotificationHub (SignalR)
│   ├── Jobs/               TaskReminderJob (Hangfire)
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/ EF Core Fluent API per entity
│   │   ├── Migrations/
│   │   └── Repositories/   All repository + query repository implementations
│   └── Services/           NotificationService, AuditService
│
└── NexaCRM.API/
    ├── Controllers/        One controller per resource
    ├── Middleware/          ExceptionHandlingMiddleware
    └── Program.cs
```

---

## Resources

### Lead
The central aggregate. Represents a potential customer entering the sales funnel. Moves through a **strict state machine** — no stage can be skipped.

```
New → Contacted → Qualified → Negotiation → Won
                                          → Lost
Lost → New  (re-open)
```

Business rules enforced in the domain:
- Won leads cannot be reassigned, edited, or deleted
- Status can only move forward (except re-open from Lost)
- Conversion to Deal requires Qualified or Negotiation status

---

### Deal
Created when a Lead is converted. Represents a qualified commercial opportunity with a monetary value. Tracks its own stage progression independently of the Lead.

```
Proposal → Negotiation → Won
                       → Lost
```

Includes **optimistic concurrency** (RowVersion) to prevent two users moving the same Deal simultaneously.

---

### Activity
An **immutable** log entry of a real-world interaction. Once created, it cannot be edited or deleted.

Types: `Call`, `Email`, `Meeting`, `Note`

Must be linked to at least one Lead or Deal.

---

### Task
A future action item assigned to a user. Has priority, due date, and completion tracking. Triggers automatic reminders via Hangfire when approaching due date.

Priority levels: `Low`, `Medium`, `High`, `Urgent`

Must be linked to at least one Lead or Deal.

---

### Contact
A rich profile for a person associated with a Lead or Deal. Captures company, job title, LinkedIn, and address in addition to contact info. One contact can be linked to multiple Leads.

---

### Notification
Created automatically when significant domain events occur (lead moved, task assigned, deal won). Delivered in real-time via SignalR and stored in DB for the inbox.

---

### Audit Log
An append-only record of every write operation (create, update, delete, assign, convert). Written automatically by the MediatR `AuditBehavior` — no manual calls needed.

---

### Report
Aggregated data views: conversion rates, pipeline value per stage, sales rep performance, activity breakdowns. All queries are DB-level aggregations.

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (local or Docker)
- Visual Studio 2022 / Rider / VS Code

### Docker (SQL Server)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPass123!" \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

### Setup

```bash
git clone https://github.com/mo7ammedabdelmoneim/Nexa-CRM.git
cd NexaCRM

# Apply migrations
dotnet ef database update --project NexaCRM.Infrastructure --startup-project NexaCRM.API

# Run
dotnet run --project NexaCRM.API
```

### Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "Default": "Server=.;Database=NexaCRM_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "NexaCRM-Super-Secret-Key-2025-Must-Be-32-Chars!!",
    "Issuer": "NexaCRM.API",
    "Audience": "NexaCRM.Client",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  }
}
```

### URLs

| Service | URL |
|---|---|
| Swagger UI | `https://localhost:{port}/swagger` |
| Hangfire Dashboard | `https://localhost:{port}/jobs` |
| SignalR Hub | `wss://localhost:{port}/hubs/notifications` |
| Log files | `./logs/nexacrm-YYYYMMDD.log` |

---

## API Reference

> All endpoints except `POST /api/auth/login` and `POST /api/auth/register` require a `Bearer` token in the `Authorization` header.

---

### Auth

#### `POST /api/auth/register`
Register a new user.

```json
// Request
{
  "email": "ahmed@nexacrm.dev",
  "password": "Admin123!",
  "fullName": "Ahmed Hassan",
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": "Admin"  // Admin | Manager | SalesRep | Viewer
}

// Response 200
{
  "id": "uuid",
  "email": "ahmed@nexacrm.dev",
  "fullName": "Ahmed Hassan",
  "role": "Admin",
  "tenantId": "uuid"
}
```

---

#### `POST /api/auth/login`
Authenticate and receive token pair.

```json
// Request
{
  "email": "ahmed@nexacrm.dev",
  "password": "Admin123!",
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}

// Response 200
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "base64string...",
  "expiresAt": "2026-05-24T11:00:00Z",
  "user": {
    "id": "uuid",
    "email": "ahmed@nexacrm.dev",
    "fullName": "Ahmed Hassan",
    "role": "Admin",
    "tenantId": "uuid"
  }
}
```

> Store `accessToken` and `refreshToken` in memory (not localStorage). Send `accessToken` as `Authorization: Bearer {token}` on every request.

---

#### `POST /api/auth/refresh`
Rotate the refresh token. Call this automatically when `accessToken` expires (401 response).

```json
// Request
{ "token": "base64refreshtoken..." }

// Response 200 — same shape as login
```

---

#### `POST /api/auth/revoke`
Logout — invalidate refresh token.

```json
// Request
{ "token": "base64refreshtoken..." }

// Response 204 No Content
```

---

#### `GET /api/auth/me`
Get current authenticated user profile.

```json
// Response 200
{
  "id": "uuid",
  "email": "ahmed@nexacrm.dev",
  "fullName": "Ahmed Hassan",
  "role": "Admin",
  "tenantId": "uuid"
}
```

---

### Leads

#### `POST /api/leads`
Create a new lead. Status starts as `New` automatically.

```json
// Request
{
  "name": "Sara Mohamed",
  "email": "sara@company.com",
  "source": "Web",       // Web | Referral | Cold Call | Event | Social
  "phone": "01012345678",
  "assignedToUserId": null  // optional
}

// Response 201 Created
{
  "id": "uuid",
  "name": "Sara Mohamed",
  "email": "sara@company.com",
  "phone": "01012345678",
  "status": "New",
  "source": "Web",
  "assignedToUserId": null,
  "tenantId": "uuid",
  "createdAt": "2026-05-24T10:00:00Z",
  "updatedAt": null
}
```

---

#### `GET /api/leads`
List leads with filtering and pagination.

```
GET /api/leads?status=Qualified&source=Web&page=1&pageSize=20
GET /api/leads?assignedToUserId={uuid}
GET /api/leads?status=New&sortBy=createdAt
```

```json
// Response 200
{
  "items": [ { ...LeadSummaryDto } ],
  "totalCount": 48,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

#### `GET /api/leads/{id}`
Get full lead details including notes and linked activities.

```json
// Response 200 — full LeadDto
```

---

#### `PUT /api/leads/{id}`
Update lead details (name, email, source, phone). Cannot update status here.

```json
// Request
{
  "name": "Sara Mohamed",
  "email": "sara.updated@company.com",
  "source": "Referral",
  "phone": "01098765432"
}
// Response 204 No Content
```

---

#### `PATCH /api/leads/{id}/status`
Advance lead to next status. The domain enforces transition rules — invalid transitions return `400`.

```json
// Request
{ "newStatus": "Contacted" }

// Valid transitions:
// New → Contacted
// Contacted → Qualified | Lost
// Qualified → Negotiation | Lost
// Negotiation → Won | Lost
// Lost → New

// Response 204 No Content
// Response 400 if invalid: { "error": "Invalid status transition from 'New' to 'Won'." }
```

---

#### `PATCH /api/leads/{id}/assign`
Assign or reassign lead to a sales rep. Fires `LeadAssignedEvent` → notification.

```json
// Request
{ "assignedToUserId": "uuid" }
// Response 204 No Content
```

---

#### `POST /api/leads/{id}/convert`
Convert a Qualified or Negotiation lead into a Deal. Creates the Deal and marks the lead as Won.

```json
// Request
{
  "title": "Sara Mohamed — Enterprise Deal",
  "amount": 25000,
  "currency": "USD",
  "closeDate": "2026-06-30T00:00:00Z",
  "assignedToUserId": "uuid"
}

// Response 201 Created — DealDto
```

---

#### `POST /api/leads/{id}/notes`
Add an immutable note to a lead.

```json
// Request
{ "content": "Client confirmed budget is approved." }
// Response 200
```

---

#### `GET /api/leads/{id}/notes`
Get all notes for a lead.

---

#### `GET /api/leads/{id}/activities`
Get all activities logged against a lead.

---

#### `GET /api/leads/{id}/tasks`
Get all tasks linked to a lead.

---

#### `DELETE /api/leads/{id}`
Soft delete. Won leads cannot be deleted.

```
// Response 204 No Content
// Response 400 if Won: { "error": "Cannot delete a won lead." }
```

---

### Pipeline / Deals

#### `GET /api/pipeline`
Pipeline overview — all stages with deal count and total value. Used to render the Kanban board.

```
GET /api/pipeline
GET /api/pipeline?assignedToUserId={uuid}
```

```json
// Response 200
[
  { "stage": "Proposal",    "dealCount": 8,  "totalValue": 120000, "currency": "USD" },
  { "stage": "Negotiation", "dealCount": 3,  "totalValue": 85000,  "currency": "USD" },
  { "stage": "Won",         "dealCount": 5,  "totalValue": 200000, "currency": "USD" },
  { "stage": "Lost",        "dealCount": 2,  "totalValue": 0,      "currency": "USD" }
]
```

---

#### `GET /api/pipeline/deals`
List deals with filtering.

```
GET /api/pipeline/deals?stage=Proposal&minValue=10000&maxValue=50000
GET /api/pipeline/deals?assignedToUserId={uuid}&page=2
```

---

#### `GET /api/pipeline/deals/{id}`
Get a single deal by ID.

---

#### `POST /api/pipeline/deals`
Create a deal directly (without converting a lead).

```json
// Request
{
  "leadId": "uuid",
  "title": "Enterprise License Deal",
  "amount": 50000,
  "currency": "USD",
  "closeDate": "2026-07-01T00:00:00Z",
  "assignedToUserId": "uuid"
}
// Response 201 Created — DealDto
```

---

#### `PATCH /api/pipeline/deals/{id}/stage`
Move a deal to the next stage.

```json
// Request
{ "newStage": "Negotiation" }
// Proposal → Negotiation → Won | Lost
// Response 204
```

---

#### `PUT /api/pipeline/deals/{id}`
Update deal details (title, value, close date).

```json
// Request
{
  "title": "Updated Deal Name",
  "amount": 60000,
  "currency": "USD",
  "closeDate": "2026-08-01T00:00:00Z"
}
// Response 204
```

---

#### `PATCH /api/pipeline/deals/{id}/assign`
Reassign deal to another sales rep.

```json
// Request
{ "assignedToUserId": "uuid" }
// Response 204
```

---

### Activities

#### `POST /api/activities`
Log an activity. Append-only — cannot be edited or deleted.

```json
// Request
{
  "type": "Call",   // Call | Email | Meeting | Note
  "description": "Discussed pricing and enterprise features for 30 mins.",
  "occurredAt": "2026-05-24T10:00:00Z",
  "leadId": "uuid",    // at least one required
  "dealId": null
}
// Response 200 — ActivityDto
```

---

#### `GET /api/activities`
List activities with filtering.

```
GET /api/activities?leadId={uuid}&type=Call&from=2026-05-01&to=2026-05-31
GET /api/activities?dealId={uuid}&page=1&pageSize=20
```

---

### Tasks

#### `POST /api/tasks`
Create a task linked to a lead or deal.

```json
// Request
{
  "title": "Send follow-up proposal to Sara",
  "priority": "High",     // Low | Medium | High | Urgent
  "assignedToUserId": "uuid",
  "dueDate": "2026-05-27T09:00:00Z",
  "leadId": "uuid",
  "dealId": null
}
// Response 200 — TaskDto
```

---

#### `GET /api/tasks`
List tasks with filtering.

```
GET /api/tasks?isCompleted=false&priority=High
GET /api/tasks?assignedToUserId={uuid}&dueBefore=2026-05-31
```

---

#### `GET /api/tasks/overdue`
Get all incomplete tasks past their due date.

---

#### `PUT /api/tasks/{id}`
Update task title, priority, or due date.

```json
// Request
{
  "title": "Send revised proposal",
  "priority": "Urgent",
  "dueDate": "2026-05-26T09:00:00Z"
}
// Response 204
```

---

#### `PATCH /api/tasks/{id}/complete`
Mark a task as done.

```
// Response 204
// Response 400 if already completed: { "error": "Task is already completed." }
```

---

#### `DELETE /api/tasks/{id}`
Soft delete a task.

---

### Contacts

#### `POST /api/contacts`
Create a contact profile.

```json
// Request
{
  "firstName": "Sara",
  "lastName": "Mohamed",
  "email": "sara@company.com",
  "phone": "01012345678",
  "company": "Tech Corp",
  "jobTitle": "CTO",
  "linkedIn": "https://linkedin.com/in/saramohamed",
  "address": "Cairo, Egypt",
  "leadId": "uuid"   // optional link
}
// Response 201 — ContactDto
```

---

#### `GET /api/contacts`
Search and list contacts.

```
GET /api/contacts?search=sara
GET /api/contacts?company=Tech
GET /api/contacts?leadId={uuid}
```

---

#### `GET /api/contacts/{id}`
Get full contact details.

---

#### `PUT /api/contacts/{id}`
Update contact profile.

---

#### `PATCH /api/contacts/{id}/link-lead`
Link an existing contact to a lead.

```json
// Request
{ "leadId": "uuid" }
// Response 204
```

---

#### `DELETE /api/contacts/{id}`
Soft delete a contact.

---

### Notifications

#### `GET /api/notifications`
Get current user's notification inbox.

```
GET /api/notifications?isRead=false&page=1&pageSize=20
```

```json
// Response 200
{
  "items": [
    {
      "id": "uuid",
      "message": "Lead 'Sara Mohamed' moved from New to Contacted.",
      "type": "LeadStatusChanged",
      "isRead": false,
      "entityId": "uuid",
      "entityType": "Lead",
      "createdAt": "2026-05-24T10:05:00Z"
    }
  ],
  "totalCount": 12,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

---

#### `GET /api/notifications/unread-count`
Badge count for the notification bell.

```json
// Response 200
{ "count": 7 }
```

---

#### `PATCH /api/notifications/{id}/read`
Mark one notification as read.

```
// Response 204
```

---

#### `PATCH /api/notifications/read-all`
Mark all notifications as read.

```
// Response 204
```

---

### Reports

All report endpoints support optional date range via `?from=` and `?to=` query params.

---

#### `GET /api/reports/conversion-rate`
Lead conversion rate over a period.

```
GET /api/reports/conversion-rate?from=2026-01-01&to=2026-05-31
```

```json
{
  "totalLeads": 80,
  "wonLeads": 18,
  "conversionRate": 22.5,
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-05-31T00:00:00Z"
}
```

---

#### `GET /api/reports/pipeline-value`
Current pipeline value grouped by stage.

```json
{
  "stages": [
    { "stage": "Proposal",    "dealCount": 8,  "totalValue": 120000, "currency": "USD" },
    { "stage": "Negotiation", "dealCount": 3,  "totalValue": 85000,  "currency": "USD" },
    { "stage": "Won",         "dealCount": 12, "totalValue": 430000, "currency": "USD" },
    { "stage": "Lost",        "dealCount": 4,  "totalValue": 0,      "currency": "USD" }
  ],
  "totalValue": 635000,
  "currency": "USD"
}
```

---

#### `GET /api/reports/sales-rep-performance`
Per-rep metrics.

```json
[
  {
    "userId": "uuid",
    "leadsHandled": 24,
    "dealsWon": 7,
    "totalValue": 185000,
    "avgCloseDays": 18.4
  }
]
```

---

#### `GET /api/reports/lead-sources`
Breakdown of leads by acquisition source.

```json
[
  { "source": "Web",      "count": 35, "percentage": 43.75 },
  { "source": "Referral", "count": 25, "percentage": 31.25 },
  { "source": "Cold Call","count": 20, "percentage": 25.0  }
]
```

---

#### `GET /api/reports/activity-summary`
Count of activities by type over a date range.

```json
[
  { "type": "Call",    "count": 42 },
  { "type": "Email",   "count": 38 },
  { "type": "Meeting", "count": 15 },
  { "type": "Note",    "count": 27 }
]
```

---

### Audit Logs

#### `GET /api/audit-logs`
Search audit history with full filtering.

```
GET /api/audit-logs?entityType=Lead&action=StatusChanged
GET /api/audit-logs?userId={uuid}&from=2026-05-01&to=2026-05-31
```

```json
{
  "items": [
    {
      "id": "uuid",
      "entityType": "Lead",
      "entityId": "uuid",
      "action": "StatusChanged",
      "oldValues": null,
      "newValues": "{\"LeadId\":\"...\",\"NewStatus\":\"Contacted\"}",
      "userId": "uuid",
      "ipAddress": "127.0.0.1",
      "timestamp": "2026-05-24T10:05:00Z"
    }
  ]
}
```

---

#### `GET /api/audit-logs/entity/{entityType}/{entityId}`
Get all audit history for a specific entity.

```
GET /api/audit-logs/entity/Lead/{leadId}
GET /api/audit-logs/entity/Deal/{dealId}
GET /api/audit-logs/entity/Contact/{contactId}
```

---

#### `GET /api/audit-logs/user/{userId}`
Get all actions performed by a specific user.

---

## Full Walkthrough

This walkthrough uses real data values and follows the natural sales workflow. Execute these requests in order to see the system working end-to-end.

---

### Step 1 — Register and Login

```bash
# Register admin user
POST /api/auth/register
{
  "email": "admin@nexacrm.dev",
  "password": "Admin123!",
  "fullName": "Ahmed Hassan",
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": "Admin"
}

# Register a sales rep
POST /api/auth/register
{
  "email": "omar@nexacrm.dev",
  "password": "Sales123!",
  "fullName": "Omar Khaled",
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": "SalesRep"
}

# Login as admin → save accessToken + refreshToken
POST /api/auth/login
{
  "email": "admin@nexacrm.dev",
  "password": "Admin123!",
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
# → save: accessToken, refreshToken, user.id (adminId)
# → save: user.id from the register of omar (omarId)
```

---

### Step 2 — Create a Lead

```bash
POST /api/leads
Authorization: Bearer {accessToken}
{
  "name": "Sara Mohamed",
  "email": "sara@techcorp.com",
  "source": "Web",
  "phone": "01012345678",
  "assignedToUserId": "{omarId}"
}
# → save: id as {leadId}
# SignalR: Omar receives "lead.assigned" notification
```

---

### Step 3 — Create Contact for the Lead

```bash
POST /api/contacts
{
  "firstName": "Sara",
  "lastName": "Mohamed",
  "email": "sara@techcorp.com",
  "phone": "01012345678",
  "company": "Tech Corp",
  "jobTitle": "CTO",
  "linkedIn": "https://linkedin.com/in/saramohamed",
  "leadId": "{leadId}"
}
# → save: id as {contactId}
```

---

### Step 4 — Log First Activity

```bash
POST /api/activities
{
  "type": "Call",
  "description": "Introduction call. Sara confirmed interest in the enterprise plan. Budget approved.",
  "occurredAt": "2026-05-24T10:00:00Z",
  "leadId": "{leadId}"
}
```

---

### Step 5 — Create a Follow-up Task

```bash
POST /api/tasks
{
  "title": "Send enterprise pricing proposal to Sara",
  "priority": "High",
  "assignedToUserId": "{omarId}",
  "dueDate": "2026-05-26T09:00:00Z",
  "leadId": "{leadId}"
}
# → save: id as {taskId}
# SignalR: Omar receives "task.assigned" notification
```

---

### Step 6 — Advance the Lead Through Pipeline

```bash
# New → Contacted
PATCH /api/leads/{leadId}/status
{ "newStatus": "Contacted" }
# SignalR: Omar receives "lead.status_changed" notification

# Log another activity
POST /api/activities
{
  "type": "Email",
  "description": "Sent enterprise pricing PDF and scheduling link.",
  "occurredAt": "2026-05-25T14:00:00Z",
  "leadId": "{leadId}"
}

# Contacted → Qualified
PATCH /api/leads/{leadId}/status
{ "newStatus": "Qualified" }

# Qualified → Negotiation
PATCH /api/leads/{leadId}/status
{ "newStatus": "Negotiation" }
```

---

### Step 7 — Complete the Task

```bash
PATCH /api/tasks/{taskId}/complete
# Response 204
```

---

### Step 8 — Convert Lead to Deal

```bash
POST /api/leads/{leadId}/convert
{
  "title": "Tech Corp — Enterprise License",
  "amount": 48000,
  "currency": "USD",
  "closeDate": "2026-06-30T00:00:00Z",
  "assignedToUserId": "{omarId}"
}
# → save: id as {dealId}
# Lead status → Won
# DealCreatedEvent fired
```

---

### Step 9 — View the Pipeline

```bash
GET /api/pipeline
# → Tech Corp deal appears in "Proposal" stage

GET /api/pipeline/deals?stage=Proposal
```

---

### Step 10 — Move Deal Through Stages

```bash
# Proposal → Negotiation
PATCH /api/pipeline/deals/{dealId}/stage
{ "newStage": "Negotiation" }

# Log meeting
POST /api/activities
{
  "type": "Meeting",
  "description": "Contract review meeting with Sara and her legal team. Terms agreed.",
  "occurredAt": "2026-06-15T11:00:00Z",
  "dealId": "{dealId}"
}

# Negotiation → Won
PATCH /api/pipeline/deals/{dealId}/stage
{ "newStage": "Won" }
# SignalR: Omar receives "deal.stage_changed" notification
```

---

### Step 11 — Check Notifications

```bash
GET /api/notifications/unread-count
# { "count": 4 }

GET /api/notifications?isRead=false
# Returns all unread notifications

PATCH /api/notifications/read-all
# Mark all as read
```

---

### Step 12 — View Reports

```bash
GET /api/reports/conversion-rate?from=2026-05-01&to=2026-05-31
# { "totalLeads": 1, "wonLeads": 1, "conversionRate": 100 }

GET /api/reports/pipeline-value
# Won stage now shows $48,000

GET /api/reports/sales-rep-performance
# Omar: 1 lead handled, 1 deal won, $48,000 total value

GET /api/reports/lead-sources
# Web: 100%

GET /api/reports/activity-summary?from=2026-05-01&to=2026-06-30
# Call: 1, Email: 1, Meeting: 1
```

---

### Step 13 — Audit Trail

```bash
# Full history of the lead
GET /api/audit-logs/entity/Lead/{leadId}

# Full history of the deal
GET /api/audit-logs/entity/Deal/{dealId}

# Everything Omar did
GET /api/audit-logs/user/{omarId}
```

---

### Step 14 — Token Refresh (automatic in frontend)

```bash
# When accessToken expires → call refresh before retrying
POST /api/auth/refresh
{ "token": "{refreshToken}" }
# → new accessToken + new refreshToken (old one is now invalid)
```

---

### Step 15 — Logout

```bash
POST /api/auth/revoke
{ "token": "{refreshToken}" }
# Response 204 — token is now invalidated server-side
```

---

## Real-Time Events (SignalR)

### Connection

```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/notifications?userId={userId}", {
    accessTokenFactory: () => accessToken
  })
  .withAutomaticReconnect()
  .build();

await connection.start();
```

### Events

| Event | Trigger | Payload |
|---|---|---|
| `notification.new` | Any notification created | `{ id, message, type, entityId, entityType, createdAt }` |
| `lead.status_changed` | Lead status transition | via notification |
| `lead.assigned` | Lead assigned to user | via notification |
| `task.assigned` | Task assigned to user | via notification |
| `task.overdue` | Task passes due date | via Hangfire job |
| `deal.stage_changed` | Deal moves to new stage | via notification |

### Listening

```javascript
connection.on("notification.new", (data) => {
  // Update badge count
  // Show toast
  // Refresh relevant list
  console.log(data.message);
});
```

---

## Background Jobs (Hangfire)

| Job | Schedule | Description |
|---|---|---|
| `task-reminder` | Every hour | Finds tasks due within 24 hours and sends reminder notifications |
| `task-overdue` | Daily at 7am | Marks tasks past due date and fires overdue notifications |
| `cleanup-tokens` | Daily at 2am | Removes expired and revoked refresh tokens |

Dashboard: `https://localhost:{port}/jobs`

---

## Response Contracts

### Success — Single Resource

```json
{ ...resourceFields }
```

### Success — Paginated List

```json
{
  "items": [ ... ],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

### Error — Validation (400)

```json
{
  "status": 400,
  "error": "Validation failed",
  "details": [
    { "field": "Email", "message": "Email is not valid." },
    { "field": "Name",  "message": "Name is required." }
  ],
  "traceId": "00-abc123-def456-00"
}
```

### Error — Business Rule (400)

```json
{
  "status": 400,
  "error": "Invalid status transition from 'New' to 'Won'.",
  "traceId": "00-abc123-def456-00"
}
```

### Error — Not Found (404)

```json
{ "error": "Lead not found." }
```

### Error — Unauthorized (401)

```json
{ "error": "Invalid email or password." }
```

### Error — Server (500)

```json
{
  "status": 500,
  "error": "An unexpected error occurred.",
  "traceId": "00-abc123-def456-00"
}
```

---

## Frontend Integration Guide

### Token Management

```typescript
// Store in memory — never localStorage/sessionStorage
let accessToken: string | null = null;
let refreshToken: string | null = null;

// Axios interceptor — auto-refresh on 401
axios.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401 && refreshToken) {
      const res = await axios.post('/api/auth/refresh', { token: refreshToken });
      accessToken  = res.data.accessToken;
      refreshToken = res.data.refreshToken;
      error.config.headers['Authorization'] = `Bearer ${accessToken}`;
      return axios(error.config);
    }
    return Promise.reject(error);
  }
);
```

---

### Pagination Pattern

```typescript
interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

// Usage
const { data } = await api.get<PaginatedResult<Lead>>(
  '/api/leads', { params: { page, pageSize: 20, status } }
);
```

---

### State Machine Guards (UI)

Disable action buttons based on lead/deal status:

```typescript
const leadStatusFlow: Record<string, string[]> = {
  New:         ['Contacted'],
  Contacted:   ['Qualified', 'Lost'],
  Qualified:   ['Negotiation', 'Lost'],
  Negotiation: ['Won', 'Lost'],
  Won:         [],
  Lost:        ['New'],
};

const allowedNext = leadStatusFlow[lead.status] ?? [];
// Disable buttons not in allowedNext
// Disable edit/delete/assign if status === 'Won'
```

---

### Notification Badge

```typescript
// Poll on page load, then rely on SignalR for live updates
const { data } = await api.get<{ count: number }>('/api/notifications/unread-count');
setBadgeCount(data.count);

// SignalR — increment badge in real-time
connection.on('notification.new', () => {
  setBadgeCount(prev => prev + 1);
});
```

---

### Error Handling

```typescript
try {
  await api.patch(`/api/leads/${id}/status`, { newStatus });
} catch (error) {
  if (axios.isAxiosError(error)) {
    const msg = error.response?.data?.error ?? 'Something went wrong.';
    // Show toast with msg
    // e.g. "Invalid status transition from 'New' to 'Won'."
  }
}
```

---

### Recommended Queries per Screen

| Screen | Endpoints |
|---|---|
| Dashboard | `GET /api/reports/pipeline-value`, `GET /api/reports/conversion-rate`, `GET /api/notifications/unread-count` |
| Leads List | `GET /api/leads` |
| Lead Detail | `GET /api/leads/{id}`, `GET /api/leads/{id}/activities`, `GET /api/leads/{id}/tasks`, `GET /api/leads/{id}/notes` |
| Pipeline Board | `GET /api/pipeline`, `GET /api/pipeline/deals` |
| Contacts | `GET /api/contacts` |
| Tasks | `GET /api/tasks?isCompleted=false`, `GET /api/tasks/overdue` |
| Notifications | `GET /api/notifications`, `GET /api/notifications/unread-count` |
| Reports | All `GET /api/reports/*` |
| Admin / Audit | `GET /api/audit-logs` |

---

## License

MIT

---

*NexaCRM — Built to demonstrate production-level .NET backend engineering.*
