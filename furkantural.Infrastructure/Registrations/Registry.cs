using furkantural.Application.Services.Abstract;
using furkantural.Infrastructure.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace furkantural.Infrastructure.Registrations
{
    public static class Registry
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IEmailRateLimiter, InMemoryEmailRateLimiter>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ITurnstileService, TurnstileService>();

            return services;
        }
    }
}