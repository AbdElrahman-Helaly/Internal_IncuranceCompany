# MCIApi – Clean Architecture Solution

This repository hosts the Medical Consultants International (MCI) API built with ASP.NET Core 8 using a Clean Architecture layout. The solution separates responsibilities across Domain, Application, Infrastructure, and API layers to keep business rules independent from persistence, delivery, and cross-cutting concerns.

## Solution Structure

| Project | Purpose |
|---------|---------|
| `MCIApi.Domain` | Core domain entities, enums, and abstractions (e.g., `Approval`, `Provider`, `MemberPolicyInfo`, `IGenericRepository`, `IUnitOfWork`). No external dependencies. |
| `MCIApi.Application` | DTOs, service interfaces, validation attributes, and common results (`ServiceResult`). This layer orchestrates use cases such as categories, providers, and approvals without knowing about EF Core. |
| `MCIApi.Infrastructure` | EF Core persistence (`AppDbContext`), repository/unit-of-work implementations, concrete services (Category, Provider, Approval, MemberPolicy, etc.), Identity integration, file/image storage helpers, and migrations. |
| `MCIApi.API` | ASP.NET Core Web API entry point. Configures DI, middleware, authentication, Swagger, and exposes controllers (e.g., Category, Provider). |
| `MCIApi.Tests` | Placeholder for automated tests (unit/integration). The `.csproj` is present so the solution builds; add test classes here. |

```
CleanSolution/
├─ MCIApi.Domain/
├─ MCIApi.Application/
├─ MCIApi.Infrastructure/
├─ MCIApi.API/
├─ MCIApi.Tests/
└─ MCIApi.sln
```

## Key Technologies

- .NET 8 SDK
- ASP.NET Core Web API
- Entity Framework Core 8 (SQL Server provider)
- ASP.NET Core Identity for authentication/authorization
- Clean Architecture + DI/IoC
- Swagger/OpenAPI for interactive docs

## Database & Seeding

`AppDbContext` (Infrastructure project) is the EF Core DbContext. It configures every entity, relationships, precision, indices, and seeds baseline data through `SeedStaticData` (categories, provider categories, policies, members, etc.).

Connection string key: `ConnectionStrings:DefaultConnection` in `MCIApi.API/appsettings.json`. Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=DEVELOPER\\LOCALHOST123;Database=MCIDb;User Id=sa;Password=123;TrustServerCertificate=True;"
}
```

## New Approval Feature

- **Domain**: `Approval` entity links optional `MemberPolicyInfo` and `Provider`, stores metadata (diagnosis, max amount, debit/repeated flags, audit trail).
- **Application DTOs**:
  - `ApprovalCreateDto`: payload for creating approvals.
  - `ApprovalReadDto`: full view for detail screens.
  - `ApprovalListItemDto` + `ApprovalPagedResultDto`: lightweight records for paginated grids.
  - `ApprovalSearchFilterDto`: keyword/member/provider/date filters with paging metadata.
- **Service Contract**: `IApprovalService` exposes `GetAllAsync`, `GetByIdAsync`, and `CreateAsync` returning `ServiceResult<T>` responses.
- **Infrastructure Service**: `ApprovalService` applies filters, pagination, projection, and persistence via `AppDbContext`. Creation automatically stamps `CreatedAt`/`CreatedBy` and trims user inputs.
- **Next steps**: expose Approval endpoints in `MCIApi.API` (e.g., `ApprovalController`) and register the service in `Program.cs`. Once ready, generate/apply an EF migration so the `Approvals` table exists in SQL Server.

## Setup & Usage

1. **Prerequisites**
   - .NET 8 SDK installed.
   - SQL Server instance accessible.

2. **Clone & Restore**
   ```powershell
   git clone <repo-url>
   cd CleanSolution
   dotnet restore MCIApi.sln
   ```

3. **Configure Connection String**
   Update `MCIApi.API/appsettings.json` with your SQL Server details under `ConnectionStrings:DefaultConnection`.

4. **Database Migrations**
   From the solution root (or `MCIApi.API` project directory), create and apply migrations:
   ```powershell
   dotnet ef migrations add <MigrationName> --project MCIApi.Infrastructure --startup-project MCIApi.API
   dotnet ef database update --project MCIApi.Infrastructure --startup-project MCIApi.API
   ```
   This compiles the entire solution, so ensure it builds before running.

5. **Run the API**
   ```powershell
   dotnet run --project MCIApi.API
   ```
   Navigate to `https://localhost:<port>/swagger` to explore endpoints. Use predefined admin credentials (`admin@mci.com` / `Admin@12345`) seeded via Identity for authentication scenarios.

6. **Testing**
   Add unit/integration tests inside `MCIApi.Tests` and execute with:
   ```powershell
   dotnet test MCIApi.Tests/MCIApi.Tests.csproj
   ```

## Developer Guidelines

- **Layering**: keep domain models pure; business logic belongs in Application services; Infrastructure handles EF/database, external services, and Identity; API should only orchestrate requests/responses.
- **ServiceResult**: services return `ServiceResult<T>` to standardize success/error handling (validation, conflicts, not found, unexpected, unauthorized).
- **Dependency Injection**: register all interfaces in `Program.cs` (API project). Approval service should be registered alongside existing services like `ICategoryService`, `IProviderService`, etc.
- **Migrations**: prefer incremental migrations over manual schema changes. Always check-in migration files plus updated `AppDbContext` or entity types.
- **Seeding**: static data lives inside `AppDbContext.SeedStaticData`. For large/conditional data, consider dedicated seeders executed at startup (see `Data/Seed/DatabaseSeeder.cs`).
- **Logging & Errors**: `GlobalExceptionMiddleware` handles unhandled exceptions and returns consistent responses. Services should provide descriptive error codes (e.g., `ApprovalNotFound`).

## Contributing / Next Work Items

- Complete Approval API controller (CRUD + pagination) and wire `ApprovalService` into DI.
- Add EF migration for Approvals and update the database.
- Fill `MCIApi.Tests` with meaningful unit/integration tests (e.g., Approval service, controller endpoints, repository behaviors).
- Expand documentation with API endpoint details once the Approval controller is added.

For any feature work, follow the established patterns (DTOs + service interface + implementation + controller + migration) to keep the codebase consistent and maintainable.
