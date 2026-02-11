# Furkan Tural - Site Web Portfolio Personnel

Un site web portfolio personnel moderne et multilingue construit avec **ASP.NET Core 8 MVC**, structuré selon le modèle **Clean Architecture** et **CQRS**. Avec des animations 3D, un formulaire de contact sécurisé, une configuration chiffrée et un support thème sombre/clair.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Licence](https://img.shields.io/badge/Licence-MIT-green)
![Version](https://img.shields.io/badge/Version-2.6.0-blue)

🌐 **Langues**: [Türkçe](README.md) | [English](README.en.md) | [Deutsch](README.de.md) | [Русский](README.ru.md)

---

## ✨ Fonctionnalités

### 🏗️ Clean Architecture

- Structure de solution en couches **Domain → Application → Infrastructure → Web**
- Pattern CQRS avec **MediatR** (séparation Command / Handler)
- Comportement pipeline **FluentValidation** pour la validation
- Patterns Generic Repository et Unit of Work
- Wrapper `Result<T>` pour un modèle de réponse cohérent

### 🌐 Support Multilingue

- **5 langues** : Turc (par défaut), Anglais, Allemand, Français, Russe
- Persistance de la langue basée sur les cookies
- Fichiers de ressources localisés (`.resx`) pour toutes les vues

### 🎨 UI/UX Moderne

- Design responsive avec variables CSS
- Basculement thème sombre/clair avec détection des préférences système
- Effets glassmorphism et animations fluides
- Arrière-plan animé 3D utilisant **Three.js**

### 📬 Formulaire de Contact

- Intégration **MailKit** via le flux MediatR Command/Handler
- Système d'email double (notification admin + confirmation utilisateur)
- Protection CAPTCHA **Cloudflare Turnstile**
- Limitation de débit en mémoire pour la prévention du spam
- Validation côté serveur avec **FluentValidation**
- Modèles d'email HTML avec substitution de variables

### 🔒 Sécurité

- Protection CSRF avec `[ValidateAntiForgeryToken]`
- Vérification des bots Cloudflare Turnstile
- **Configuration chiffrée AES** (préfixe `$e$`, variable d'environnement `FT_ENCRYPTION_KEY`)
- `GlobalExceptionMiddleware` pour la gestion centralisée des erreurs
- Support des en-têtes transférés pour environnements proxy/CDN
- Configuration SMTP sécurisée

### 📊 Journalisation

- Journalisation basée sur la base de données via **Entity Framework Core**
- Niveaux de log : Info, Succès, Avertissement, Erreur
- Suivi d'adresse IP (avec support Cloudflare)

### 🎵 Vitrine Musicale

- Section dédiée aux chansons publiées via DistroKid
- Intégration Spotify et YouTube Music
- Crédits chanteur, auteur et producteur
- Design moderne en carte avec pochette d'album

### 🧪 Tests Unitaires

- Suite de tests complète avec **xUnit**, **Moq** et **FluentAssertions**
- Projets de test séparés pour les couches Application et Infrastructure
- Tests de validation, comportement pipeline, handler, repository, middleware et service

---

## 🛠️ Stack Technologique

| Catégorie | Technologie |
|-----------|-------------|

| **Framework** | ASP.NET Core 8 MVC |
| **Architecture** | Clean Architecture, CQRS |
| **Base de données** | SQL Server + Entity Framework Core 8 |
| **Médiateur** | MediatR 12.2 |
| **Validation** | FluentValidation 11.9 |
| **Email** | MailKit 4.14 |
| **Sécurité** | Cloudflare Turnstile, Chiffrement AES |
| **Graphiques 3D** | Three.js |
| **Style** | CSS Vanilla avec Variables CSS |
| **Localisation** | ASP.NET Core Localization avec fichiers .resx |
| **Tests** | xUnit 2.9, Moq 4.20, FluentAssertions 6.12 |

---

## 📁 Structure de la Solution

```plaintext
furkantural.sln
│
├── furkantural/                            # 🌐 Couche Web (Présentation)
│   ├── Controllers/
│   │   └── BaseController.cs               # Contrôleur principal avec MediatR
│   ├── Models/
│   │   ├── ContactFormRequest.cs            # Modèle de requête du formulaire
│   │   └── ErrorViewModel.cs               # Modèle de page d'erreur
│   ├── Views/
│   │   ├── Base/
│   │   │   ├── Index.cshtml                # Page portfolio principale
│   │   │   └── Error.cshtml                # Page de gestion des erreurs
│   │   └── Shared/
│   │       └── _Layout.cshtml              # Layout maître
│   ├── Resources/                          # Fichiers de localisation (.resx)
│   ├── wwwroot/
│   │   ├── css/                            # Feuilles de style
│   │   ├── js/                             # JavaScript (y compris Three.js)
│   │   └── templates/                      # Modèles d'email HTML
│   ├── Program.cs                          # Point d'entrée de l'application
│   └── appsettings.json                    # Configuration chiffrée
│
├── furkantural.Application/                # 📋 Couche Application
│   ├── Common/
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs       # Comportement pipeline FluentValidation
│   ├── Features/
│   │   └── Contact/
│   │       ├── Commands/
│   │       │   └── SendContactFormCommand.cs   # Commande CQRS
│   │       └── Validators/
│   │           └── SendContactFormValidator.cs # Validateur FluentValidation
│   ├── Models/                             # Modèles d'options / configuration
│   ├── Repositories/
│   │   ├── IRepository.cs                  # Interface Generic Repository
│   │   └── IUnitOfWork.cs                  # Interface Unit of Work
│   ├── Services/
│   │   └── Abstract/                       # Interfaces de services
│   └── Wrappers/
│       └── Result.cs                       # Wrapper Result<T>
│
├── furkantural.Domain/                     # 🏛️ Couche Domain
│   ├── Entities/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Classe entité de base
│   │   └── Log.cs                          # Entité Log
│   └── Enums/
│       └── EmailType.cs                    # Enum type d'email
│
├── furkantural.Infrastructure/             # ⚙️ Couche Infrastructure
│   ├── Configuration/
│   │   └── ConfigurationDecryptionExtensions.cs  # Déchiffrement $e$
│   ├── Data/
│   │   └── AppDbContext.cs                 # Entity Framework DbContext
│   ├── Features/
│   │   └── Contact/
│   │       └── Handlers/
│   │           └── SendContactFormHandler.cs    # Handler CQRS
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs    # Gestion centralisée des erreurs
│   ├── Migrations/                         # Migrations EF Core
│   ├── Registrations/
│   │   └── Registry.cs                     # Enregistrements de services DI
│   ├── Repositories/
│   │   ├── Repository.cs                   # Implémentation Generic Repository
│   │   └── UnitOfWork.cs                   # Implémentation Unit of Work
│   ├── Security/
│   │   └── EncryptionService.cs            # Service de chiffrement AES
│   └── Services/
│       └── Concrete/                       # Implémentations de services
│
├── furkantural.Application.Tests/          # 🧪 Tests Application
│   ├── Behaviors/                          # Tests ValidationBehavior
│   ├── Validators/                         # Tests SendContactFormValidator
│   └── Wrappers/                           # Tests wrapper Result
│
├── furkantural.Infrastructure.Tests/       # 🧪 Tests Infrastructure
│   ├── Handlers/                           # Tests SendContactFormHandler
│   ├── Middleware/                          # Tests GlobalExceptionMiddleware
│   ├── Repositories/                       # Tests Repository & UnitOfWork
│   ├── Security/                           # Tests EncryptionService
│   └── Services/                           # Tests de services
│
└── furkantural.Tools/                      # 🔧 Outils CLI
    └── Program.cs                          # Outil de chiffrement appsettings
```

---

## 🚀 Démarrage

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB ou édition complète)

### Installation

1. **Cloner le dépôt**

   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Configurer l'application**

   Mettez à jour `appsettings.json` avec vos paramètres. Les valeurs sensibles peuvent être chiffrées avec `furkantural.Tools` :

   ```bash
   dotnet run --project furkantural.Tools -- <clé-maître>
   ```

   Les valeurs chiffrées sont stockées avec le préfixe `$e$` et déchiffrées à l'exécution via la variable d'environnement `FT_ENCRYPTION_KEY`.

3. **Définir la variable d'environnement** (si configuration chiffrée)

   ```bash
   export FT_ENCRYPTION_KEY="votre-clé-maître"
   ```

4. **Appliquer les migrations**

   ```bash
   dotnet ef database update --project furkantural.Infrastructure --startup-project furkantural
   ```

5. **Exécuter l'application**

   ```bash
   dotnet run --project furkantural
   ```

6. **Exécuter les tests**

   ```bash
   dotnet test
   ```

---

## 📝 Licence

Licence MIT - voir [LICENSE](License) pour les détails.

---

## 👤 Auteur

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
