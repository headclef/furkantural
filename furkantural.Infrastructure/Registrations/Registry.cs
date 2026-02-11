using furkantural.Application.Common.Behaviors;
using furkantural.Application.Repositories;
using furkantural.Application.Services.Abstract;
using furkantural.Application.Wrappers;
using furkantural.Infrastructure.Repositories;
using furkantural.Infrastructure.Services.Concrete;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace furkantural.Infrastructure.Registrations
{
    public static class Registry
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // MediatR + Pipeline
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(Registry).Assembly));
            services.AddValidatorsFromAssembly(typeof(Application.Features.Contact.Commands.SendContactFormCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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