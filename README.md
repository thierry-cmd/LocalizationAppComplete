# Localization App - Blazor Server

Application de démonstration pour la gestion des traductions dans Blazor Server avec fichiers JSON.

## 🎯 Objectif

Montrer une approche simple et maintenable pour gérer les traductions :
- **Pas de base de données** pour les traductions
- **Fichiers JSON** versionnés avec Git
- **Structure modulaire** : `@Trad.Module.Category["Key"]`
- **Multi-langues** : EN, FR, NL (extensible)
- **Architecture testable** avec services et DTOs
- **CI/CD** avec GitHub Actions

## 🏗️ Architecture
```
LocalizationAppComplete/
├── src/
│   ├── localizationApp.Client/          # Application Blazor Server
│   │   ├── Components/
│   │   │   ├── Layout/
│   │   │   │   ├── MainLayout.razor
│   │   │   │   └── NavMenu.razor
│   │   │   ├── Pages/
│   │   │   │   ├── Home.razor
│   │   │   │   └── Persons/
│   │   │   │       ├── Index.razor
│   │   │   │       ├── Create.razor
│   │   │   │       ├── Edit.razor
│   │   │   │       └── Delete.razor
│   │   │   └── CultureSelector.razor
│   │   ├── Controllers/
│   │   │   └── CultureController.cs
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Extensions/
│   │   │   └── EnumExtensions.cs
│   │   ├── Models/
│   │   │   ├── Person.cs
│   │   │   ├── PersonStatus.cs
│   │   │   ├── Gender.cs
│   │   │   └── Dtos/
│   │   │       └── PersonDto.cs
│   │   ├── Services/
│   │   │   ├── Persons/
│   │   │   │   ├── IPersonService.cs
│   │   │   │   └── PersonService.cs
│   │   │   └── Translation/
│   │   │       ├── JsonTranslationLoader.cs
│   │   │       ├── CategoryLocalizer.cs
│   │   │       ├── ModuleTranslations.cs
│   │   │       └── TranslationService.cs
│   │   ├── Translations/
│   │   │   ├── Global.en.json
│   │   │   ├── Global.fr.json
│   │   │   ├── Global.nl.json
│   │   │   ├── Person.en.json
│   │   │   ├── Person.fr.json
│   │   │   └── Person.nl.json
│   │   ├── Validators/
│   │   │   └── PersonValidator.cs
│   │   ├── Dockerfile
│   │   └── Program.cs
│   │
│   └── localizationApp.Tests/           # Tests unitaires
│       ├── Services/
│       │   ├── Persons/
│       │   │   └── PersonServiceTests.cs
│       │   └── Translation/
│       │       └── JsonTranslationLoaderTests.cs
│       └── Validators/
│           └── CreatePersonDtoValidatorTests.cs
│
├── .github/
│   └── workflows/
│       └── build-and-test.yml           # CI/CD GitHub Actions
│
└── README.md
```

## 🚀 Démarrage
```bash
cd src/localizationApp.Client
dotnet run
```

Ouvrir https://localhost:7288

## 🐳 Docker
```bash
# Build l'image
docker build -t localizationapp .

# Lancer le conteneur
docker run -p 8080:8080 localizationapp
```

Ou dans Visual Studio : sélectionner **Docker** dans le menu de lancement.

## 🧪 Tests
```bash
cd src/localizationApp.Tests
dotnet test
```

### Tests disponibles (14 tests)

| Classe | Tests | Description |
|--------|-------|-------------|
| `PersonServiceTests` | 8 | CRUD complet (GetAll, GetById, Create, Update, Delete) |
| `JsonTranslationLoaderTests` | 5 | Traductions EN/FR/NL, fallback, clé manquante |
| `CreatePersonDtoValidatorTests` | 1 | Validation des DTOs |

## 🔄 CI/CD

GitHub Actions exécute automatiquement à chaque push :
1. **Restore** des packages NuGet
2. **Build** du projet
3. **Exécution** des 14 tests

Voir l'onglet **Actions** sur GitHub pour le statut.

## 📁 Structure des fichiers de traduction

Chaque fichier suit le format `Module.culture.json` :
```json
// Person.fr.json
{
  "Labels": {
    "FirstName": "Prénom",
    "LastName": "Nom",
    "Gender": "Genre"
  },
  "Titles": {
    "PageTitle": "Personnes"
  },
  "Enums": {
    "Status_Active": "Actif",
    "Gender_Male": "Homme"
  }
}
```

