# Localization App - Blazor Server

Application de démonstration pour la gestion des traductions dans Blazor Server avec fichiers JSON.

## 🎯 Objectif

Montrer une approche simple et maintenable pour gérer les traductions :
- **Pas de base de données** pour les traductions
- **Fichiers JSON** versionnés avec Git
- **Structure modulaire** : `@Trad.Module.Category["Key"]`
- **Multi-langues** : EN, FR, NL (extensible)

## 🏗️ Architecture
```
localizationApp.Client/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── Home.razor
│   │   └── Persons/
│   │       ├── Index.razor
│   │       ├── Create.razor
│   │       ├── Edit.razor
│   │       └── Delete.razor
│   └── CultureSelector.razor
├── Data/
│   └── AppDbContext.cs
├── Extensions/
│   └── EnumExtensions.cs
├── Models/
│   ├── Person.cs
│   ├── PersonStatus.cs
│   └── Gender.cs
├── Services/
│   └── Translation/
│       ├── JsonTranslationLoader.cs
│       ├── CategoryLocalizer.cs
│       ├── ModuleTranslations.cs
│       └── TranslationService.cs
├── Translations/
│   ├── Global.en.json
│   ├── Global.fr.json
│   ├── Global.nl.json
│   ├── Person.en.json
│   ├── Person.fr.json
│   └── Person.nl.json
└── Program.cs
```

## 🚀 Démarrage
```bash
cd src/localizationApp.Client
dotnet run
```

Ouvrir https://localhost:7288

## 📁 Structure des fichiers de traduction

Chaque fichier suit le format `Module.culture.json` :
```json
// Person.fr.json
{
  "Labels": {
    "FirstName": "Prénom",
    "LastName": "Nom"
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
    "Gender" => "Gender",  // Ajouter cette ligne
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
| Pas de sync | Même fichiers dans tous les environnements |

## 🛠️ Technologies

- .NET 9
- Blazor Server
- Entity Framework Core (SQLite)
- FluentValidation
- Bootstrap 5 + Bootstrap Icons