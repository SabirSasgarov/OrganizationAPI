# Organization API

ASP.NET Core Web API for managing events, organizers, tickets, and user accounts. The solution includes the API project and an xUnit test project with controller tests plus integration tests for the HTTP pipeline.

## Solution Structure

```text
OriganizationAPI.slnx
OriganizationAPI/
  Controllers/        API endpoints
  Data/               EF Core DbContext, migrations, entity configuration
  Dtos/               Request and response DTOs with FluentValidation rules
  Models/             Domain and Identity models
  Profiles/           AutoMapper mapping profile
  Services/           JWT and refresh token services
Organization.Tests/
  *ControllerTests.cs Controller-level tests
  ApiIntegrationTests.cs HTTP pipeline integration tests
```

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Bearer authentication
- AutoMapper
- FluentValidation
- xUnit, Moq, and `Microsoft.AspNetCore.Mvc.Testing`

## Configuration

The API reads settings from [appsettings.json](</C:/Users/Admin/OneDrive/İş masası/Programs/Web Apps/OriganizationAPI/OriganizationAPI/appsettings.json>).

Important settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\MSSQLSERVER01;Database=OrganizationApiDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your_secure_secret_key_which_is_256_bits_long_minimum",
    "Issuer": "http://localhost:5195",
    "Audience": "http://localhost:5195",
    "Expire": 60
  }
}
```

For local development, update `DefaultConnection` if your SQL Server instance or database name is different. Use a strong JWT key outside sample/local setups.

## Database

Apply the existing EF Core migrations before running the API against SQL Server:

```powershell
dotnet ef database update --project OriganizationAPI
```

If the `dotnet ef` command is unavailable, install the EF Core CLI tools:

```powershell
dotnet tool install --global dotnet-ef
```

## Run The API

From the solution root:

```powershell
dotnet run --project OriganizationAPI
```

The development profile runs at:

```text
http://localhost:5195
```

Swagger is available in Development mode:

```text
http://localhost:5195/swagger
```

## API Overview

Account endpoints:

- `POST /api/Account/register`
- `POST /api/Account/login`
- `POST /api/Account/reset_password`
- `POST /api/Account/forget_password`
- `POST /api/Account/confirm_email`
- `POST /api/Account/refresh-token`

Event endpoints:

- `GET /api/Events`
- `POST /api/Events`
- `PATCH /api/Events/{id}/banner`
- `GET /api/Events/{id}/tickets`
- `GET /api/Events/{id}/organizer`
- `POST /api/Events/{id}/tickets`

Organizer endpoints:

- `GET /api/Organizer`
- `POST /api/Organizer`
- `PATCH /api/Organizer/{id}/logo`
- `GET /api/Organizer/{id}/events`

Ticket endpoints:

- `GET /api/Ticket`
- `POST /api/Ticket`

User endpoints:

- `GET /api/User/all_users`
- `GET /api/User/profile`

Some endpoints require a JWT bearer token. Admin-only endpoints also require the `Admin` role.

## Run Tests

From the solution root:

```powershell
dotnet test OriganizationAPI.slnx
```

The integration tests run the API in the `Testing` environment. In that environment, [Program.cs](</C:/Users/Admin/OneDrive/İş masası/Programs/Web Apps/OriganizationAPI/OriganizationAPI/Program.cs>) uses EF Core InMemory instead of SQL Server, so tests do not require a local database.

## Notes

- The project name and namespace currently use `OriganizationAPI`.
- File upload endpoints save images under `wwwroot/images`.
- Login requires confirmed email before returning JWT and refresh tokens.
