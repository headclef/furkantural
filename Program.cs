using furkantural.Models;
using furkantural.Registrations;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

#region Container Injections
// EmailSettings ayarlarý için gerekli modelin kaydýdýr.
builder.Services.Configure<SmtpViewModel>(builder.Configuration.GetSection("Smtp"));

// EmailSettings ayarlarý için gerekli modelin kaydýdýr.
builder.Services.Configure<AllowerViewModel>(builder.Configuration.GetSection("Allower"));

// CloudFlare Turnstile ayarlarý için gerekli modelin kaydýdýr.
builder.Services.Configure<TurnstileViewModel>(builder.Configuration.GetSection("Turnstile"));

// Versiyon yönetimi için model doldurumu
builder.Services.Configure<VersionViewModel>(builder.Configuration.GetSection("AppVersion"));

// AddServices metodunu çaðýrarak AutoMapper ve diðer servisleri ekle.
builder.Services.AddServices();

// HttpClient kayýt için gereklidir.
builder.Services.AddHttpClient();

// Tüm controller'lar için varsayýlan yetkilendirme politikasý belirle
builder.Services.AddControllersWithViews();
#endregion

var app = builder.Build();

#region App Configurations
// Proxy 'leri öne çýkar.
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
app.UseForwardedHeaders(forwardedOptions);

// Hatalarý her ortamda hata sayfasýna yönlenirmek için gereklidir.
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
        ctx.Request.Path = "/Base/Error/Code=400&Type=Bad%20Request&Message=Var%20bir%20sýkýntý&Detail=Galiba%20bir%20problem%20oluþtu%20istersen%20tekrar%20dene%20heh?";
        await next();
    }
});

// HTTP isteklerini HTTPS'e yönlendir
app.UseHttpsRedirection();

// Statik dosyalarýn sunulmasýný etkinleþtir
app.UseStaticFiles();

// Rota belirlemeyi etkinleþtir
app.UseRouting();

// Varsayýlan controller rotasýný belirle
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Base}/{action=Index}/{id?}");
#endregion

// Uygulamayý çalýþtýr
app.Run();