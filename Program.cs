using furkantural.Data;
using furkantural.Models;
using furkantural.Registrations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

#region Container Injections
// Smtp 'yi ayarla
builder.Services.Configure<SmtpViewModel>(builder.Configuration.GetSection("Smtp"));

// Allower 'ı ayarla
builder.Services.Configure<AllowerViewModel>(builder.Configuration.GetSection("Allower"));

// Turnstile 'ı ayarla
builder.Services.Configure<TurnstileViewModel>(builder.Configuration.GetSection("Turnstile"));

// Versiyon yönetimi için model doldur
builder.Services.Configure<VersionViewModel>(builder.Configuration.GetSection("AppVersion"));

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