# Furkan Tural - Персональный Портфолио Сайт

Современный многоязычный персональный портфолио-сайт, созданный на **ASP.NET Core 8 MVC**, с 3D-анимациями, защищённой контактной формой и профессиональным дизайном с поддержкой тёмной/светлой темы.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Лицензия](https://img.shields.io/badge/%D0%9B%D0%B8%D1%86%D0%B5%D0%BD%D0%B7%D0%B8%D1%8F-MIT-green)
![Версия](https://img.shields.io/badge/%D0%92%D0%B5%D1%80%D1%81%D0%B8%D1%8F-1.2.9-blue)

🌐 **Языки**: [Türkçe](README.md) | [English](README.en.md) | [Deutsch](README.de.md) | [Français](README.fr.md)

---

## ✨ Возможности

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
- Интеграция **MailKit** для транзакционных писем
- Двойная система email (уведомление админа + подтверждение пользователю)
- Защита **Cloudflare Turnstile** CAPTCHA
- Ограничение частоты запросов в памяти для защиты от спама
- HTML-шаблоны email с подстановкой переменных

### 🔒 Безопасность
- CSRF-защита с `[ValidateAntiForgeryToken]`
- Верификация ботов Cloudflare Turnstile
- Поддержка переадресованных заголовков для proxy/CDN окружений
- Безопасная конфигурация SMTP

### 📊 Логирование
- Логирование в базу данных через **Entity Framework Core**
- Уровни логов: Инфо, Успех, Предупреждение, Ошибка
- Отслеживание IP-адресов (с поддержкой Cloudflare)

---

## 🛠️ Технологический Стек

| Категория | Технология |
|-----------|------------|
| **Фреймворк** | ASP.NET Core 8 MVC |
| **База данных** | SQL Server + Entity Framework Core 8 |
| **Email** | MailKit 4.14 |
| **Безопасность** | Cloudflare Turnstile |
| **3D Графика** | Three.js |
| **Стилизация** | Vanilla CSS с CSS-переменными |
| **Локализация** | ASP.NET Core Localization с .resx файлами |

---

## 🚀 Начало Работы

### Требования
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server)

### Установка

1. **Клонировать репозиторий**
   ```bash
   git clone https://github.com/headclef/furkantural.git
   cd furkantural
   ```

2. **Настроить** `appsettings.json` с вашими настройками SMTP, базы данных и Turnstile

3. **Применить миграции**
   ```bash
   dotnet ef database update
   ```

4. **Запустить**
   ```bash
   dotnet run
   ```

---

## 📝 Лицензия

MIT Лицензия - см. [LICENSE](License) для деталей.

---

## 👤 Автор

**Furkan Tural** - [furkantural.com](https://furkantural.com) | [GitHub](https://github.com/headclef) | [LinkedIn](https://linkedin.com/in/furkantural)
