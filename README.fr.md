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
├── Person.nl.json
├── Health.en.json    ← Labels du tableau de santé
├── Health.fr.json
└── Health.nl.json
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
- Tableau de santé — surveillance visuelle du statut de l'API et de la base de données
- Validation avec FluentValidation
- Interface responsive avec Bootstrap
- Affichage du numéro de version

### API REST
- Architecture CQRS avec MediatR
- Vertical Slice Architecture
- FluentValidation avec Pipeline Behavior
- Health checks (`/health`, `/ready`) + informations du service (`/api/info`)
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

## Tableau de santé

### Pourquoi une page santé ?

En production, les health checks sont invisibles. C'est Docker, Kubernetes ou Azure qui les appellent automatiquement en arrière-plan pour savoir si l'application tourne et si ses dépendances (base de données, services externes, etc.) sont accessibles. Si quelque chose ne va pas, l'infrastructure peut redémarrer le conteneur ou arrêter de lui envoyer du trafic — le tout sans intervention humaine.

Le souci, c'est qu'on ne voit rien de tout ça juste en lisant le code. Alors j'ai ajouté une page "Tableau de santé" dans le client Blazor qui appelle exactement les mêmes endpoints que l'infrastructure utiliserait. C'est à des fins de démonstration, mais ça montre deux choses en même temps : l'implémentation côté backend (middleware de health check, pattern Options, contrôleur dédié) et le travail côté frontend (appels REST depuis Blazor, gestion des états de chargement, affichage des résultats avec des indicateurs visuels).

### Ce que le tableau affiche

La page est accessible à `/health` sur le client. Elle affiche deux cartes de statut :

- **Statut API** — appelle `/health` sur l'API. C'est le check de vivacité (liveness) : il confirme que le processus tourne.
- **Base de données** — appelle `/ready` sur l'API. C'est le check de disponibilité (readiness) : il essaie vraiment de requêter SQLite pour confirmer que la base est joignable.

En dessous, un tableau récupère les informations du service via `/api/info` : le nom du service, l'environnement en cours, et un horodatage UTC.

Chaque carte passe au vert ou au rouge selon le résultat, et affiche le temps de réponse en millisecondes. Un bouton de rafraîchissement permet de tout relancer à la demande.

### Testez par vous-même

Une fois l'application lancée (Docker ou Visual Studio), tu peux appeler chaque endpoint directement :

| Endpoint | URL (Docker) | Ce qu'il renvoie |
|----------|--------------|-------------------|
| Vivacité | http://localhost:5001/health | `Healthy` si le processus API tourne |
| Disponibilité | http://localhost:5001/ready | `Healthy` si la connexion à la base fonctionne |
| Infos service | http://localhost:5001/api/info | JSON avec nom du service, environnement, horodatage |
| Tableau de bord | http://localhost:5000/health | Page visuelle qui appelle les trois endpoints ci-dessus |

Depuis la ligne de commande :

```bash
# L'API est-elle vivante ?
curl http://localhost:5001/health

# La base de données est-elle joignable ?
curl http://localhost:5001/ready

# Infos du service (réponse JSON)
curl http://localhost:5001/api/info
```

### Comment c'est construit

