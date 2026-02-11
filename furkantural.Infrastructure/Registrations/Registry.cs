using furkantural.Application.Repositories;
using furkantural.Application.Services.Abstract;
using furkantural.Infrastructure.Repositories;
using furkantural.Infrastructure.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace furkantural.Infrastructure.Registrations
{
    public static class Registry
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IEmailRateLimiter, InMemoryEmailRateLimiter>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpContextAccessor();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ITurnstileService, TurnstileService>();

            return services;
        }
    }
}