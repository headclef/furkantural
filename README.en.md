# Furkan Tural - Personal Portfolio Website

A modern, multi-language personal portfolio website built with **ASP.NET Core 8 MVC**, structured with **Clean Architecture** and **CQRS** pattern. Features 3D animations, a secure contact form, encrypted configuration, and dark/light theme support.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green)
![Version](https://img.shields.io/badge/Version-2.6.0-blue)

🌐 **Languages**: [Türkçe](README.md) | [Deutsch](README.de.md) | [Français](README.fr.md) | [Русский](README.ru.md)

---

## ✨ Features

### 🏗️ Clean Architecture

- **Domain → Application → Infrastructure → Web** layered solution structure
- **MediatR** CQRS pattern (Command / Handler separation)
- **FluentValidation** pipeline behavior for validation
- Generic Repository and Unit of Work patterns
- `Result<T>` wrapper for consistent response model

### 🌐 Multi-Language Support

- **5 languages**: Turkish (default), English, German, French, Russian
- Cookie-based language persistence
- Localized resource files (`.resx`) for all views

### 🎨 Modern UI/UX

- Responsive design with CSS variables
- Dark/Light theme toggle with system preference detection
- Glassmorphism effects and smooth animations
- 3D animated background using **Three.js**

### 📬 Contact Form

- **MailKit** integration via MediatR Command/Handler flow
- Dual email system (admin notification + user confirmation)
- **Cloudflare Turnstile** CAPTCHA protection
- In-memory rate limiting for spam prevention
- **FluentValidation** server-side validation
- HTML email templates with placeholder substitution

### 🔒 Security

- CSRF protection with `[ValidateAntiForgeryToken]`
- Cloudflare Turnstile bot verification
- **AES encrypted configuration** (`$e$` prefix, `FT_ENCRYPTION_KEY` env var)
- `GlobalExceptionMiddleware` for centralized error handling
- Forwarded headers support for proxy/CDN environments
- Secure SMTP configuration

### 📊 Logging

- Database-backed logging via **Entity Framework Core**
- Log levels: Info, Success, Warning, Error
- IP address tracking (with Cloudflare support)

### 🎵 Music Showcase

- Dedicated section for songs released via DistroKid
- Spotify and YouTube Music integration
- Singer, songwriter, and producer credits
- Modern card design with album cover artwork

### 🧪 Unit Tests

- Comprehensive test suite with **xUnit**, **Moq**, and **FluentAssertions**
- Separate test projects for Application and Infrastructure layers
- Validation, pipeline behavior, handler, repository, middleware, and service tests

---

## 🛠️ Tech Stack

| Category | Technology |
|----------|------------|

| **Framework** | ASP.NET Core 8 MVC |
| **Architecture** | Clean Architecture, CQRS |
| **Database** | SQL Server + Entity Framework Core 8 |
| **Mediator** | MediatR 12.2 |
| **Validation** | FluentValidation 11.9 |
| **Email** | MailKit 4.14 |
| **Security** | Cloudflare Turnstile, AES Encryption |
| **3D Graphics** | Three.js |
| **Styling** | Vanilla CSS with CSS Variables |
| **Localization** | ASP.NET Core Localization with .resx files |
| **Testing** | xUnit 2.9, Moq 4.20, FluentAssertions 6.12 |

---

## 📁 Solution Structure

```plaintext
furkantural.sln
│
├── furkantural/                            # 🌐 Web (Presentation) Layer
│   ├── Controllers/
│   │   └── BaseController.cs               # Main controller with MediatR
│   ├── Models/
│   │   ├── ContactFormRequest.cs            # Contact form request model
│   │   └── ErrorViewModel.cs               # Error page model
│   ├── Views/
│   │   ├── Base/
│   │   │   ├── Index.cshtml                # Main portfolio page
│   │   │   └── Error.cshtml                # Error handling page
│   │   └── Shared/
│   │       └── _Layout.cshtml              # Master layout
│   ├── Resources/                          # Localization files (.resx)
│   ├── wwwroot/
│   │   ├── css/                            # Stylesheets
│   │   ├── js/                             # JavaScript (including Three.js)
│   │   └── templates/                      # HTML email templates
│   ├── Program.cs                          # Application entry point
│   └── appsettings.json                    # Encrypted configuration
│
├── furkantural.Application/                # 📋 Application Layer
│   ├── Common/
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs       # FluentValidation pipeline behavior
│   ├── Features/
│   │   └── Contact/
│   │       ├── Commands/
│   │       │   └── SendContactFormCommand.cs   # CQRS command
│   │       └── Validators/
│   │           └── SendContactFormValidator.cs # FluentValidation validator
│   ├── Models/                             # Options / configuration models
│   ├── Repositories/
│   │   ├── IRepository.cs                  # Generic repository interface
│   │   └── IUnitOfWork.cs                  # Unit of Work interface
│   ├── Services/
│   │   └── Abstract/                       # Service interfaces
│   └── Wrappers/
│       └── Result.cs                       # Result<T> wrapper
│
├── furkantural.Domain/                     # 🏛️ Domain Layer
│   ├── Entities/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Base entity class
│   │   └── Log.cs                          # Log entity
│   └── Enums/
│       └── EmailType.cs                    # Email type enum
│
├── furkantural.Infrastructure/             # ⚙️ Infrastructure Layer
│   ├── Configuration/
│   │   └── ConfigurationDecryptionExtensions.cs  # $e$ decryption
│   ├── Data/
│   │   └── AppDbContext.cs                 # Entity Framework DbContext
│   ├── Features/
│   │   └── Contact/
│   │       └── Handlers/
│   │           └── SendContactFormHandler.cs    # CQRS handler
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs    # Centralized error handling
│   ├── Migrations/                         # EF Core migrations
│   ├── Registrations/
│   │   └── Registry.cs                     # DI service registrations
│   ├── Repositories/
│   │   ├── Repository.cs                   # Generic repository implementation
│   │   └── UnitOfWork.cs                   # Unit of Work implementation
│   ├── Security/
│   │   └── EncryptionService.cs            # AES encryption service
│   └── Services/
│       └── Concrete/                       # Service implementations
│
├── furkantural.Application.Tests/          # 🧪 Application Tests
│   ├── Behaviors/                          # ValidationBehavior tests
│   ├── Validators/                         # SendContactFormValidator tests
│   └── Wrappers/                           # Result wrapper tests
│
├── furkantural.Infrastructure.Tests/       # 🧪 Infrastructure Tests
│   ├── Handlers/                           # SendContactFormHandler tests
│   ├── Middleware/                          # GlobalExceptionMiddleware tests
│   ├── Repositories/                       # Repository & UnitOfWork tests
│   ├── Security/                           # EncryptionService tests
│   └── Services/                           # Service tests
│
└── furkantural.Tools/                      # 🔧 CLI Tools
    └── Program.cs                          # appsettings encryption tool
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full edition)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Configure the application**

   Update `appsettings.json` with your settings. Sensitive values can be encrypted using `furkantural.Tools`:

   ```bash
   dotnet run --project furkantural.Tools -- <master-key>
   ```

   Encrypted values are stored with a `$e$` prefix and decrypted at runtime via the `FT_ENCRYPTION_KEY` environment variable.

3. **Set the environment variable** (if using encrypted configuration)

   ```bash
   export FT_ENCRYPTION_KEY="your-master-key"
   ```

4. **Apply migrations**

   ```bash
   dotnet ef database update --project furkantural.Infrastructure --startup-project furkantural
   ```

5. **Run the application**

   ```bash
   dotnet run --project furkantural
   ```

6. **Run the tests**

   ```bash
   dotnet test
   ```

---

## 📝 License

MIT License - see [LICENSE](License) for details.

---

## 👤 Author

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
