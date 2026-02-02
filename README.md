# Localization Demo

A Blazor Server application demonstrating JSON-based localization with a REST API backend.

![Home Page](screenshots/Screenshot_1.png)

## Live Demo

- **Blazor Client**: https://localizationapp-client.azurewebsites.net
- **API**: https://localizationapp-api.azurewebsites.net/api/persons

---

## Why This Project?

### The Localization Problem in .NET

The standard approach in .NET for localization uses `.resx` files. While it works, it has some limitations:

| Limitation | Impact |
|------------|--------|
| Compiled XML files | Requires recompilation to change a translation |
| Hard to version | Frequent Git conflicts on XML files |
| Not readable by non-devs | Translators can't easily edit them |
| Flat structure | All keys at the same level, no hierarchy |
| Tooling required | Need Visual Studio or special tools |

### My Solution: JSON Files

I chose JSON files for translations because:

| Advantage | Why it matters |
|-----------|----------------|
| No recompilation | Change a JSON file, refresh the page |
| Git-friendly | JSON = plain text, easy to merge |
| Readable by anyone | A translator can edit without Visual Studio |
| Hierarchical structure | Organized by domain (Global, Person, etc.) |
| Universal standard | JSON is used everywhere (React, Angular, etc.) |

### File Structure

```
Translations/
├── Global.en.json    ← Navigation, common buttons
├── Global.fr.json
├── Global.nl.json
├── Person.en.json    ← Labels, messages for Person module
├── Person.fr.json
└── Person.nl.json
```

### Usage in Blazor

```csharp
@inject TranslationService Trad

<h1>@Trad.Person.Titles["PageTitle"]</h1>
<label>@Trad.Person.Labels["FirstName"]</label>
```

---

## Features

### Blazor Client
- Multi-language support (EN, FR, NL) with JSON files
- Full CRUD for person management
- Validation with FluentValidation
- Responsive UI with Bootstrap
- Version number display

### REST API
- CQRS architecture with MediatR
- Vertical Slice Architecture
- FluentValidation with Pipeline Behavior
- Mapping with Mapster
- Swagger documentation
- Global exception middleware

---

## Technologies

| Technology | Version |
|------------|---------|
| .NET | 9.0 |
| Blazor Server | 9.0 |
| Entity Framework Core | 9.0 |
| MediatR | 12.x |
| FluentValidation | 12.x |
| Mapster | 7.x |
| SQLite | - |
| Docker | - |

---

## Screenshots

### Language Selector
![Language Selector](screenshots/Screenshot_1.png)

### Home Page (French)
![Home FR](screenshots/Screenshot_2.png)

### Person List
![Persons List](screenshots/Screenshot_3.png)

### Form with Validation
![Form Validation](screenshots/Screenshot_4.png)

### Edit Form
![Edit Form](screenshots/Screenshot_5.png)

### Delete Confirmation
![Delete Confirmation](screenshots/Screenshot_6.png)

### API Swagger
![Swagger](screenshots/Screenshot_7.png)

---

## Getting Started with Docker

### What is Docker?

Docker allows you to run applications in isolated containers. Instead of installing .NET, SQLite, and configuring everything manually, you just run one command and everything works.

### Prerequisites

1. **Install Docker Desktop**
   - Download from: https://docs.docker.com/get-started/get-docker/
   - Available for Windows, Mac, and Linux
   - After installation, make sure Docker Desktop is running

2. **Check that ports 5000 and 5001 are available**
   - Port 5000: Blazor Client
   - Port 5001: API

### Running the Application

```bash
# Clone the repository
git clone https://github.com/thierry-cmd/LocalizationAppComplete.git
cd LocalizationAppComplete

# Build and start the containers
docker-compose up --build
```

Wait for the build to complete. You'll see logs from both containers. When ready, open:
- Client: http://localhost:5000
- API: http://localhost:5001/api/persons

### Docker Commands

| Command | Description |
|---------|-------------|
| `docker-compose up --build` | Build images and start containers |
| `docker-compose up` | Start containers (without rebuilding) |
| `docker-compose down` | Stop and remove containers |
| `docker-compose down -v` | Stop containers AND delete data |
| `docker-compose logs -f` | View logs in real-time |

### Data Persistence

The `docker-compose.yml` file includes a volume for the API database:

```yaml
volumes:
  - api-data:/app/data
```

This means:
- `docker-compose down` → Data is **kept**
- `docker-compose down -v` → Data is **deleted**

### How It Works

```
┌─────────────────────────────────────────────────────────┐
│                   Docker Network                         │
│  ┌─────────────────┐         ┌─────────────────┐        │
│  │     Client      │         │      API        │        │
│  │   (Port 5000)   │ ──────> │   (Port 5001)   │        │
│  │                 │  HTTP   │                 │        │
│  │  Blazor Server  │         │  .NET API       │        │
│  └─────────────────┘         └────────┬────────┘        │
│                                       │                 │
│                              ┌────────▼────────┐        │
│                              │    SQLite DB    │        │
│                              │   (Volume)      │        │
│                              └─────────────────┘        │
└─────────────────────────────────────────────────────────┘
```

