# Furkan Tural - Персональный Портфолио Сайт

Современный многоязычный персональный портфолио-сайт на **ASP.NET Core 8 MVC**, построенный по принципам **Clean Architecture** и паттерну **CQRS**. С 3D-анимациями, защищённой контактной формой, зашифрованной конфигурацией и поддержкой тёмной/светлой темы.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Лицензия](https://img.shields.io/badge/%D0%9B%D0%B8%D1%86%D0%B5%D0%BD%D0%B7%D0%B8%D1%8F-MIT-green)
![Версия](https://img.shields.io/badge/%D0%92%D0%B5%D1%80%D1%81%D0%B8%D1%8F-2.6.0-blue)

🌐 **Языки**: [Türkçe](README.md) | [English](README.en.md) | [Deutsch](README.de.md) | [Français](README.fr.md)

---

## ✨ Возможности

### 🏗️ Clean Architecture

- Слоистая структура решения **Domain → Application → Infrastructure → Web**
- Паттерн CQRS с **MediatR** (разделение Command / Handler)
- Pipeline-поведение **FluentValidation** для валидации
- Паттерны Generic Repository и Unit of Work
- Обёртка `Result<T>` для согласованной модели ответа

### 🌐 Многоязычная Поддержка

- **5 языков**: Турецкий (по умолчанию), Английский, Немецкий, Французский, Русский
- Сохранение языка на основе cookies
- Локализованные файлы ресурсов (`.resx`) для всех представлений

### 🎨 Современный UI/UX

- Адаптивный дизайн с CSS-переменными
- Переключение тёмной/светлой темы с определением системных настроек
- Эффекты glassmorphism и плавные анимации
- 3D-анимированный фон на **Three.js**

### 📬 Контактная Форма

- Интеграция **MailKit** через поток MediatR Command/Handler
- Двойная система email (уведомление админа + подтверждение пользователю)
- Защита **Cloudflare Turnstile** CAPTCHA
- Ограничение частоты запросов в памяти для защиты от спама
- Серверная валидация с **FluentValidation**
- HTML-шаблоны email с подстановкой переменных

### 🔒 Безопасность

- CSRF-защита с `[ValidateAntiForgeryToken]`
- Верификация ботов Cloudflare Turnstile
- **AES-шифрованная конфигурация** (префикс `$e$`, переменная окружения `FT_ENCRYPTION_KEY`)
- `GlobalExceptionMiddleware` для централизованной обработки ошибок
- Поддержка переадресованных заголовков для proxy/CDN окружений
- Безопасная конфигурация SMTP

### 📊 Логирование

- Логирование в базу данных через **Entity Framework Core**
- Уровни логов: Инфо, Успех, Предупреждение, Ошибка
- Отслеживание IP-адресов (с поддержкой Cloudflare)

### 🎵 Музыкальная Витрина

- Специальный раздел для песен, выпущенных через DistroKid
- Интеграция Spotify и YouTube Music
- Информация об исполнителе, авторе и продюсере
- Современный дизайн карточек с обложкой альбома

### 🧪 Модульные Тесты

- Полный набор тестов с **xUnit**, **Moq** и **FluentAssertions**
- Отдельные тестовые проекты для слоёв Application и Infrastructure
- Тесты валидации, pipeline-поведения, обработчиков, репозиториев, middleware и сервисов

---

## 🛠️ Технологический Стек

| Категория | Технология |
|-----------|------------|

| **Фреймворк** | ASP.NET Core 8 MVC |
| **Архитектура** | Clean Architecture, CQRS |
| **База данных** | SQL Server + Entity Framework Core 8 |
| **Медиатор** | MediatR 12.2 |
| **Валидация** | FluentValidation 11.9 |
| **Email** | MailKit 4.14 |
| **Безопасность** | Cloudflare Turnstile, AES-шифрование |
| **3D Графика** | Three.js |
| **Стилизация** | Vanilla CSS с CSS-переменными |
| **Локализация** | ASP.NET Core Localization с .resx файлами |
| **Тестирование** | xUnit 2.9, Moq 4.20, FluentAssertions 6.12 |

---

## 📁 Структура Решения

```plaintext
furkantural.sln
│
├── furkantural/                            # 🌐 Веб (Презентационный) Слой
│   ├── Controllers/
│   │   └── BaseController.cs               # Основной контроллер с MediatR
│   ├── Models/
│   │   ├── ContactFormRequest.cs            # Модель запроса контактной формы
│   │   └── ErrorViewModel.cs               # Модель страницы ошибки
│   ├── Views/
│   │   ├── Base/
│   │   │   ├── Index.cshtml                # Главная страница портфолио
│   │   │   └── Error.cshtml                # Страница обработки ошибок
│   │   └── Shared/
│   │       └── _Layout.cshtml              # Основной макет
│   ├── Resources/                          # Файлы локализации (.resx)
│   ├── wwwroot/
│   │   ├── css/                            # Таблицы стилей
│   │   ├── js/                             # JavaScript (включая Three.js)
│   │   └── templates/                      # HTML-шаблоны email
│   ├── Program.cs                          # Точка входа приложения
│   └── appsettings.json                    # Зашифрованная конфигурация
│
├── furkantural.Application/                # 📋 Слой Application
│   ├── Common/
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs       # Pipeline-поведение FluentValidation
│   ├── Features/
│   │   └── Contact/
│   │       ├── Commands/
│   │       │   └── SendContactFormCommand.cs   # CQRS-команда
│   │       └── Validators/
│   │           └── SendContactFormValidator.cs # Валидатор FluentValidation
│   ├── Models/                             # Модели опций / конфигурации
│   ├── Repositories/
│   │   ├── IRepository.cs                  # Интерфейс Generic Repository
│   │   └── IUnitOfWork.cs                  # Интерфейс Unit of Work
│   ├── Services/
│   │   └── Abstract/                       # Интерфейсы сервисов
│   └── Wrappers/
│       └── Result.cs                       # Обёртка Result<T>
│
├── furkantural.Domain/                     # 🏛️ Доменный Слой
│   ├── Entities/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs               # Базовый класс сущности
│   │   └── Log.cs                          # Сущность Log
│   └── Enums/
│       └── EmailType.cs                    # Перечисление типа email
│
├── furkantural.Infrastructure/             # ⚙️ Инфраструктурный Слой
│   ├── Configuration/
│   │   └── ConfigurationDecryptionExtensions.cs  # Расшифровка $e$
│   ├── Data/
│   │   └── AppDbContext.cs                 # Entity Framework DbContext
│   ├── Features/
│   │   └── Contact/
│   │       └── Handlers/
│   │           └── SendContactFormHandler.cs    # CQRS-обработчик
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs    # Централизованная обработка ошибок
│   ├── Migrations/                         # Миграции EF Core
│   ├── Registrations/
│   │   └── Registry.cs                     # Регистрации DI-сервисов
│   ├── Repositories/
│   │   ├── Repository.cs                   # Реализация Generic Repository
│   │   └── UnitOfWork.cs                   # Реализация Unit of Work
│   ├── Security/
│   │   └── EncryptionService.cs            # Сервис AES-шифрования
│   └── Services/
│       └── Concrete/                       # Реализации сервисов
│
├── furkantural.Application.Tests/          # 🧪 Тесты Application
│   ├── Behaviors/                          # Тесты ValidationBehavior
│   ├── Validators/                         # Тесты SendContactFormValidator
│   └── Wrappers/                           # Тесты обёртки Result
│
├── furkantural.Infrastructure.Tests/       # 🧪 Тесты Infrastructure
│   ├── Handlers/                           # Тесты SendContactFormHandler
│   ├── Middleware/                          # Тесты GlobalExceptionMiddleware
│   ├── Repositories/                       # Тесты Repository и UnitOfWork
│   ├── Security/                           # Тесты EncryptionService
│   └── Services/                           # Тесты сервисов
│
└── furkantural.Tools/                      # 🔧 CLI-инструменты
    └── Program.cs                          # Инструмент шифрования appsettings
```

---

## 🚀 Начало Работы

### Требования

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB или полная версия)

### Установка

1. **Клонировать репозиторий**

   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Настроить приложение**

   Обновите `appsettings.json` вашими параметрами. Конфиденциальные значения можно зашифровать с помощью `furkantural.Tools`:

   ```bash
   dotnet run --project furkantural.Tools -- <мастер-ключ>
   ```

   Зашифрованные значения хранятся с префиксом `$e$` и расшифровываются при запуске через переменную окружения `FT_ENCRYPTION_KEY`.

3. **Установить переменную окружения** (при использовании зашифрованной конфигурации)

   ```bash
   export FT_ENCRYPTION_KEY="ваш-мастер-ключ"
   ```

4. **Применить миграции**

   ```bash
   dotnet ef database update --project furkantural.Infrastructure --startup-project furkantural
   ```

5. **Запустить приложение**

   ```bash
   dotnet run --project furkantural
   ```

6. **Запустить тесты**

   ```bash
   dotnet test
   ```

---

## 📝 Лицензия

MIT Лицензия - см. [LICENSE](License) для деталей.

---

## 👤 Автор

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
