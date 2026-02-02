# Démo Localisation

🇬🇧 [English version](README.md)

Application Blazor Server avec localisation JSON et API REST.

![Page d'accueil](screenshots/Screenshot_1.png)

## Démo en ligne

- **Client Blazor** : https://localizationapp-client.azurewebsites.net
- **API** : https://localizationapp-api.azurewebsites.net/api/persons

---

## Pourquoi ce projet ?

### Le problème de la localisation en .NET

L'approche standard en .NET utilise les fichiers `.resx`. Ça fonctionne, mais il y a des limitations :

| Limitation | Impact |
|------------|--------|
| Fichiers XML compilés | Nécessite une recompilation pour changer une traduction |
| Difficile à versionner | Conflits Git fréquents sur les fichiers XML |
| Pas lisible par les non-devs | Les traducteurs ne peuvent pas les éditer facilement |
| Structure plate | Toutes les clés au même niveau, pas de hiérarchie |
| Outils obligatoires | Besoin de Visual Studio ou d'outils spéciaux |

### Ma solution : fichiers JSON

J'ai choisi les fichiers JSON pour les traductions parce que :

| Avantage | Pourquoi c'est important |
|----------|--------------------------|
| Pas de recompilation | Modifier un fichier JSON, rafraîchir la page |
| Compatible Git | JSON = texte simple, facile à merger |
| Lisible par tous | Un traducteur peut éditer sans Visual Studio |
| Structure hiérarchique | Organisé par domaine (Global, Person, etc.) |
| Standard universel | JSON est utilisé partout (React, Angular, etc.) |

### Structure des fichiers

```
Translations/
├── Global.en.json    ← Navigation, boutons communs
├── Global.fr.json
├── Global.nl.json
├── Person.en.json    ← Labels, messages pour le module Person
├── Person.fr.json
└── Person.nl.json
```

### Utilisation dans Blazor

```csharp
@inject TranslationService Trad

<h1>@Trad.Person.Titles["PageTitle"]</h1>
<label>@Trad.Person.Labels["FirstName"]</label>
```

---

## Fonctionnalités

### Client Blazor
- Support multilingue (EN, FR, NL) avec fichiers JSON
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
- Middleware global pour les exceptions

---

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

---

## Captures d'écran

### Sélecteur de langue
![Sélecteur de langue](screenshots/Screenshot_1.png)

### Page d'accueil (Français)
![Accueil FR](screenshots/Screenshot_2.png)

### Liste des personnes
![Liste des personnes](screenshots/Screenshot_3.png)

### Formulaire avec validation
![Validation du formulaire](screenshots/Screenshot_4.png)

### Formulaire de modification
![Formulaire de modification](screenshots/Screenshot_5.png)

### Confirmation de suppression
![Confirmation de suppression](screenshots/Screenshot_6.png)

### API Swagger
![Swagger](screenshots/Screenshot_7.png)

---

## Démarrage avec Docker

### C'est quoi Docker ?

Docker permet d'exécuter des applications dans des conteneurs isolés. Au lieu d'installer .NET, SQLite et de tout configurer manuellement, tu lances une seule commande et tout fonctionne.

### Prérequis

1. **Installer Docker Desktop**
   - Télécharger depuis : https://docs.docker.com/get-started/get-docker/
   - Disponible pour Windows, Mac et Linux
   - Après l'installation, vérifie que Docker Desktop est lancé

2. **Vérifier que les ports 5000 et 5001 sont libres**
   - Port 5000 : Client Blazor
   - Port 5001 : API

### Lancer l'application

```bash
# Cloner le dépôt
git clone https://github.com/thierry-cmd/LocalizationAppComplete.git
cd LocalizationAppComplete

# Construire et démarrer les conteneurs
docker-compose up --build
```

Attends que la construction se termine. Tu verras les logs des deux conteneurs. Quand c'est prêt, ouvre :
- Client : http://localhost:5000
- API : http://localhost:5001/api/persons

### Commandes Docker

| Commande | Description |
|----------|-------------|
| `docker-compose up --build` | Construire les images et démarrer les conteneurs |
| `docker-compose up` | Démarrer les conteneurs (sans reconstruire) |
| `docker-compose down` | Arrêter et supprimer les conteneurs |
| `docker-compose down -v` | Arrêter les conteneurs ET supprimer les données |
| `docker-compose logs -f` | Voir les logs en temps réel |

### Persistance des données

Le fichier `docker-compose.yml` inclut un volume pour la base de données de l'API :

```yaml
volumes:
  - api-data:/app/data
```

Ça veut dire :
- `docker-compose down` → Les données sont **conservées**
- `docker-compose down -v` → Les données sont **supprimées**

### Comment ça fonctionne

```
┌─────────────────────────────────────────────────────────┐
│                   Réseau Docker                          │
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

## Lancer avec Visual Studio

1. Ouvrir `LocalizationApp.sln`
2. Clic droit sur la Solution → Propriétés
3. Sélectionner "Plusieurs projets de démarrage"
4. Mettre API et Client sur "Démarrer"
5. Appuyer sur F5

---

## Structure du projet

```
LocalizationApp/
├── src/
│   ├── localizationApp.API/
│   │   ├── Behaviors/          # Pipeline MediatR
│   │   ├── Controllers/        # Contrôleurs API
│   │   ├── Data/               # DbContext
│   │   ├── Features/           # CQRS (Commands/Queries)
│   │   │   └── Persons/
│   │   │       ├── Commands/
│   │   │       └── Queries/
│   │   ├── Mapping/            # Configuration Mapster
│   │   ├── Middleware/         # Gestionnaire d'exceptions
│   │   └── Models/             # Entités et DTOs
│   │
│   ├── localizationApp.Client/
│   │   ├── Components/         # Pages et composants Blazor
│   │   ├── Services/           # Services (HttpClient, Translation)
│   │   ├── Translations/       # Fichiers JSON de traduction
│   │   └── Validators/         # FluentValidation
│   │
│   └── localizationApp.Tests/  # Tests unitaires
│
├── docker-compose.yml
└── README.md
```

---

## Endpoints de l'API

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| GET | /api/persons | Lister toutes les personnes |
| GET | /api/persons/{id} | Récupérer une personne par ID |
| GET | /api/persons/list | Liste légère (DTO simplifié) |
| POST | /api/persons | Créer une personne |
| PUT | /api/persons/{id} | Modifier une personne |
| DELETE | /api/persons/{id} | Supprimer une personne |

---

## Déploiement Azure

### Architecture

```
┌──────────────────┐         ┌──────────────────┐
│   Azure App      │         │   Azure App      │
│   Service        │ ──────> │   Service        │
│                  │  HTTPS  │                  │
│   Client         │         │   API            │
│   (Tier gratuit) │         │   (Tier gratuit) │
└──────────────────┘         └──────────────────┘
```

### Étapes de déploiement

1. **Créer deux App Services** sur Azure (Tier gratuit F1)
   - `localizationapp-client`
   - `localizationapp-api`

2. **Configurer les variables d'environnement**

   **Pour le Client :**
   | Nom | Valeur |
   |-----|--------|
   | `ApiBaseUrl` | `https://localizationapp-api.azurewebsites.net` |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

   **Pour l'API :**
   | Nom | Valeur |
   |-----|--------|
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

3. **Configurer CORS sur l'API**

   L'API doit accepter les requêtes du Client. Dans `Program.cs` :
   ```csharp
   policy.WithOrigins(
       "https://localhost:7288",           // Dev local
       "http://localhost:5000",            // Docker
       "https://localizationapp-client.azurewebsites.net"  // Azure
   )
   ```

4. **Publier depuis Visual Studio**
   - Clic droit sur le projet → Publier
   - Sélectionner Azure App Service
   - Suivre l'assistant

### Problèmes courants et solutions

| Problème | Cause | Solution |
|----------|-------|----------|
| 404 Not Found | Mauvaise `ApiBaseUrl` | Vérifier la variable d'environnement dans Azure |
| CORS bloqué | URL Azure non autorisée | Ajouter l'URL dans la politique CORS, republier l'API |
| Erreur SQLite dans Docker | Dossier `/app/data` manquant | Ajouter `RUN mkdir -p /app/data` dans le Dockerfile |
| Données perdues au redémarrage | Pas de volume configuré | Ajouter le volume dans docker-compose.yml |

### Hiérarchie de configuration .NET

```
1. appsettings.json                 ← Configuration de base
2. appsettings.{Environment}.json   ← Spécifique à l'environnement
3. Variables d'environnement        ← Surcharge tout (Azure)
4. Arguments de ligne de commande   ← Priorité maximale
```

---

## Tests

Le projet contient 67 tests unitaires.

### Lancer les tests

```bash
cd src/localizationApp.Tests
dotnet test
```

### Couverture des tests

| Catégorie | Tests | Description |
|-----------|-------|-------------|
| API Commands | 33 | Create, Update, Delete (handlers + validators) |
| API Queries | 27 | GetAll, GetById, GetPersonsList |
| Localisation | 6 | Chargement JSON, fallback anglais |
| Validation Client | 1 | Validation des DTOs |

### Structure des tests

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

## Ce que ce projet démontre

- Solution personnalisée quand les outils standards ne suffisent pas
- Compréhension des compromis techniques
- Architecture modulaire (séparation par domaine)
- Focus sur la maintenabilité
- Conteneurisation avec Docker
- Déploiement cloud (Azure)
- Tests unitaires avec xUnit

---

## Licence

MIT

## Auteur

Thierry Leblanc
