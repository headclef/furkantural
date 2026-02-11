# Furkan Tural - Persönliche Portfolio-Website

Eine moderne, mehrsprachige persönliche Portfolio-Website, entwickelt mit **ASP.NET Core 8 MVC**, strukturiert nach **Clean Architecture** und **CQRS**-Muster. Mit 3D-Animationen, sicherem Kontaktformular, verschlüsselter Konfiguration und Dark/Light-Theme-Unterstützung.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Lizenz](https://img.shields.io/badge/Lizenz-MIT-green)
![Version](https://img.shields.io/badge/Version-2.6.0-blue)

🌐 **Sprachen**: [Türkçe](README.md) | [English](README.en.md) | [Français](README.fr.md) | [Русский](README.ru.md)

---

## ✨ Funktionen

### 🏗️ Clean Architecture

- **Domain → Application → Infrastructure → Web** geschichtete Lösungsstruktur
- **MediatR** CQRS-Muster (Command / Handler Trennung)
- **FluentValidation** Pipeline-Verhalten für Validierung
- Generic Repository und Unit of Work Muster
- `Result<T>` Wrapper für konsistentes Antwortmodell

### 🌐 Mehrsprachige Unterstützung

- **5 Sprachen**: Türkisch (Standard), Englisch, Deutsch, Französisch, Russisch
- Cookie-basierte Sprachpersistenz
- Lokalisierte Ressourcendateien (`.resx`) für alle Ansichten

### 🎨 Modernes UI/UX

- Responsives Design mit CSS-Variablen
- Dark/Light-Theme-Umschaltung mit Systemeinstellungserkennung
- Glassmorphism-Effekte und flüssige Animationen
- 3D-animierter Hintergrund mit **Three.js**

### 📬 Kontaktformular

- **MailKit**-Integration über MediatR Command/Handler-Fluss
- Duales E-Mail-System (Admin-Benachrichtigung + Benutzerbestätigung)
- **Cloudflare Turnstile** CAPTCHA-Schutz
- In-Memory-Ratenbegrenzung zur Spam-Prävention
- **FluentValidation** serverseitige Validierung
- HTML-E-Mail-Vorlagen mit Platzhalterersetzung

### 🔒 Sicherheit

- CSRF-Schutz mit `[ValidateAntiForgeryToken]`
- Cloudflare Turnstile Bot-Verifizierung
- **AES-verschlüsselte Konfiguration** (`$e$`-Präfix, `FT_ENCRYPTION_KEY` Umgebungsvariable)
- `GlobalExceptionMiddleware` für zentralisierte Fehlerbehandlung
- Forwarded-Headers-Unterstützung für Proxy/CDN-Umgebungen
- Sichere SMTP-Konfiguration

### 📊 Protokollierung

- Datenbankgestützte Protokollierung über **Entity Framework Core**
- Log-Level: Info, Erfolg, Warnung, Fehler
- IP-Adressverfolgung (mit Cloudflare-Unterstützung)

### 🎵 Musik-Showcase

- Dedizierter Bereich für über DistroKid veröffentlichte Songs
- Spotify und YouTube Music Integration
- Sänger, Songwriter und Produzenten-Credits
- Modernes Card-Design mit Albumcover-Artwork

### 🧪 Unit-Tests

- Umfassende Testsuite mit **xUnit**, **Moq** und **FluentAssertions**
- Separate Testprojekte für Application- und Infrastructure-Schichten
- Validierungs-, Pipeline-Verhaltens-, Handler-, Repository-, Middleware- und Service-Tests

---

## 🛠️ Technologie-Stack

| Kategorie | Technologie |
|-----------|-------------|

| **Framework** | ASP.NET Core 8 MVC |
| **Architektur** | Clean Architecture, CQRS |
| **Datenbank** | SQL Server + Entity Framework Core 8 |
| **Mediator** | MediatR 12.2 |
| **Validierung** | FluentValidation 11.9 |
| **E-Mail** | MailKit 4.14 |
| **Sicherheit** | Cloudflare Turnstile, AES-Verschlüsselung |
| **3D-Grafik** | Three.js |
| **Styling** | Vanilla CSS mit CSS-Variablen |
| **Lokalisierung** | ASP.NET Core Localization mit .resx-Dateien |
| **Testing** | xUnit 2.9, Moq 4.20, FluentAssertions 6.12 |

---

## 📁 Lösungsstruktur

```plaintext
furkantural.sln
│
├── furkantural/                            # 🌐 Web (Präsentations-) Schicht
│   ├── Controllers/
│   │   └── BaseController.cs               # Haupt-Controller mit MediatR
│   ├── Models/
│   │   ├── ContactFormRequest.cs            # Kontaktformular-Anforderungsmodell
│   │   └── ErrorViewModel.cs               # Fehlerseitenmodell
│   ├── Views/
│   │   ├── Base/
│   │   │   ├── Index.cshtml                # Haupt-Portfolio-Seite
│   │   │   └── Error.cshtml                # Fehlerbehandlungsseite
│   │   └── Shared/
│   │       └── _Layout.cshtml              # Master-Layout
│   ├── Resources/                          # Lokalisierungsdateien (.resx)
│   ├── wwwroot/
│   │   ├── css/                            # Stylesheets
│   │   ├── js/                             # JavaScript (inkl. Three.js)
│   │   └── templates/                      # HTML-E-Mail-Vorlagen
│   ├── Program.cs                          # Anwendungseinstiegspunkt
│   └── appsettings.json                    # Verschlüsselte Konfiguration
│
├── furkantural.Application/                # 📋 Application-Schicht
│   ├── Common/
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs       # FluentValidation Pipeline-Verhalten
│   ├── Features/
│   │   └── Contact/
│   │       ├── Commands/
│   │       │   └── SendContactFormCommand.cs   # CQRS-Befehl
│   │       └── Validators/
│   │           └── SendContactFormValidator.cs # FluentValidation-Validator
│   ├── Models/                             # Options- / Konfigurationsmodelle
│   ├── Repositories/
│   │   ├── IRepository.cs                  # Generic Repository Interface
│   │   └── IUnitOfWork.cs                  # Unit of Work Interface
│   ├── Services/
│   │   └── Abstract/                       # Service-Interfaces
│   └── Wrappers/
│       └── Result.cs                       # Result<T> Wrapper
│
├── furkantural.Domain/                     # 🏛️ Domain-Schicht
│   ├── Entities/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Basis-Entity-Klasse
│   │   └── Log.cs                          # Log-Entity
│   └── Enums/
│       └── EmailType.cs                    # E-Mail-Typ Enum
│
├── furkantural.Infrastructure/             # ⚙️ Infrastruktur-Schicht
│   ├── Configuration/
│   │   └── ConfigurationDecryptionExtensions.cs  # $e$ Entschlüsselung
│   ├── Data/
│   │   └── AppDbContext.cs                 # Entity Framework DbContext
│   ├── Features/
│   │   └── Contact/
│   │       └── Handlers/
│   │           └── SendContactFormHandler.cs    # CQRS-Handler
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs    # Zentralisierte Fehlerbehandlung
│   ├── Migrations/                         # EF Core Migrationen
│   ├── Registrations/
│   │   └── Registry.cs                     # DI-Service-Registrierungen
│   ├── Repositories/
│   │   ├── Repository.cs                   # Generic Repository Implementierung
│   │   └── UnitOfWork.cs                   # Unit of Work Implementierung
│   ├── Security/
│   │   └── EncryptionService.cs            # AES-Verschlüsselungsdienst
│   └── Services/
│       └── Concrete/                       # Service-Implementierungen
│
├── furkantural.Application.Tests/          # 🧪 Application-Tests
│   ├── Behaviors/                          # ValidationBehavior-Tests
│   ├── Validators/                         # SendContactFormValidator-Tests
│   └── Wrappers/                           # Result-Wrapper-Tests
│
├── furkantural.Infrastructure.Tests/       # 🧪 Infrastructure-Tests
│   ├── Handlers/                           # SendContactFormHandler-Tests
│   ├── Middleware/                          # GlobalExceptionMiddleware-Tests
│   ├── Repositories/                       # Repository- & UnitOfWork-Tests
│   ├── Security/                           # EncryptionService-Tests
│   └── Services/                           # Service-Tests
│
└── furkantural.Tools/                      # 🔧 CLI-Werkzeuge
    └── Program.cs                          # appsettings-Verschlüsselungstool
```

---

## 🚀 Erste Schritte

### Voraussetzungen

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB oder Vollversion)

### Installation

1. **Repository klonen**

   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Anwendung konfigurieren**

   Aktualisieren Sie `appsettings.json` mit Ihren Einstellungen. Sensible Werte können mit `furkantural.Tools` verschlüsselt werden:

   ```bash
   dotnet run --project furkantural.Tools -- <master-schlüssel>
   ```

   Verschlüsselte Werte werden mit dem Präfix `$e$` gespeichert und zur Laufzeit über die Umgebungsvariable `FT_ENCRYPTION_KEY` entschlüsselt.

3. **Umgebungsvariable setzen** (bei verschlüsselter Konfiguration)

   ```bash
   export FT_ENCRYPTION_KEY="ihr-master-schlüssel"
   ```

4. **Migrationen anwenden**

   ```bash
   dotnet ef database update --project furkantural.Infrastructure --startup-project furkantural
   ```

5. **Anwendung ausführen**

   ```bash
   dotnet run --project furkantural
   ```

6. **Tests ausführen**

   ```bash
   dotnet test
   ```

---

## 📝 Lizenz

MIT-Lizenz - siehe [LICENSE](License) für Details.

---

## 👤 Autor

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
