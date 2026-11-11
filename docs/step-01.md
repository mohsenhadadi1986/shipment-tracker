# Step 1 — Foundation: solution, EF Core, SQL Server

Audience: you already know web apps (React/Angular/APIs, TypeScript/Python), but .NET / C# / SQL Server are rusty. This note is practical — enough to review the PR with confidence and talk about it in an interview.

---

## 1. Solution / project structure (and why)

```
backend/
  ShipmentTracker.sln
  src/
    ShipmentTracker.Domain/           # Pure C# model
    ShipmentTracker.Infrastructure/   # EF Core + SQL Server
    ShipmentTracker.Api/              # HTTP host (Program.cs)
frontend/                             # Empty until step 7
docs/
docker-compose.yml
.env.example
```

| Project | Responsibility | Depends on |
|---------|----------------|------------|
| **Domain** | Entities (`Port`, `Vessel`, …) and enums. No NuGet packages for EF or ASP.NET. | Nothing |
| **Infrastructure** | `DbContext`, Fluent API configs, migrations, seed data, DI registration for SQL Server. | Domain |
| **Api** | Composition root: reads config, registers services, middleware, Swagger, `/health`. | Infrastructure (+ Domain) |

**Why not one big project?** A single Web API project is fine for tiny demos. Splitting teaches the interview answer: *domain model should not know about SQL or HTTP*. Controllers (step 2) will talk to the database through Infrastructure; Domain stays portable.

**Mental map to stacks you know:**

| Concept you know | Here |
|------------------|------|
| NestJS module / Spring `@Configuration` | `Program.cs` + `AddInfrastructure(...)` |
| Prisma / TypeORM / SQLAlchemy model | EF Core entity + `DbContext` |
| Alembic / Flyway / Prisma migrate | EF Core migrations |
| `docker-compose` Postgres | Same idea — SQL Server container |

---

## 2. How ASP.NET Core startup and DI work (`Program.cs`)

Modern ASP.NET Core (.NET 6+) uses a **minimal hosting model** — one `Program.cs`, no `Startup.cs`.

Rough flow:

1. **`WebApplication.CreateBuilder(args)`**  
   Loads configuration (`appsettings.json`, `appsettings.{Environment}.json`, environment variables, command-line). Builds a **DI container** (`builder.Services`).

2. **Register services** (before `Build()`):
   ```csharp
   builder.Services.AddSwaggerGen(...);
   builder.Services.AddInfrastructure(builder.Configuration); // our extension
   builder.Services.AddHealthChecks().AddDbContextCheck<ShipmentTrackerDbContext>(...);
   ```
   “Register” means: *when something asks for `ShipmentTrackerDbContext`, create one with these options*. Lifetime for `AddDbContext` is **scoped** (one instance per HTTP request).

3. **`var app = builder.Build()`**  
   Freezes the container and builds the middleware pipeline.

4. **Middleware + endpoints**:
   ```csharp
   app.UseSwagger();
   app.MapHealthChecks("/health");
   app.MapGet("/", () => Results.Ok(...));
   app.Run();
   ```

**Dependency Injection** is the same idea as Angular constructors or NestJS `@Inject`: you ask for an abstraction/type; the container provides it. You almost never `new ShipmentTrackerDbContext(...)` yourself in controllers.

Connection string lives in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=ShipmentTracker;User Id=sa;Password=...;TrustServerCertificate=True;Encrypt=True"
}
```

`TrustServerCertificate=True` is normal for local Docker SQL Server (dev cert). Do **not** copy that blindly to production without a real TLS story.

---

## 3. EF Core in 5 minutes

**Entity Framework Core** is Microsoft’s ORM for .NET (think Prisma/TypeORM, but with a stronger “change tracker” culture).

| Piece | Role |
|-------|------|
| **Entity** | Plain C# class mapped to a table (`Port`, `Shipment`, …). |
| **`DbContext`** | Session with the database: `DbSet<T>` properties, change tracking, `SaveChanges`. |
| **Provider** | `Microsoft.EntityFrameworkCore.SqlServer` — translates LINQ to T-SQL. |
| **Fluent API** | `IEntityTypeConfiguration<T>` — keys, lengths, indexes, relationships, delete behavior. |
| **Migration** | Versioned C# class that creates/alters SQL schema (and can insert seed rows). |

### Fluent API vs data annotations

You *can* put `[Required]`, `[MaxLength(200)]`, `[ForeignKey]` on entity properties (**data annotations**). We used **Fluent API** classes under `Infrastructure/Persistence/Configurations/` instead.

| | Fluent API | Data annotations |
|--|------------|------------------|
| Keeps Domain free of EF attributes | Yes | No (attributes live on entities) |
| Handles complex relationships cleanly | Excellent (two FKs to `Port`, composite keys, delete behaviors) | Gets awkward fast |
| Discoverable in one place | `ApplyConfigurationsFromAssembly` | Scattered on classes |

Interview-friendly answer: *“Annotations are fine for simple apps; Fluent API scales better and keeps the domain model clean.”*

### Enums

Statuses and container types are C# enums. We store them as **strings** in SQL (`HasConversion<string>()`) so SSMS queries stay readable (`'InTransit'` vs `1`). Integers are slightly cheaper/faster; strings are friendlier for learning and support tools.

---

## 4. Domain relationships → SQL Server tables

### Conceptual model

```
Port 1───* Shipment (as Origin)
Port 1───* Shipment (as Destination)
Vessel 1───* Shipment
Vessel 1───* Container (CurrentVessel, optional)

