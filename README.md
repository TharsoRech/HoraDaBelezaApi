# Hora da Beleza — Backend API

REST API for the **Hora da Beleza** beauty salon SaaS platform.

## Tech Stack

- **.NET 9** — Web API
- **CQRS + MediatR** — Command/Query separation
- **Dapper** — Lightweight SQL ORM
- **SQL Server** — Database
- **JWT Bearer** — Authentication
- **FluentValidation** — Input validation
- **BCrypt** — Password hashing
- **Swagger / Swashbuckle** — Interactive API docs

## Project Structure

```
src/
├── HoraDaBeleza.Domain/
│   ├── Entities/         # User, Salon, Professional, Service, Appointment...
│   ├── Enums/            # AppointmentStatus, UserType, SubscriptionStatus...
│   └── Exceptions/       # NotFoundException, BusinessException...
│
├── HoraDaBeleza.Application/
│   ├── Commands/         # Users/, Salons/, Appointments/, + OtherCommands.cs
│   ├── Queries/          # AllQueries.cs
│   ├── DTOs/             # Request/Response records
│   ├── Interfaces/       # IRepository contracts + ITokenService
│   └── Behaviors/        # ValidationBehavior (FluentValidation pipeline)
│
├── HoraDaBeleza.Infrastructure/
│   ├── Data/             # SqlServerConnectionFactory
│   ├── Repositories/     # Dapper implementations
│   └── Services/         # JwtTokenService
│
└── HoraDaBeleza.API/
    ├── Controllers/      # One controller per resource
    ├── Middleware/        # ExceptionMiddleware
    └── Properties/       # launchSettings.json

Database/
└── 001_CreateDatabase.sql   # Full schema + seed data
```

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)

### 1. Create the database

```bash
sqlcmd -S localhost -i Database/001_CreateDatabase.sql
```

### 2. Update the connection string

Edit `src/HoraDaBeleza.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HoraDaBeleza;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### 3. Run

```bash
dotnet run --project src/HoraDaBeleza.API
```

Swagger opens at: **http://localhost:5000**

## Authentication

1. `POST /api/auth/register` — create an account
2. `POST /api/auth/login` — get a JWT token
3. Click **Authorize 🔒** in Swagger and enter: `Bearer {token}`

## User Types

| Value | Type         | Permissions                           |
|-------|--------------|---------------------------------------|
| 1     | Client       | Book appointments, submit reviews     |
| 2     | Professional | Manage schedule, update status        |
| 3     | Owner        | Manage salon, services, professionals |
| 4     | Admin        | Full access                           |

## Default Admin Credentials

- **Email:** `admin@horadabeleza.com`
- **Password:** `Admin@123`

## Related Project

- **Mobile App (React Native + Expo):** https://github.com/TharsoRech/horadabeleza
