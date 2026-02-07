# Furkan Tural - Site Web Portfolio Personnel

Un site web portfolio personnel moderne et multilingue construit avec **ASP.NET Core 8 MVC**, avec des animations 3D, un formulaire de contact sécurisé et un design professionnel avec support thème sombre/clair.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Licence](https://img.shields.io/badge/Licence-MIT-green)
![Version](https://img.shields.io/badge/Version-1.2.7-blue)

🌐 **Langues**: [Türkçe](README.md) | [English](README.en.md) | [Deutsch](README.de.md) | [Русский](README.ru.md)

---

## ✨ Fonctionnalités

### 🌐 Support Multilingue
- **5 langues**: Turc (par défaut), Anglais, Allemand, Français, Russe
- Persistance de la langue basée sur les cookies
- Fichiers de ressources localisés (`.resx`) pour toutes les vues

### 🎨 UI/UX Moderne
- Design responsive avec variables CSS
- Basculement thème sombre/clair avec détection des préférences système
- Effets glassmorphism et animations fluides
- Arrière-plan animé 3D utilisant **Three.js**

### 📬 Formulaire de Contact
- Intégration **MailKit** pour les emails transactionnels
- Système d'email double (notification admin + confirmation utilisateur)
- Protection CAPTCHA **Cloudflare Turnstile**
- Limitation de débit en mémoire pour la prévention du spam
- Modèles d'email HTML avec substitution de variables

### 🔒 Sécurité
- Protection CSRF avec `[ValidateAntiForgeryToken]`
- Vérification des bots Cloudflare Turnstile
- Support des en-têtes transférés pour environnements proxy/CDN
- Configuration SMTP sécurisée

### 📊 Journalisation
- Journalisation basée sur la base de données via **Entity Framework Core**
- Niveaux de log: Info, Succès, Avertissement, Erreur
- Suivi d'adresse IP (avec support Cloudflare)

---

## 🛠️ Stack Technologique

| Catégorie | Technologie |
|-----------|-------------|
| **Framework** | ASP.NET Core 8 MVC |
| **Base de données** | SQL Server + Entity Framework Core 8 |
| **Email** | MailKit 4.14 |
| **Sécurité** | Cloudflare Turnstile |
| **Graphiques 3D** | Three.js |
| **Style** | CSS Vanilla avec Variables CSS |
| **Localisation** | ASP.NET Core Localization avec fichiers .resx |

---

## 🚀 Démarrage

### Prérequis
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)

### Installation

1. **Cloner le dépôt**
   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Configurer** `appsettings.json` avec vos paramètres SMTP, base de données et Turnstile

3. **Appliquer les migrations**
   ```bash
   dotnet ef database update
   ```

4. **Exécuter**
   ```bash
   dotnet run
   ```

---

## 📝 Licence

Licence MIT - voir [LICENSE](LICENSE) pour les détails.

---

## 👤 Auteur

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
