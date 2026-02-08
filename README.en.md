# Furkan Tural - Personal Portfolio Website

A modern, multi-language personal portfolio website built with **ASP.NET Core 8 MVC**, featuring 3D animations, a secure contact form, and a professional design with dark/light theme support.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-green)
![Version](https://img.shields.io/badge/Version-1.3.0-blue)

🌐 **Languages**: [Türkçe](README.md) | [Deutsch](README.de.md) | [Français](README.fr.md) | [Русский](README.ru.md)

---

## ✨ Features

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
- **MailKit** integration for transactional emails
- Dual email system (admin notification + user confirmation)
- **Cloudflare Turnstile** CAPTCHA protection
- In-memory rate limiting for spam prevention
- HTML email templates with placeholder substitution

### 🔒 Security
- CSRF protection with `[ValidateAntiForgeryToken]`
- Cloudflare Turnstile bot verification
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

---

## 🛠️ Tech Stack

| Category | Technology |
|----------|------------|
| **Framework** | ASP.NET Core 8 MVC |
| **Database** | SQL Server + Entity Framework Core 8 |
| **Email** | MailKit 4.14 |
| **Security** | Cloudflare Turnstile |
| **3D Graphics** | Three.js |
| **Styling** | Vanilla CSS with CSS Variables |
| **Localization** | ASP.NET Core Localization with .resx files |

---

## 📁 Project Structure

```
furkantural/
├── Controllers/
│   └── BaseController.cs          # Main controller (Index, SendMail, Error, SetLanguage)
├── Data/
│   └── AppDbContext.cs            # Entity Framework DbContext
├── Migrations/                    # EF Core migrations
├── Models/
│   ├── ErrorViewModel.cs          # Error page model
│   ├── MailViewModel.cs           # Contact form model
│   ├── SmtpViewModel.cs           # SMTP configuration model
│   ├── TurnstileViewModel.cs      # Turnstile config model
│   ├── Log.cs                     # Log entity
│   └── ...
├── Resources/                     # Localization files (.resx)
├── Services/
│   ├── Abstract/                  # Service interfaces
│   └── Concrete/                  # Service implementations
├── Views/
│   ├── Base/
│   │   ├── Index.cshtml           # Main portfolio page
│   │   └── Error.cshtml           # Error handling page
│   └── Shared/
│       └── _Layout.cshtml         # Master layout
├── wwwroot/
│   ├── css/                       # Stylesheets
│   ├── js/                        # JavaScript files (including Three.js)
│   └── templates/                 # HTML email templates
├── Program.cs                     # Application entry point
└── appsettings.json               # Configuration
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Configure** `appsettings.json` with your SMTP, database, and Turnstile settings

3. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run**
   ```bash
   dotnet run
   ```

---

## 📝 License

MIT License - see [LICENSE](License) for details.

---

## 👤 Author

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