## 💡 Utilisation dans le code
```razor
@inject TranslationService Trad

<h1>@Trad.Person.Titles["PageTitle"]</h1>
<label>@Trad.Person.Labels["FirstName"]</label>
<button>@Trad.Global.Actions["Save"]</button>
<span>@person.Status.ToLocalizedString(Trad.Person.Enums)</span>
```

## 🏛️ Architecture en couches
```
Pages Razor (UI)
      │
      ▼
IPersonService (Interface)
      │
      ▼
PersonService (Logique métier)
      │
      ▼
AppDbContext (Accès données)
      │
      ▼
SQLite
```

### DTOs utilisés

| DTO | Usage |
|-----|-------|
| `PersonDto` | Lecture (liste, affichage) |
| `CreatePersonDto` | Création |
| `UpdatePersonDto` | Modification |

## ➕ Ajouter une nouvelle langue

1. Créer `Translations/Global.xx.json`
2. Créer `Translations/Person.xx.json`
3. Ajouter dans `Program.cs` : `new CultureInfo("xx")`
4. Ajouter dans `CultureSelector.razor` : `<option value="xx">Langue</option>`

## ➕ Ajouter un champ enum

Exemple : ajouter `Gender` à `Person`

### 1. Créer l'enum
```csharp
// Models/Gender.cs
public enum Gender
{
    Male,
    Female,
    Other
}
```

### 2. Ajouter au modèle
```csharp
// Models/Person.cs
public Gender Gender { get; set; } = Gender.Other;
```

### 3. Ajouter les traductions
```json
// Person.xx.json - dans "Labels"
"Gender": "Genre"

// Person.xx.json - dans "Enums"
"Gender_Male": "Homme",
"Gender_Female": "Femme",
"Gender_Other": "Autre"
```

### 4. Modifier EnumExtensions.cs
```csharp
var prefix = enumTypeName switch
{
    "PersonStatus" => "Status",
    "Gender" => "Gender",
    _ => enumTypeName
};
```

### 5. Modifier les pages Razor
```razor
// Label
<label>@Trad.Person.Labels["Gender"]</label>

// Select
<InputSelect @bind-Value="person.Gender">
    @foreach (var gender in Enum.GetValues<Gender>())
    {
        <option value="@gender">@gender.ToLocalizedString(Trad.Person.Enums)</option>
    }
</InputSelect>

// Affichage
@person.Gender.ToLocalizedString(Trad.Person.Enums)
```

### 6. Recréer la base de données
```bash
del app.db
dotnet run
```

## ➕ Ajouter une nouvelle feature

Exemple : ajouter un module `Invoice`

### 1. Créer les fichiers de traduction

- `Translations/Invoice.en.json`
- `Translations/Invoice.fr.json`
- `Translations/Invoice.nl.json`

### 2. Créer la classe dans ModuleTranslations.cs
```csharp
public class InvoiceTranslations
{
    public CategoryLocalizer Labels { get; }
    public CategoryLocalizer Titles { get; }

    public InvoiceTranslations(JsonTranslationLoader loader)
    {
        Labels = new CategoryLocalizer(loader, "Invoice.Labels");
        Titles = new CategoryLocalizer(loader, "Invoice.Titles");
    }
}
```

### 3. Ajouter dans TranslationService.cs
```csharp
public InvoiceTranslations Invoice { get; }

public TranslationService(JsonTranslationLoader loader)
{
    // ...
    Invoice = new InvoiceTranslations(loader);
}
```

### 4. Utiliser dans le code
```razor
@Trad.Invoice.Labels["InvoiceNumber"]
```

## ➖ Supprimer une feature

1. Supprimer les fichiers `Translations/Feature.xx.json`
2. Supprimer la classe `FeatureTranslations` dans `ModuleTranslations.cs`
3. Supprimer la propriété dans `TranslationService.cs`
4. Supprimer les pages Razor associées

## ✅ Avantages de cette approche

| Aspect | Bénéfice |
|--------|----------|
| Git | Historique, merge, review, rollback |
| Simplicité | Fichiers JSON lisibles par tous |
| Performance | Chargé 1x en mémoire au démarrage |
| Typage | IntelliSense avec `@Trad.Module.Category["Key"]` |
| Isolation | 1 feature = 1 fichier par langue |
| Testable | Services mockables, tests unitaires |
| CI/CD | Build et tests automatiques |
| Docker | Déploiement conteneurisé |

## 🛠️ Technologies

- .NET 9
- Blazor Server
- Entity Framework Core (SQLite)
- FluentValidation
- xUnit (tests unitaires)
- NSubstitute (mocking)
- Docker
- GitHub Actions (CI/CD)
- Bootstrap 5 + Bootstrap Icons