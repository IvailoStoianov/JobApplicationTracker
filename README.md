# Job Application Tracker

A simple full‑stack app to track job applications. Built as a portfolio project.

## Live demo

- Live: [jobapplicationtracker-fe.onrender.com](https://jobapplicationtracker-fe.onrender.com/)
- Note: Hosted on a free tier. The first request may take a few seconds due to cold start.

## Tech stack

- Backend: ASP.NET Core 9 (Web API), Entity Framework Core, Identity (custom user), JWT auth
- Database: PostgreSQL
- Frontend: React 19, Vite 7, TypeScript, React Router
- Testing: xUnit (unit) and basic UI test scaffold

## Features

- Register and login with email/password
- JWT‑based authentication stored in localStorage on the client
- CRUD for job applications tied to the authenticated user
- Filter/paginate jobs; view statuses; optional salary and notes
- Responsive, minimal UI with light/dark theme toggle

## Project structure

```
JobApplicationTracker/
  JobApplicationTracker.API/            # ASP.NET Core API (ports: https 7068, http 5088 in dev)
    Controllers/
      AuthenticationController.cs       # /api/Authentication/Login, /Register
      JobsController.cs                 # /api/Jobs/* (auth required except Statuses)
    Program.cs                          # DI, EF Core, JWT, CORS, Swagger/OpenAPI
    appsettings.json                    # JwtConfig, ConnectionStrings
  JobApplicationTracker.Data/           # EF Core DbContext, Repositories, Migrations
  JobApplicationTracker.Data.Models/    # Entity models (e.g., Job, ApplicationUser)
  JobApplicationTracker.API.Models/     # Request/Response DTOs
  JobApplicationTracker.API.Infrastructure/ # ApiResponse, DI helpers, Claims extensions
  JobApplicationTracker.Services/       # Domain services (Account, Jobs, Jwt)
  jobapplicationtracker-client-web/     # React + Vite frontend
    src/
      auth/AuthContext.tsx              # login/logout, token storage
      api/client.ts                     # fetch helper adds Bearer token
      pages/                            # Login, Register, Jobs (CRUD UI)
      components/ThemeToggle.tsx
      vite.config.ts                    # dev proxy to API
```

## Running locally

### Prerequisites

- .NET SDK 9 (API) and Node.js 20+ (frontend)
- PostgreSQL instance (local Docker or managed)

### 1) Configure the API

Set a PostgreSQL connection string. You can use either standard Npgsql syntax or a `postgres://` URL. The API auto‑migrates the database on startup.

Environment variables (recommended for secrets):

- `ConnectionStrings__DefaultConnection` — e.g. `Host=localhost;Port=5432;Database=jat;Username=postgres;Password=postgres;SSL Mode=Disable;Trust Server Certificate=true`
  - Or as URL: `postgres://user:pass@localhost:5432/jat?sslmode=disable`
- `JwtConfig__Issuer` — e.g. `https://localhost:7068/`
- `JwtConfig__Audience` — e.g. `https://localhost:7068`
- `JwtConfig__Key` — long random string
- `JwtConfig__TokenValidityMins` — e.g. `60`
- `FRONTEND_ORIGIN` — e.g. `https://localhost:5173` (enables CORS for the dev frontend)

Run the API (development):

```bash
dotnet run --project JobApplicationTracker.API
```

Dev profiles (from `Properties/launchSettings.json`):

- HTTP: `http://localhost:5088`
- HTTPS: `https://localhost:7068`

OpenAPI/Swagger is enabled in Development.

### 2) Start the frontend

From `jobapplicationtracker-client-web`:

```bash
npm install
npm run dev
```

By default the frontend calls relative `/api` and Vite proxies to `https://localhost:7068` (see `vite.config.ts`).

Optional: to point the frontend at a deployed backend, create `.env` and set:

```
VITE_API_BASE_URL=https://your.api.host
```

Then the client will call `${VITE_API_BASE_URL}/api/...` without using the proxy.

## API overview

All responses are wrapped in a consistent envelope:

```json
{
  "success": true,
  "message": "optional",
  "data": {}
}
```

### Auth

- POST `/api/Authentication/Login`
  - Body: `{ email, password }`
  - Returns: `{ email, accessToken, expiresInSeconds }`

- POST `/api/Authentication/Register`
  - Body: `{ username, email, password, confirmPassword }`
  - Returns: `{ userId, email }`

### Jobs (requires Bearer token unless noted)

- GET `/api/Jobs/Statuses` (public)
- GET `/api/Jobs/GetAllUserJobs?status&from&to&page&pageSize`
- POST `/api/Jobs/CreateJob`
  - Body: `{ company, position, status, applicationDate, notes?, salary?, contact? }`
- PUT `/api/Jobs/UpdateJob`
  - Body: `{ jobId, company, position, status, applicationDate, notes?, salary?, contact? }`
- DELETE `/api/Jobs/DeleteJob?id={guid}`

Status values: `Applied | InterviewScheduled | InterviewCompleted | OfferReceived | Rejected | Withdrawn`

## Data model (simplified)

- `Job`: `Id`, `ApplicationUserId`, `Company`, `Position`, `Status`, `ApplicationDate`, `LastUpdated`, `Notes?`, `Salary?`, `Contact?`
- Users: custom `ApplicationUser` with hashed passwords; JWT includes `NameIdentifier` claim with user id

EF Core applies migrations automatically on app start. Seeding is wired via `SeedDataConfiguration`.

## Docker

There are two Dockerfiles:

- Root `Dockerfile` (runtime: .NET 8)
- `JobApplicationTracker.API/Dockerfile` (runtime: .NET 9)

Example build/run (root Dockerfile):

```bash
docker build -t jat-api .
docker run -p 8080:8080 \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=jat;Username=postgres;Password=postgres;SSL Mode=Disable;Trust Server Certificate=true" \
  -e JwtConfig__Issuer="http://localhost:8080/" \
  -e JwtConfig__Audience="http://localhost:8080" \
  -e JwtConfig__Key="REPLACE_ME_LONG_RANDOM" \
  -e JwtConfig__TokenValidityMins=60 \
  -e FRONTEND_ORIGIN="http://localhost:5173" \
  jat-api
```

If you prefer the .NET 9 image, build using `JobApplicationTracker.API/Dockerfile`:

```bash
docker build -f JobApplicationTracker.API/Dockerfile -t jat-api:net9 .
```

## Tests

```bash
dotnet test
```

## Notes / limitations

- Portfolio project; not hardened for production
- JWT is stored in localStorage for simplicity
- Validation is basic; server may return generic error messages
- Free‑tier hosting may cold start; first request could take a few seconds

## License

MIT — feel free to use parts of this project for learning.


