# Furkan Tural - Kişisel Portfolyo Web Sitesi

**ASP.NET Core 8 MVC** ile geliştirilmiş, 3D animasyonlar, güvenli iletişim formu ve karanlık/aydınlık tema desteği sunan modern, çok dilli kişisel portfolyo web sitesi.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Lisans](https://img.shields.io/badge/Lisans-MIT-green)
![Sürüm](https://img.shields.io/badge/S%C3%BCr%C3%BCm-1.2.9-blue)

🌐 **Dil Seçenekleri**: [English](README.en.md) | [Deutsch](README.de.md) | [Français](README.fr.md) | [Русский](README.ru.md)

---

## ✨ Özellikler

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
- Transaksiyonel e-postalar için **MailKit** entegrasyonu
- Çift e-posta sistemi (yönetici bildirimi + kullanıcı onayı)
- **Cloudflare Turnstile** CAPTCHA koruması
- Spam önleme için bellek içi hız sınırlama
- Yer tutucu değişkenli HTML e-posta şablonları

### 🔒 Güvenlik
- `[ValidateAntiForgeryToken]` ile CSRF koruması
- Cloudflare Turnstile bot doğrulaması
- Proxy/CDN ortamları için yönlendirilmiş başlık desteği
- Güvenli SMTP yapılandırması

### 📊 Loglama
- **Entity Framework Core** ile veritabanı destekli loglama
- Log seviyeleri: Bilgi, Başarı, Uyarı, Hata
- IP adresi takibi (Cloudflare desteği ile)

---

## 🛠️ Teknoloji Yığını

| Kategori | Teknoloji |
|----------|-----------|
| **Framework** | ASP.NET Core 8 MVC |
| **Veritabanı** | SQL Server + Entity Framework Core 8 |
| **E-posta** | MailKit 4.14 |
| **Güvenlik** | Cloudflare Turnstile |
| **3D Grafikler** | Three.js |
| **Stil** | Vanilla CSS ve CSS Değişkenleri |
| **Yerelleştirme** | ASP.NET Core Localization ve .resx dosyaları |

---

## 📁 Proje Yapısı

```
furkantural/
├── Controllers/
│   └── BaseController.cs          # Ana controller (Index, SendMail, Error, SetLanguage)
├── Data/
│   └── AppDbContext.cs            # Entity Framework DbContext
├── Migrations/                    # EF Core migration'ları
├── Models/
│   ├── ErrorViewModel.cs          # Hata sayfası modeli
│   ├── MailViewModel.cs           # İletişim formu modeli
│   ├── SmtpViewModel.cs           # SMTP yapılandırma modeli
│   ├── TurnstileViewModel.cs      # Turnstile yapılandırma modeli
│   ├── Log.cs                     # Log entity'si
│   └── ...
├── Resources/                     # Yerelleştirme dosyaları (.resx)
│   ├── SharedResource.*.resx      # Paylaşılan çeviriler
│   ├── Views.Base.Index.*.resx    # Index sayfası çevirileri
│   └── Views.Base.Error.*.resx    # Hata sayfası çevirileri
├── Services/
│   ├── Abstract/                  # Servis arayüzleri
│   │   ├── IEmailService.cs
│   │   ├── IEmailRateLimiter.cs
│   │   ├── ILogService.cs
│   │   └── ITurnstileService.cs
│   └── Concrete/                  # Servis uygulamaları
│       ├── EmailService.cs
│       ├── InMemoryEmailRateLimiter.cs
│       ├── LogService.cs
│       └── TurnstileService.cs
├── Views/
│   ├── Base/
│   │   ├── Index.cshtml           # Ana portfolyo sayfası
│   │   └── Error.cshtml           # Hata yönetim sayfası
│   └── Shared/
│       └── _Layout.cshtml         # Ana layout
├── wwwroot/
│   ├── css/
│   │   ├── site.css               # Ana stil dosyası
│   │   └── error.css              # Hata sayfası stilleri
│   ├── js/
│   │   ├── site.js                # Ana JavaScript
│   │   ├── site-3d.js             # Three.js 3D arka plan
│   │   ├── site-3d-ui.js          # 3D arayüz etkileşimleri
│   │   └── error.js               # Hata sayfası scriptleri
│   ├── templates/                 # HTML e-posta şablonları
│   ├── robots.txt
│   └── sitemap.xml
├── Program.cs                     # Uygulama giriş noktası
├── appsettings.json               # Yapılandırma
└── furkantural.csproj             # Proje dosyası
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
   
   `appsettings.json` dosyasını kendi ayarlarınızla güncelleyin:
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

3. **Veritabanı migration'larını uygulayın**
   ```bash
   dotnet ef database update
   ```

4. **Uygulamayı çalıştırın**
   ```bash
   dotnet run
   ```

5. **Tarayıcıda açın**
   ```
   https://localhost:5001
   ```

---

## ⚙️ Yapılandırma

### SMTP Ayarları
`appsettings.json` dosyasında e-posta gönderimini yapılandırın:

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
| `/` | Ana portfolyo sayfası: Hero, Hakkımda, Yetenekler, Projeler, Fiyatlandırma, İletişim |
| `/Base/Error/{statusCode}` | Özel hata sayfaları (404, 500, vb.) |

---

## 🧪 Geliştirme

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

**Furkan Tural**

- Web Sitesi: [furkantural.com](https://furkantural.com)
- LinkedIn: [furkantural](https://linkedin.com/in/furkantural)
- GitHub: [headclef](https://github.com/headclef)
- Instagram: [headclef](https://instagram.com/headclef)