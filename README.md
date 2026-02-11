# Furkan Tural - Kişisel Portfolyo Web Sitesi

**Clean Architecture** ve **CQRS** deseni ile yapılandırılmış, **ASP.NET Core 8 MVC** tabanlı modern, çok dilli kişisel portfolyo web sitesi. 3D animasyonlar, güvenli iletişim formu, şifreli yapılandırma ve karanlık/aydınlık tema desteği sunar.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Lisans](https://img.shields.io/badge/Lisans-MIT-green)
![Sürüm](https://img.shields.io/badge/S%C3%BCr%C3%BCm-2.6.0-blue)

🌐 **Dil Seçenekleri**: [English](README.en.md) | [Deutsch](README.de.md) | [Français](README.fr.md) | [Русский](README.ru.md)

---

## ✨ Özellikler

### 🏗️ Clean Architecture

- **Domain → Application → Infrastructure → Web** katmanlı çözüm yapısı
- **MediatR** ile CQRS deseni (Command / Handler ayrımı)
- **FluentValidation** ile doğrulama pipeline davranışı
- Generic Repository ve Unit of Work desenleri
- `Result<T>` sarmalayıcı ile tutarlı yanıt modeli

### 🌐 Çoklu Dil Desteği

- **5 dil**: Türkçe (varsayılan), İngilizce, Almanca, Fransızca, Rusça
- Çerez tabanlı dil tercihi saklama
- Tüm görünümler için yerelleştirilmiş kaynak dosyaları (`.resx`)

### 🎨 Modern Arayüz

- Duyarlı (responsive) tasarım ve CSS değişkenleri
- Sistem tercihini algılayan karanlık/aydınlık tema geçişi
- Glassmorphism efektleri ve akıcı animasyonlar
- **Three.js** ile 3D animasyonlu arka plan

### 📬 İletişim Formu

- MediatR Command/Handler akışı ile **MailKit** entegrasyonu
- Çift e-posta sistemi (yönetici bildirimi + kullanıcı onayı)
- **Cloudflare Turnstile** CAPTCHA koruması
- Spam önleme için bellek içi hız sınırlama
- **FluentValidation** ile sunucu tarafı doğrulama
- Yer tutucu değişkenli HTML e-posta şablonları

### 🔒 Güvenlik

- `[ValidateAntiForgeryToken]` ile CSRF koruması
- Cloudflare Turnstile bot doğrulaması
- **AES şifreli yapılandırma** (`$e$` öneki, `FT_ENCRYPTION_KEY` ortam değişkeni)
- `GlobalExceptionMiddleware` ile merkezi hata yönetimi
- Proxy/CDN ortamları için yönlendirilmiş başlık desteği
- Güvenli SMTP yapılandırması

### 📊 Loglama

- **Entity Framework Core** ile veritabanı destekli loglama
- Log seviyeleri: Bilgi, Başarı, Uyarı, Hata
- IP adresi takibi (Cloudflare desteği ile)

### 🎵 Müzik Vitrini

- DistroKid üzerinden yayınlanan şarkılar için özel bölüm
- Spotify ve YouTube Music entegrasyonu
- Şarkıcı, söz yazarı ve prodüktör bilgileri
- Albüm kapağı görselleri ile modern kart tasarımı

### 🧪 Birim Testleri

- **xUnit**, **Moq** ve **FluentAssertions** ile kapsamlı test paketi
- Application ve Infrastructure katmanları için ayrı test projeleri
- Doğrulama, pipeline davranışı, handler, repository, middleware ve servis testleri

---

## 🛠️ Teknoloji Yığını

| Kategori | Teknoloji |
|----------|-----------|

| **Framework** | ASP.NET Core 8 MVC |
| **Mimari** | Clean Architecture, CQRS |
| **Veritabanı** | SQL Server + Entity Framework Core 8 |
| **Medyatör** | MediatR 12.2 |
| **Doğrulama** | FluentValidation 11.9 |
| **E-posta** | MailKit 4.14 |
| **Güvenlik** | Cloudflare Turnstile, AES Şifreleme |
| **3D Grafikler** | Three.js |
| **Stil** | Vanilla CSS ve CSS Değişkenleri |
| **Yerelleştirme** | ASP.NET Core Localization ve .resx dosyaları |
| **Test** | xUnit 2.9, Moq 4.20, FluentAssertions 6.12 |

---

## 📁 Çözüm Yapısı

```plaintext
furkantural.sln
│
├── furkantural/                            # 🌐 Web (Sunum) Katmanı
│   ├── Controllers/
│   │   └── BaseController.cs               # MediatR ile ana controller
│   ├── Models/
│   │   ├── ContactFormRequest.cs            # İletişim formu istek modeli
│   │   └── ErrorViewModel.cs               # Hata sayfası modeli
│   ├── Views/
│   │   ├── Base/
│   │   │   ├── Index.cshtml                # Ana portfolyo sayfası
│   │   │   └── Error.cshtml                # Hata yönetim sayfası
│   │   └── Shared/
│   │       └── _Layout.cshtml              # Ana layout
│   ├── Resources/                          # Yerelleştirme dosyaları (.resx)
│   ├── wwwroot/
│   │   ├── css/                            # Stil dosyaları
│   │   ├── js/                             # JavaScript (Three.js dahil)
│   │   └── templates/                      # HTML e-posta şablonları
│   ├── Program.cs                          # Uygulama giriş noktası
│   └── appsettings.json                    # Şifreli yapılandırma
│
├── furkantural.Application/                # 📋 Uygulama Katmanı
│   ├── Common/
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs       # FluentValidation pipeline davranışı
│   ├── Features/
│   │   └── Contact/
│   │       ├── Commands/
│   │       │   └── SendContactFormCommand.cs   # CQRS komutu
│   │       └── Validators/
│   │           └── SendContactFormValidator.cs # FluentValidation doğrulayıcı
│   ├── Models/
│   │   ├── SmtpOptions.cs                  # SMTP yapılandırma seçenekleri
│   │   ├── TurnstileOptions.cs             # Turnstile yapılandırma seçenekleri
│   │   ├── AllowerOptions.cs               # Özellik açma/kapama seçenekleri
│   │   └── VersionOptions.cs               # Sürüm yönetimi seçenekleri
│   ├── Repositories/
│   │   ├── IRepository.cs                  # Generic repository arayüzü
│   │   └── IUnitOfWork.cs                  # Unit of Work arayüzü
│   ├── Services/
│   │   └── Abstract/                       # Servis arayüzleri
│   │       ├── IEmailService.cs
│   │       ├── IEmailRateLimiter.cs
│   │       ├── ILogService.cs
│   │       ├── ITurnstileService.cs
│   │       ├── IEncryptionService.cs
│   │       └── IDateTimeProvider.cs
│   └── Wrappers/
│       └── Result.cs                       # Result<T> sarmalayıcı
│
├── furkantural.Domain/                     # 🏛️ Domain Katmanı
│   ├── Entities/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Temel entity sınıfı
│   │   └── Log.cs                          # Log entity'si
│   └── Enums/
│       └── EmailType.cs                    # E-posta türü enum'u
│
├── furkantural.Infrastructure/             # ⚙️ Altyapı Katmanı
│   ├── Configuration/
│   │   └── ConfigurationDecryptionExtensions.cs  # $e$ şifre çözme
│   ├── Data/
│   │   └── AppDbContext.cs                 # Entity Framework DbContext
│   ├── Features/
│   │   └── Contact/
│   │       └── Handlers/
│   │           └── SendContactFormHandler.cs    # CQRS handler
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs    # Merkezi hata yönetimi
│   ├── Migrations/                         # EF Core migration'ları
│   ├── Registrations/
│   │   └── Registry.cs                     # DI servis kayıtları
│   ├── Repositories/
│   │   ├── Repository.cs                   # Generic repository uygulaması
│   │   └── UnitOfWork.cs                   # Unit of Work uygulaması
│   ├── Security/
│   │   └── EncryptionService.cs            # AES şifreleme servisi
│   └── Services/
│       └── Concrete/                       # Servis uygulamaları
│           ├── EmailService.cs
│           ├── InMemoryEmailRateLimiter.cs
│           ├── LogService.cs
│           ├── TurnstileService.cs
│           └── DateTimeProvider.cs
│
├── furkantural.Application.Tests/          # 🧪 Application Testleri
│   ├── Behaviors/
│   │   └── ValidationBehaviorTests.cs
│   ├── Validators/
│   │   └── SendContactFormValidatorTests.cs
│   └── Wrappers/
│       └── ResultTests.cs
│
├── furkantural.Infrastructure.Tests/       # 🧪 Infrastructure Testleri
│   ├── Handlers/
│   │   └── SendContactFormHandlerTests.cs
│   ├── Middleware/
│   │   └── GlobalExceptionMiddlewareTests.cs
│   ├── Repositories/
│   │   ├── RepositoryTests.cs
│   │   └── UnitOfWorkTests.cs
│   ├── Security/
│   │   └── EncryptionServiceTests.cs
│   └── Services/
│       ├── DateTimeProviderTests.cs
│       ├── InMemoryEmailRateLimiterTests.cs
│       └── LogServiceTests.cs
│
└── furkantural.Tools/                      # 🔧 CLI Araçları
    └── Program.cs                          # appsettings şifreleme aracı
```

---

## 🚀 Başlarken

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB veya tam sürüm)

### Kurulum

1. **Depoyu klonlayın**

   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Uygulamayı yapılandırın**

   `appsettings.json` dosyasını kendi ayarlarınızla güncelleyin. Hassas değerler `furkantural.Tools` ile şifrelenebilir:

   ```bash
   dotnet run --project furkantural.Tools -- <ana-anahtar>
   ```

   Şifreli değerler `$e$` öneki ile saklanır ve çalışma zamanında `FT_ENCRYPTION_KEY` ortam değişkeni ile çözülür.

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=furkantural_dev;..."
     },
     "Smtp": {
       "Host": "smtp-sunucunuz",
       "Port": 587,
       "SenderEmail": "gonderen@alanadi.com",
       "Password": "smtp-sifreniz",
       "ListenerEmail": "admin@alanadi.com"
     },
     "Turnstile": {
       "SiteKey": "turnstile-site-anahtariniz",
       "SecretKey": "turnstile-gizli-anahtariniz"
     }
   }
   ```

3. **Ortam değişkenini ayarlayın** (şifreli yapılandırma kullanıyorsanız)

   ```bash
   export FT_ENCRYPTION_KEY="ana-anahtariniz"
   ```

4. **Veritabanı migration'larını uygulayın**

   ```bash
   dotnet ef database update --project furkantural.Infrastructure --startup-project furkantural
   ```

5. **Uygulamayı çalıştırın**

   ```bash
   dotnet run --project furkantural
   ```

6. **Testleri çalıştırın**

   ```bash
   dotnet test
   ```

---

## ⚙️ Yapılandırma

### Şifreli Yapılandırma

Hassas ayarlar `$e$` öneki ile AES şifreli olarak `appsettings.json` içinde saklanır. Uygulama başlatılırken `FT_ENCRYPTION_KEY` ortam değişkeni ile otomatik olarak çözülür.

Değerleri şifrelemek için:

```bash
dotnet run --project furkantural.Tools -- <ana-anahtar>
```

### SMTP Ayarları

| Anahtar | Açıklama |
|---------|----------|

| `Host` | SMTP sunucu adresi |
| `Port` | SMTP portu (genellikle TLS için 587) |
| `SenderEmail` | Giden e-postalar için gönderen adresi |
| `ListenerEmail` | İletişim bildirimleri için yönetici e-postası |
| `PerHourLimit` | E-posta gönderimi için hız sınırı |

### Cloudflare Turnstile

[Cloudflare Dashboard](https://dash.cloudflare.com/)'tan anahtarları alın:

- `SiteKey`: Frontend widget için genel anahtar
- `SecretKey`: Sunucu doğrulaması için gizli anahtar

### Yerelleştirme

Desteklenen kültürler: `tr`, `en`, `de`, `fr`, `ru`

Yeni çeviri eklemek için:

1. `Resources/Views.Base.Index.{kultur}.resx` dosyası oluşturun
2. Kültür kodunu `Program.cs`'deki `supportedCultures` dizisine ekleyin

---

## 📄 Sayfalar

| Rota | Açıklama |
|------|----------|

| `/` | Ana portfolyo sayfası: Hero, Hakkımda, Yetenekler, Projeler, Şarkılar, Fiyatlandırma, İletişim |
| `/Base/Error/{statusCode}` | Özel hata sayfaları (404, 500, vb.) |

---

## 🧪 Geliştirme

### Yeni Özellik Ekleme (CQRS Akışı)

1. `furkantural.Application/Features/` altında Command sınıfı oluşturun
2. Aynı dizinde FluentValidation Validator ekleyin
3. `furkantural.Infrastructure/Features/` altında Handler yazın
4. Servisler DI kaydı için `Registry.cs`'yi kontrol edin

### Yeni Bölüm Ekleme

1. `Views/Base/Index.cshtml` dosyasına HTML ekleyin
2. Tüm `Views.Base.Index.*.resx` dosyalarına yerelleştirme anahtarları ekleyin
3. `wwwroot/css/site.css` dosyasında stil tanımlayın

### E-posta Şablonları

`wwwroot/templates/` dizininde bulunur:

- Yer tutucu değişken desteği: `{{NameSurname}}`, `{{Email}}`, vb.

---

## 📝 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır - detaylar için [LICENSE](License) dosyasına bakın.

---

## 👤 Geliştirici

### Furkan Tural

- Web Sitesi: [furkantural.com](https://furkantural.com)
- LinkedIn: [furkantural](https://linkedin.com/in/furkantural)
- GitHub: [headclef](https://github.com/headclef)
- Instagram: [headclef](https://instagram.com/headclef)
