# Payroll System

Payroll management system with a .NET 10 API and an Angular admin UI. Supports Arabic (RTL) and English.

## Features

- **Employees** — CRUD with search, filters, and paging
- **Departments** — management and department incentive %
- **Salaries** — grade-based, effective-dated salary tables
- **Incentives** — years-of-service tiers
- **Attendance** — monthly absence records and absence deduction/bonus rules
- **Payroll runs** — generate, recalculate, approve; payslip snapshots
- **Reports** — attendance, employees, incentives/deductions, salaries (PDF/Excel)
- **Auth** — JWT login/refresh with **Admin** and **HR** roles

## Stack

| Layer | Technology |
|--------|------------|
| API | .NET 10, ASP.NET Core, EF Core, SQL Server, JWT, Scalar/Swagger |
| Admin UI | Angular 22, Angular Material, ngx-translate |
| Architecture | Clean Architecture (`Api` → `Application` → `Domain` / `Infrastructure`) |

## Repository layout

```
Payroll.slnx                 # Solution
Payroll.Api/                 # HTTP host, controllers, auth wiring
Payroll.Application/         # Commands, queries, validators, DTOs
Payroll.Domain/              # Entities, enums, domain services
Payroll.Infrastructure/      # EF Core, Identity, reports, seeding
payroll-admin/               # Angular admin SPA
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (npm 11+)
- SQL Server (LocalDB, Express, or full) reachable from your machine

## Quick start

### 1. Clone

```bash
git clone https://github.com/mahmoudadel42/payroll-system.git
cd payroll-system
```

### 2. Configure the API

Copy the development settings template and fill in local values:

```bash
cp Payroll.Api/appsettings.Development.example.json Payroll.Api/appsettings.Development.json
```

Edit `Payroll.Api/appsettings.Development.json`:

- `ConnectionStrings:Default` — your SQL Server connection string
- `Jwt:Key` — a long random secret (32+ characters)
- `Seed:AdminPassword` / `Seed:HrPassword` — initial user passwords
- `Cors:AllowedOrigins` — include `http://localhost:4200` for the Angular app

On startup (when `Database:InitializeOnStartup` is `true`), the API applies migrations and seeds roles/users. With `Seed:SampleData` set to `true`, sample master data is also created.

Default seeded usernames:

| User  | Role  |
|-------|-------|
| `admin` | Admin |
| `hr`    | HR    |

### 3. Run the API

```bash
dotnet run --project Payroll.Api
```

- API: `http://localhost:5276`
- Swagger UI: `http://localhost:5276/swagger`

### 4. Run the admin UI

```bash
cd payroll-admin
npm install
npm start
```

Open `http://localhost:4200`. The Angular dev server proxies `/api` to `http://localhost:5276` (see `payroll-admin/proxy.conf.json`).

## Useful commands

```bash
# Build entire solution
dotnet build Payroll.slnx

# EF migrations (from repo root)
dotnet ef migrations add <Name> --project Payroll.Infrastructure --startup-project Payroll.Api

# Production Angular build
cd payroll-admin && npm run build
```

## Configuration reference

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:Default` | SQL Server database |
| `Jwt:*` | Issuer, audience, signing key, token lifetimes |
| `Seed:*` | Bootstrapped Admin/HR users and optional sample data |
| `Database:InitializeOnStartup` | Migrate + seed on API start |
| `Cors:AllowedOrigins` | Allowed browser origins |

Do not commit `appsettings.Development.json` — it is gitignored. Use the `.example` file as a template.

## License

Private repository. All rights reserved.