**Côté API** — Les health checks utilisent le package intégré `Microsoft.Extensions.Diagnostics.HealthChecks`. Deux vérifications sont enregistrées avec des tags différents : une vérification "self-live" qui retourne toujours Healthy (le processus est debout), et une vérification "database-ready" qui appelle `CanConnect()` sur le contexte SQLite. Le endpoint `/health` filtre sur le tag `live`, `/ready` filtre sur le tag `ready`. Le endpoint `/api/info` est un contrôleur séparé qui lit depuis `AppOptions` (configuré dans `appsettings.json`, surchargeable via variables d'environnement).

**Côté client** — Un `HealthService` appelle ces trois endpoints et enveloppe chaque appel dans un `Stopwatch` pour mesurer la latence. La page `HealthDashboard.razor` consomme ce service et affiche les résultats. Comme tout le reste dans l'app, tous les labels et messages passent par le système de traduction (`Health.en.json`, `Health.fr.json`, `Health.nl.json`).

### HEALTHCHECK Docker

Le Dockerfile de l'API inclut une instruction `HEALTHCHECK` :

```dockerfile
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1
```

Docker exécute ça toutes les 30 secondes. Si ça échoue 3 fois de suite, le conteneur passe en "unhealthy". Dans `docker-compose.yml`, le service client utilise `depends_on` avec `condition: service_healthy`, ce qui veut dire qu'il ne démarrera pas tant que l'API n'a pas passé son premier health check. Plus de situation où le client démarre et essaie d'appeler une API qui n'est pas encore prête.

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

Attends que la construction se termine. Tu verras l'API démarrer, passer son health check, puis le client démarrera automatiquement. Quand c'est prêt, ouvre :
- Client : http://localhost:5000
- API : http://localhost:5001/api/persons
- Tableau de santé : http://localhost:5000/health

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
│                                                         │
│  Le client attend le health check de l'API avant        │
│  de démarrer (depends_on → condition: service_healthy)   │
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
│   │   ├── Controllers/        # Contrôleurs API (+ InfoController)
│   │   ├── Data/               # DbContext
│   │   ├── Features/           # CQRS (Commands/Queries)
│   │   │   └── Persons/
│   │   │       ├── Commands/
│   │   │       └── Queries/
│   │   ├── Mapping/            # Configuration Mapster
│   │   ├── Middleware/         # Gestionnaire d'exceptions
│   │   └── Models/             # Entités, DTOs, AppOptions
│   │
│   ├── localizationApp.Client/
│   │   ├── Components/         # Pages et composants Blazor
│   │   │   └── Pages/
│   │   │       └── HealthDashboard.razor
│   │   ├── Services/           # Services (HttpClient, Translation, Health)
│   │   ├── Translations/       # Fichiers JSON de traduction (incl. Health.xx.json)
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
| GET | /health | Check de vivacité (le processus API tourne-t-il ?) |
| GET | /ready | Check de disponibilité (la base est-elle joignable ?) |
| GET | /api/info | Nom du service, environnement, horodatage |

---

## Déploiement Azure

### C'est quoi Azure ?

Azure est la plateforme cloud de Microsoft. Elle offre plein de services, mais pour ce projet on utilise **Azure App Service** - un service d'hébergement managé pour les applications web. Tu déploies ton code, Azure s'occupe du reste (serveurs, mises à jour, scaling).

Plus d'infos : https://azure.microsoft.com/fr-fr

### Pourquoi App Service ?

| Service | Cas d'usage |
|---------|-------------|
| Virtual Machines | Contrôle total, tu gères tout |
| **App Service** | Option la plus simple, pas d'infrastructure à gérer |
| Kubernetes (AKS) | Orchestration avancée de conteneurs |
| Container Apps | Conteneurs simplifiés sans Kubernetes |
| Functions | Code à la demande (événements, webhooks) |

Pour un projet démo comme celui-ci, App Service avec le tier gratuit est parfait.

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

### Portail Azure

![Portail Azure](screenshots/Screenshot_8.png)

Le portail Azure est l'endroit où tu gères toutes tes ressources. Va dans **App Services** pour voir tes applications web.

### Nos App Services

![Liste des App Services](screenshots/Screenshot_9.png)

On a deux App Services :
- `localizationapp-api` - L'API REST
- `localizationapp-client` - Le frontend Blazor

Les deux sont hébergés dans la région **Belgium Central**, utilisent le tier **Gratuit**, et partagent le même App Service Plan.

### Convention de nommage

En suivant les [bonnes pratiques Azure](https://learn.microsoft.com/fr-be/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming) :

| Ressource | Nom | Pattern |
|-----------|-----|---------|
| Resource Group | `rg-localizationapp-dev` | `rg-{app}-{env}` |
| App Service Plan | `asp-localizationapp-dev-01` | `asp-{app}-{env}-{number}` |
| Web App (API) | `localizationapp-api` | `{app}-{role}` |
| Web App (Client) | `localizationapp-client` | `{app}-{role}` |

### Configuration de l'App Service

![Détails App Service](screenshots/Screenshot_10.png)

Les paramètres importants :
- **Flux de journaux** - Voir les logs en temps réel
- **Variables d'environnement** - Configurer les paramètres de l'app
- **Domaine par défaut** - L'URL publique de ton app

### Déploiement depuis Visual Studio

![Visual Studio Publish](screenshots/Screenshot_11.png)

1. Clic droit sur le projet → **Publier**
2. Sélectionner **Azure** → **Azure App Service (Windows)**
3. Sélectionner ton abonnement et ton App Service
4. Cliquer sur **Publier**

Le profil de publication est sauvegardé dans ton projet pour les futurs déploiements.

### Étapes de déploiement

Tout se fait directement depuis Visual Studio (sauf la création du compte Azure).

#### 1. Créer un compte Azure

Va sur https://azure.microsoft.com/fr-fr et crée un compte gratuit si tu n'en as pas.

#### 2. Publier l'API depuis Visual Studio

1. Clic droit sur `localizationApp.API` → **Publier**
2. Sélectionner **Azure** → **Suivant**
3. Sélectionner **Azure App Service (Windows)** → **Suivant**
4. Cliquer sur **Créer un nouvel Azure App Service**
5. Remplir le formulaire :
   - **Nom** : `localizationapp-api`
   - **Groupe de ressources** : Cliquer sur **Nouveau** → `rg-localizationapp-dev`
   - **Plan d'hébergement** : Cliquer sur **Nouveau** → `asp-localizationapp-dev-01`, sélectionner **Free F1**
6. Cliquer sur **Créer**
7. Cliquer sur **Terminer** puis **Publier**

#### 3. Publier le Client depuis Visual Studio

1. Clic droit sur `localizationApp.Client` → **Publier**
2. Même processus que l'API
3. **Important** : Sélectionner le **même Groupe de ressources** et le **même Plan d'hébergement** (économies !)
4. **Nom** : `localizationapp-client`
5. Cliquer sur **Créer** puis **Publier**

#### 4. Configurer les variables d'environnement

Après la publication, va sur le portail Azure pour configurer les variables d'environnement.

**Pour le Client :**

Aller dans App Service → **Paramètres** → **Variables d'environnement**

| Nom | Valeur |
|-----|--------|
| `ApiBaseUrl` | `https://localizationapp-api.azurewebsites.net` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

**Pour l'API :**

| Nom | Valeur |
|-----|--------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |

#### 5. Configurer CORS

L'API doit accepter les requêtes du Client. Dans `Program.cs` :

```csharp
policy.WithOrigins(
    "https://localhost:7288",           // Dev local
    "http://localhost:5000",            // Docker
    "https://localizationapp-client.azurewebsites.net"  // Azure
)
```

**Important** : Chaque fois que tu changes l'URL du Client, tu dois mettre à jour CORS sur l'API et republier.

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
3. Variables d'environnement        ← Surcharge tout (Azure utilise ça)
4. Arguments de ligne de commande   ← Priorité maximale
```

### Tarifs

| Plan | Prix | Cas d'usage |
|------|------|-------------|
| Free F1 | Gratuit | Tests, apprentissage |
| Basic B1 | ~13€/mois | Dev, petites apps |
| Standard S1 | ~70€/mois | Production |

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
- Health checks et surveillance de l'infrastructure
- Conteneurisation Docker avec orchestration de services
- Déploiement cloud (Azure)
- Tests unitaires avec xUnit
- Focus sur la maintenabilité

---

## Licence

MIT

## Auteur

Thierry Leblanc