Shipment *───* Container
        └── via ShipmentContainer (composite PK + SealNumber)

Shipment 1───* TrackingEvent
Container 0..1───* TrackingEvent
Port 0..1───* TrackingEvent
```

### Tables generated by `InitialCreate`

| Table | PK | Notable columns / FKs | Delete behavior |
|-------|----|------------------------|-----------------|
| `Ports` | `Id` | Unique `UnLocode` (nchar(5)) | — |
| `Vessels` | `Id` | Unique `ImoNumber` (nchar(7)) | — |
| `Containers` | `Id` | Unique `ContainerNumber`; FK `CurrentVesselId` → `Vessels` | **SetNull** if vessel deleted |
| `Shipments` | `Id` | Unique `ReferenceNumber`; FKs `OriginPortId`, `DestinationPortId` → `Ports`; `VesselId` → `Vessels` | Ports: **Restrict**; Vessel: **SetNull** |
| `ShipmentContainers` | `(ShipmentId, ContainerId)` | Optional `SealNumber`; FKs to both parents | Shipment: **Cascade**; Container: **Restrict** |
| `TrackingEvents` | `Id` | FK `ShipmentId` (required); optional `ContainerId`, `PortId` | Shipment: **Cascade**; others: **SetNull** |

**Why Restrict on ports?** Deleting Genoa while shipments still reference it would orphan business history — SQL should refuse.  
**Why Cascade on tracking events when a shipment is deleted?** Events only make sense in the context of that shipment.  
**Why an explicit `ShipmentContainer` join entity?** Many-to-many is natural (one shipment has many containers; a container rides many shipments over its life). An explicit join lets us store `SealNumber` now and more payload later without redesigning a hidden join table.

### Example SQL against seed data

After `dotnet ef database update`, try these in Azure Data Studio, SSMS, or `sqlcmd`:

```sql
-- Shipments with origin / destination port names and vessel
SELECT
    s.ReferenceNumber,
    s.Status,
    o.Name AS OriginPort,
    d.Name AS DestinationPort,
    v.Name AS VesselName
FROM Shipments s
INNER JOIN Ports o ON o.Id = s.OriginPortId
INNER JOIN Ports d ON d.Id = s.DestinationPortId
LEFT JOIN Vessels v ON v.Id = s.VesselId;
```

```sql
-- Containers on shipment SHP-2026-0001 (many-to-many via join table)
SELECT
    s.ReferenceNumber,
    c.ContainerNumber,
    c.Type,
    c.Status,
    sc.SealNumber
FROM Shipments s
INNER JOIN ShipmentContainers sc ON sc.ShipmentId = s.Id
INNER JOIN Containers c ON c.Id = sc.ContainerId
WHERE s.ReferenceNumber = N'SHP-2026-0001';
```

```sql
-- Tracking timeline for that shipment
SELECT
    e.OccurredAt,
    e.EventType,
    e.Description,
    p.Name AS PortName,
    c.ContainerNumber
FROM TrackingEvents e
INNER JOIN Shipments s ON s.Id = e.ShipmentId
LEFT JOIN Ports p ON p.Id = e.PortId
LEFT JOIN Containers c ON c.Id = e.ContainerId
WHERE s.ReferenceNumber = N'SHP-2026-0001'
ORDER BY e.OccurredAt;
```

---

## 5. Migrations: create and apply

A **migration** is a C# class (plus a designer + model snapshot) that describes how to move the database schema from version N to N+1.

```bash
# Create (compares your model to the last snapshot → generates Up/Down)
dotnet ef migrations add InitialCreate \
  --project src/ShipmentTracker.Infrastructure \
  --startup-project src/ShipmentTracker.Api \
  --output-dir Persistence/Migrations