---

## Running with Visual Studio

1. Open `LocalizationApp.sln`
2. Right-click on Solution → Properties
3. Select "Multiple startup projects"
4. Set both API and Client to "Start"
5. Press F5

---

## Project Structure

```
LocalizationApp/
├── src/
│   ├── localizationApp.API/
│   │   ├── Behaviors/          # MediatR pipeline
│   │   ├── Controllers/        # API Controllers
│   │   ├── Data/               # DbContext
│   │   ├── Features/           # CQRS (Commands/Queries)
│   │   │   └── Persons/
│   │   │       ├── Commands/
│   │   │       └── Queries/
│   │   ├── Mapping/            # Mapster configuration
│   │   ├── Middleware/         # Exception handler
│   │   └── Models/             # Entities and DTOs
│   │
│   ├── localizationApp.Client/
│   │   ├── Components/         # Blazor pages and components
│   │   ├── Services/           # Services (HttpClient, Translation)
│   │   ├── Translations/       # JSON translation files
│   │   └── Validators/         # FluentValidation
│   │
│   └── localizationApp.Tests/  # Unit tests
│
├── docker-compose.yml
└── README.md
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/persons | List all persons |
| GET | /api/persons/{id} | Get a person by ID |
| GET | /api/persons/list | Light list (simplified DTO) |
| POST | /api/persons | Create a person |
| PUT | /api/persons/{id} | Update a person |
| DELETE | /api/persons/{id} | Delete a person |

---

## Azure Deployment

### Architecture

```
┌──────────────────┐         ┌──────────────────┐
│   Azure App      │         │   Azure App      │
│   Service        │ ──────> │   Service        │
│                  │  HTTPS  │                  │
│   Client         │         │   API            │
│   (Free tier)    │         │   (Free tier)    │
└──────────────────┘         └──────────────────┘
```

### Deployment Steps

1. **Create two App Services** on Azure (Free tier F1)
   - `localizationapp-client`
   - `localizationapp-api`

2. **Configure environment variables**

   **For the Client:**
   | Name | Value |
   |------|-------|
   | `ApiBaseUrl` | `https://localizationapp-api.azurewebsites.net` |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

   **For the API:**
   | Name | Value |
   |------|-------|
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

3. **Configure CORS on the API**

   The API must accept requests from the Client. In `Program.cs`:
   ```csharp
   policy.WithOrigins(
       "https://localhost:7288",           // Local dev
       "http://localhost:5000",            // Docker
       "https://localizationapp-client.azurewebsites.net"  // Azure
   )
   ```

4. **Publish from Visual Studio**
   - Right-click on project → Publish
   - Select Azure App Service
   - Follow the wizard

### Common Issues and Solutions

| Problem | Cause | Solution |
|---------|-------|----------|
| 404 Not Found | Wrong `ApiBaseUrl` | Check environment variable in Azure |
| CORS blocked | Azure URL not allowed | Add URL to CORS policy, republish API |
| SQLite error in Docker | `/app/data` folder missing | Add `RUN mkdir -p /app/data` in Dockerfile |
| Data lost on restart | No volume configured | Add volume in docker-compose.yml |

### .NET Configuration Hierarchy

```
1. appsettings.json                 ← Base configuration
2. appsettings.{Environment}.json   ← Environment-specific
3. Environment variables            ← Overrides everything (Azure)
4. Command line arguments           ← Highest priority
```

---

## Tests

The project includes 67 unit tests.

### Running Tests

```bash
cd src/localizationApp.Tests
dotnet test
```

### Test Coverage

| Category | Tests | Description |
|----------|-------|-------------|
| API Commands | 33 | Create, Update, Delete (handlers + validators) |
| API Queries | 27 | GetAll, GetById, GetPersonsList |
| Localization | 6 | JSON loading, English fallback |
| Client Validation | 1 | DTO validation |

### Test Structure

```
localizationApp.Tests/
├── Features/
│   └── Persons/
│       ├── Commands/
│       │   ├── CreatePersonCommandTests.cs
│       │   ├── UpdatePersonCommandTests.cs
│       │   └── DeletePersonCommandTests.cs
│       └── Queries/
│           ├── GetAllPersonsQueryTests.cs
│           ├── GetPersonByIdQueryTests.cs
│           └── GetPersonsListQueryTests.cs
├── Services/
│   └── Translation/
│       └── JsonTranslationLoaderTests.cs
└── Validators/
    └── CreatePersonDtoValidatorTests.cs
```

---

## What This Project Demonstrates

- Custom solution when standard tools don't fit the need
- Understanding of technical trade-offs
- Modular architecture (separation by domain)
- Focus on maintainability
- Docker containerization
- Cloud deployment (Azure)
- Unit testing with xUnit

---

## License

MIT

## Author

Thierry Leblanc
