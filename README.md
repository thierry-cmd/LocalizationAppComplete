# Localization Demo

Application Blazor Server avec localisation JSON et API REST.

![Home Page](screenshots/Screenshot_1.png)

## Démo en ligne

- **Client Blazor** : https://localizationapp-client.azurewebsites.net
- **API** : https://localizationapp-api.azurewebsites.net/api/persons

## Fonctionnalités

### Client Blazor
- Localisation multilingue (EN, FR, NL) avec fichiers JSON
- CRUD complet pour la gestion des personnes
- Validation avec FluentValidation
- Interface responsive avec Bootstrap
- Affichage du numéro de version

### API REST
- Architecture CQRS avec MediatR
- Vertical Slice Architecture
- FluentValidation avec Pipeline Behavior
- Mapping avec Mapster
- Documentation Swagger
- Middleware de gestion des exceptions

## Technologies

| Technologie | Version |
|-------------|---------|
| .NET | 9.0 |
| Blazor Server | 9.0 |
| Entity Framework Core | 9.0 |
| MediatR | 12.x |
| FluentValidation | 12.x |
| Mapster | 7.x |
| SQLite | - |
| Docker | - |

## Screenshots

### Sélecteur de langue
![Language Selector](screenshots/Screenshot_1.png)

### Page d'accueil (Français)
![Home FR](screenshots/Screenshot_2.png)

### Liste des personnes
![Persons List](screenshots/Screenshot_3.png)

### Formulaire avec validation
![Form Validation](screenshots/Screenshot_4.png)

### Modification
![Edit Form](screenshots/Screenshot_5.png)

### Suppression
![Delete Confirmation](screenshots/Screenshot_6.png)

### API Swagger
![Swagger](screenshots/Screenshot_7.png)

## Démarrage rapide

### Prérequis
- .NET 9.0 SDK
- Docker (optionnel)

### Option 1 : Docker Compose
```bash
git clone https://github.com/thierry-cmd/LocalizationAppComplete.git
cd LocalizationAppComplete
docker-compose up --build
```

Accéder à :
- Client : http://localhost:5000
- API : http://localhost:5001/api/persons

### Option 2 : Visual Studio

1. Ouvrir `LocalizationApp.sln`
2. Configurer les projets de démarrage multiples (API + Client)
3. F5

## Structure du projet
```
LocalizationApp/
├── src/
│   ├── localizationApp.API/
│   │   ├── Behaviors/          # Pipeline MediatR
│   │   ├── Controllers/        # API Controllers
│   │   ├── Data/               # DbContext
│   │   ├── Features/           # CQRS (Commands/Queries)
│   │   │   └── Persons/
│   │   │       ├── Commands/
│   │   │       └── Queries/
│   │   ├── Mapping/            # Configuration Mapster
│   │   ├── Middleware/         # Exception Handler
│   │   └── Models/             # Entités et DTOs
│   │
│   ├── localizationApp.Client/
│   │   ├── Components/         # Pages et composants Blazor
│   │   ├── Services/           # Services (HttpClient)
│   │   ├── Translations/       # Fichiers JSON de traduction
│   │   └── Validators/         # FluentValidation
│   │
│   └── localizationApp.Tests/  # Tests unitaires
│
├── docker-compose.yml
└── README.md
```

## API Endpoints

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | /api/persons | Liste toutes les personnes |
| GET | /api/persons/{id} | Récupère une personne |
| GET | /api/persons/list | Liste légère (DTO simplifié) |
| POST | /api/persons | Crée une personne |
| PUT | /api/persons/{id} | Modifie une personne |
| DELETE | /api/persons/{id} | Supprime une personne |

## Localisation

Les traductions sont dans des fichiers JSON :
```
Translations/
├── Global.en.json
├── Global.fr.json
├── Global.nl.json
├── Person.en.json
├── Person.fr.json
└── Person.nl.json
```

Utilisation dans Blazor :
```csharp
@inject TranslationService Trad

<h1>@Trad.Person.Titles["PageTitle"]</h1>
```

## Déploiement Azure

L'application est déployée sur Azure App Service :

| Service | URL |
|---------|-----|
| Client Blazor | https://localizationapp-client.azurewebsites.net |
| API | https://localizationapp-api.azurewebsites.net |

### Variables d'environnement

**Client :**
- `ApiBaseUrl` : URL de l'API
- `ASPNETCORE_ENVIRONMENT` : Production

**API :**
- `ASPNETCORE_ENVIRONMENT` : Production

## Tests

Le projet contient 67 tests unitaires.

### Lancer les tests
```bash
cd src/localizationApp.Tests
dotnet test
```

### Ce qui est testé

| Catégorie | Nb tests | Description |
|-----------|----------|-------------|
| API Commands | 33 | Create, Update, Delete (handlers + validators) |
| API Queries | 27 | GetAll, GetById, GetPersonsList |
| Localisation | 6 | Chargement JSON, fallback anglais |
| Validation Client | 1 | Validation des DTOs |

### Organisation des tests
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

## Licence

MIT

## Auteur

Thierry Leblanc