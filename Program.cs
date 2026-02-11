using furkantural.Infrastructure.Data;
using furkantural.Application.Models;
using furkantural.Infrastructure.Registrations;
using furkantural.Infrastructure.Middleware;
using furkantural.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Decrypt encrypted configuration values (requires FT_ENCRYPTION_KEY env var)
builder.Configuration.DecryptEncryptedValues();

#region Container Injections
// Smtp 'yi ayarla
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

// Allower 'ı ayarla
builder.Services.Configure<AllowerOptions>(builder.Configuration.GetSection("Allower"));

// Turnstile 'ı ayarla
builder.Services.Configure<TurnstileOptions>(builder.Configuration.GetSection("Turnstile"));

// Versiyon yönetimi için model doldur
builder.Services.Configure<VersionOptions>(builder.Configuration.GetSection("AppVersion"));

// Veritabanına bağlan
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AddServices metodunu arayarak AutoMapper ve diğer servisleri ekle
builder.Services.AddServices();

// HttpClient kaydı
builder.Services.AddHttpClient();

// Controller ve View kaydı
builder.Services.AddControllersWithViews()
    .AddViewLocalization();

// Dil desteği
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
#endregion

var app = builder.Build();

#region App Configurations
// Proxy 'leri belirle
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
app.UseForwardedHeaders(forwardedOptions);

// Hatalar her ortamda hata sayfasna yönlendir
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseStatusCodePagesWithReExecute("/Base/Error/{0}");

// Dil desteği
var supportedCultures = new[] { "tr", "en", "de", "fr", "ru" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

// HTTP isteklerini HTTPS'e yönlendir
app.UseHttpsRedirection();

// Statik dosyaların sunulmasını etkinleştir
app.UseStaticFiles();

// Rota belirlemeyi etkinletir
app.UseRouting();

// Varsayılan controller rotasını belirle
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Base}/{action=Index}/{id?}");
#endregion

// Uygulamayı çalıştır
app.Run();