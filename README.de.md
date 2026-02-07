# Furkan Tural - Persönliche Portfolio-Website

Eine moderne, mehrsprachige persönliche Portfolio-Website, entwickelt mit **ASP.NET Core 8 MVC**, mit 3D-Animationen, sicherem Kontaktformular und professionellem Design mit Dark/Light-Theme-Unterstützung.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Lizenz](https://img.shields.io/badge/Lizenz-MIT-green)
![Version](https://img.shields.io/badge/Version-1.0.1,7-blue)

🌐 **Sprachen**: [Türkçe](README.md) | [English](README.en.md) | [Français](README.fr.md) | [Русский](README.ru.md)

---

## ✨ Funktionen

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
- **MailKit**-Integration für transaktionale E-Mails
- Duales E-Mail-System (Admin-Benachrichtigung + Benutzerbestätigung)
- **Cloudflare Turnstile** CAPTCHA-Schutz
- In-Memory-Ratenbegrenzung zur Spam-Prävention
- HTML-E-Mail-Vorlagen mit Platzhalterersetzung

### 🔒 Sicherheit
- CSRF-Schutz mit `[ValidateAntiForgeryToken]`
- Cloudflare Turnstile Bot-Verifizierung
- Forwarded-Headers-Unterstützung für Proxy/CDN-Umgebungen
- Sichere SMTP-Konfiguration

### 📊 Protokollierung
- Datenbankgestützte Protokollierung über **Entity Framework Core**
- Log-Level: Info, Erfolg, Warnung, Fehler
- IP-Adressverfolgung (mit Cloudflare-Unterstützung)

---

## 🛠️ Technologie-Stack

| Kategorie | Technologie |
|-----------|-------------|
| **Framework** | ASP.NET Core 8 MVC |
| **Datenbank** | SQL Server + Entity Framework Core 8 |
| **E-Mail** | MailKit 4.14 |
| **Sicherheit** | Cloudflare Turnstile |
| **3D-Grafik** | Three.js |
| **Styling** | Vanilla CSS mit CSS-Variablen |
| **Lokalisierung** | ASP.NET Core Localization mit .resx-Dateien |

---

## 🚀 Erste Schritte

### Voraussetzungen
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)

### Installation

1. **Repository klonen**
   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Konfigurieren** Sie `appsettings.json` mit Ihren SMTP-, Datenbank- und Turnstile-Einstellungen

3. **Migrationen anwenden**
   ```bash
   dotnet ef database update
   ```

4. **Ausführen**
   ```bash
   dotnet run
   ```

---

## 📝 Lizenz

MIT-Lizenz - siehe [LICENSE](LICENSE) für Details.

---

## 👤 Autor

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
