using furkantural.Services.Abstract;
using furkantural.Services.Concrete;
using Microsoft.AspNetCore.Http;

namespace furkantural.Registrations
{
    public static class Registry
    {
        #region Methods
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Add services
            services.AddSingleton<IEmailRateLimiter, InMemoryEmailRateLimiter>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ILogService, LogService>();

            // TurnstileService can be scoped as it may depend on scoped services
            services.AddScoped<ITurnstileService, TurnstileService>();

            return services;
        }
        #endregion
    }
}