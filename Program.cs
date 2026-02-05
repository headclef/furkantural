using furkantural.Models;
using furkantural.Registrations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using furkantural.Data;

var builder = WebApplication.CreateBuilder(args);

#region Container Injections
// EmailSettings ayarlar iin gerekli modelin kayddr.
builder.Services.Configure<SmtpViewModel>(builder.Configuration.GetSection("Smtp"));

// EmailSettings ayarlar iin gerekli modelin kayddr.
builder.Services.Configure<AllowerViewModel>(builder.Configuration.GetSection("Allower"));

// CloudFlare Turnstile ayarlar iin gerekli modelin kayddr.
builder.Services.Configure<TurnstileViewModel>(builder.Configuration.GetSection("Turnstile"));

// Versiyon ynetimi iin model doldurumu
builder.Services.Configure<VersionViewModel>(builder.Configuration.GetSection("AppVersion"));

// Veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AddServices metodunu ararak AutoMapper ve dier servisleri ekle.
builder.Services.AddServices();

// HttpClient kayt iin gereklidir.
builder.Services.AddHttpClient();

// Tm controller'lar iin varsaylan yetkilendirme politikas belirle
builder.Services.AddControllersWithViews()
    .AddViewLocalization();

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
#endregion

var app = builder.Build();

#region App Configurations
// Proxy 'leri ne kar.
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
app.UseForwardedHeaders(forwardedOptions);

// Hatalar her ortamda hata sayfasna ynlenirmek iin gereklidir.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Base/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStatusCodePagesWithReExecute("/Base/Error/{0}");

app.Use(async (ctx, next) =>
{
    try { await next(); }
    catch (AntiforgeryValidationException)
    {
        ctx.Response.Clear();
        ctx.Response.StatusCode = 400;
        ctx.Request.Path = "/Base/Error/Code=400&Type=Bad%20Request&Message=Var%20bir%20sknt&Detail=Galiba%20bir%20problem%20olutu%20istersen%20tekrar%20dene%20heh?";
        await next();
    }
});

// Localization Middleware
var supportedCultures = new[] { "tr", "en", "de", "fr", "ru" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

// HTTP isteklerini HTTPS'e ynlendir
app.UseHttpsRedirection();

// Statik dosyalarn sunulmasn etkinletir
app.UseStaticFiles();

// Rota belirlemeyi etkinletir
app.UseRouting();

// Varsaylan controller rotasn belirle
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Base}/{action=Index}/{id?}");
#endregion

// Uygulamay altr
app.Run();
