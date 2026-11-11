# Shipment & Container Tracker

A learning project for preparing a **Full Stack Developer** interview at a shipping company (MSC-style stack: **.NET + SQL Server + frontend**).

We build a small **Shipment & Container Tracker** in small pull requests. Each step is meant to be reviewed, learned from, and merged by you — teaching is the main goal.

## Planned roadmap

| Step | Focus |
|------|--------|
| **1** | Repo layout, SQL Server in Docker, domain entities, EF Core + Fluent API, initial migration, Swagger + `/health` |
| **2** | CRUD Web API controllers/endpoints for ports, vessels, containers, shipments, tracking events |
| **3** | Advanced SQL: raw queries, CTEs, window functions, a stored procedure, indexes |
| **4** | Validation, consistent error handling, structured logging |
| **5** | JWT authentication & authorization |
| **6** | xUnit tests (unit + integration) |
| **7** | React (TypeScript, Vite) frontend |
| **8** | Docker Compose full stack + GitHub Actions CI |

This PR is **step 1 only**.

## Why this solution layout?

```
backend/
  ShipmentTracker.sln
  src/
    ShipmentTracker.Domain/          # Entities + enums (no EF / ASP.NET references)
    ShipmentTracker.Infrastructure/  # DbContext, Fluent API, migrations, seed
    ShipmentTracker.Api/             # Program.cs, Swagger, health — hosts the app
frontend/                            # Placeholder until step 7
docs/                                # Teaching notes per step
```

**Api / Domain / Infrastructure** is a lightweight clean-ish layering used in many .NET shops:

- **Domain** stays framework-free so you can reason about the shipping model without SQL or HTTP noise.
- **Infrastructure** owns persistence concerns (EF Core, SQL Server).
- **Api** is the composition root: DI, middleware, endpoints.

A single project would work for a tiny demo, but this split matches interview conversations about separation of concerns and scales cleanly into steps 2–6.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/) + Docker Compose
- Optional: EF Core CLI — `dotnet tool install --global dotnet-ef --version 8.0.11`

## Run locally (step 1)

```bash
# 1) Clone and enter the repo
cd shipment-tracker   # or your local folder name

# 2) Create a local env file from the example (gitignored)
cp .env.example .env
# Edit .env if you want a different SA password — then mirror it in
# backend/src/ShipmentTracker.Api/appsettings.json ConnectionStrings:DefaultConnection

# 3) Start SQL Server 2022
docker compose up -d

# Wait until healthy (optional check)
docker compose ps

# 4) Apply EF Core migrations (creates DB + seed data)
cd backend
dotnet ef database update \
  --project src/ShipmentTracker.Infrastructure \
  --startup-project src/ShipmentTracker.Api

# 5) Run the API
cd src/ShipmentTracker.Api
dotnet run --launch-profile http
```

Then open:

- Swagger UI: [http://localhost:5080/swagger](http://localhost:5080/swagger)
- Health: [http://localhost:5080/health](http://localhost:5080/health) — should return `Healthy` when SQL is reachable

### Useful commands

```bash
# Build the whole solution
dotnet build backend/ShipmentTracker.sln

# Create a new migration later (after model changes)
dotnet ef migrations add <Name> \
  --project backend/src/ShipmentTracker.Infrastructure \
  --startup-project backend/src/ShipmentTracker.Api \
  --output-dir Persistence/Migrations

# Stop SQL Server (keeps the named volume / data)
docker compose down

# Stop and wipe the database volume
docker compose down -v
```

## Seed choice (step 1)

Demo rows (ports Genoa / Rotterdam / Shanghai / Singapore, two MSC vessels, containers, shipments, tracking events) are seeded with **`HasData`** inside the initial migration.

**Why HasData here:** it travels with the migration, so `dotnet ef database update` on a fresh machine always produces the same demo data — great for learning and for JOIN examples in `docs/step-01.md`. A **runtime seeder** is better when seed data must call APIs, hash passwords, or change often without new migrations; we can switch later if needed.

## Teaching notes

Read **[docs/step-01.md](docs/step-01.md)** — written for someone who knows web development but is rusty on .NET / SQL Server.

## License

Learning / personal interview prep project.