# Apply (runs pending Up methods against the connection string)
dotnet ef database update \
  --project src/ShipmentTracker.Infrastructure \
  --startup-project src/ShipmentTracker.Api
```

EF keeps a history table `__EFMigrationsHistory` so it knows what already ran.

**Rules of thumb:**

- Commit migrations to git (we did). Teammates / CI apply the same files.
- Don’t edit an already-applied migration on a shared DB — add a new one.
- `dotnet ef migrations remove` only if the last migration was never applied anywhere important.

### Seed: `HasData` vs runtime seeder

We used **`HasData`** on the model so the initial migration contains `INSERT` statements.

| Approach | Pros | Cons |
|----------|------|------|
| **HasData** (this step) | Reproducible with `database update`; versioned; great for demo ports/vessels | Needs fixed IDs; awkward for passwords / dynamic data |
| **Runtime seeder** (`if (!db.Ports.Any()) …` at startup) | Flexible; can hash secrets; easy to tweak without a migration | Easy to diverge across environments if not careful |

For interview prep seed data, HasData is the clearer teaching choice.

---

## 6. SQL Server in Docker (basics)

`docker-compose.yml` runs:

- Image: `mcr.microsoft.com/mssql/server:2022-latest`
- Port: `1433` (default TDS port)
- Env: `ACCEPT_EULA=Y`, `MSSQL_SA_PASSWORD` from `.env`
- Named volume `sqlserver_data` so data survives `docker compose down`
- **Healthcheck** via `sqlcmd` so you know when the engine is ready

```bash
cp .env.example .env          # then edit password if you want
docker compose up -d
docker compose ps             # wait for (healthy)
docker compose down           # stop, keep volume
docker compose down -v        # stop AND delete database files
```

If you change the password in `.env`, also update `ConnectionStrings:DefaultConnection` in `appsettings.json` (or override with an environment variable / user secrets — a later step topic).

---

## 7. What this step deliberately does *not* include

- No CRUD controllers / minimal APIs for resources → **step 2**
- No DTOs, validation attributes, ProblemDetails → **step 4**
- No auth → **step 5**
- No tests → **step 6**
- No React UI → **step 7**

You *should* be able to: build the solution, start SQL Server, apply the migration, hit `/health` and see `Healthy`, open Swagger.

---

## 8. Self-check / likely interview questions

**1. What is a `DbContext`?**  
A unit-of-work / session with the database: holds `DbSet`s, tracks entity changes, and generates SQL on `SaveChanges`. Usually registered as scoped in DI (one per HTTP request).

**2. Fluent API vs data annotations — when do you prefer Fluent API?**  
Complex relationships (two FKs to the same table), composite keys, precise delete behaviors, and keeping the domain free of persistence attributes.

**3. What does `OnDelete(DeleteBehavior.Restrict)` do?**  
SQL Server rejects deleting a parent row while children still reference it (FK violation). We use it for ports referenced by shipments.

**4. How do you model many-to-many in EF Core?**  
Either a skip navigation (`Shipment.Containers`) with a hidden join table, or an **explicit join entity** (`ShipmentContainer`) when the relationship has payload (we store `SealNumber`).

**5. What is an EF migration?**  
A versioned, code-generated change script (C#) that upgrades (and can downgrade) the database schema. Applied with `dotnet ef database update`; history is stored in `__EFMigrationsHistory`.

**6. Why put the connection string in configuration instead of hard-coding it in `DbContext`?**  
Different environments (local Docker, CI, Azure) need different servers/credentials without recompiling. Configuration + DI keeps Infrastructure testable and deployable.

**7. What does `/health` with `AddDbContextCheck` prove?**  
Not only that the process is running, but that it can open a connection through EF to SQL Server — useful for orchestrators and for your own “is my compose stack up?” check.

**8. Identity columns vs seed `HasData` IDs?**  
Tables use SQL Server `IDENTITY(1,1)`. Seed rows insert fixed IDs with `IDENTITY_INSERT` so FKs between seed rows are stable across machines.

---

## Suggested next step (preview)

**Step 2:** add CRUD endpoints (controllers or minimal APIs) for ports, vessels, containers, shipments, and tracking events — DTOs, basic filtering, and `Include` / projections so you practice LINQ that becomes real T-SQL.
