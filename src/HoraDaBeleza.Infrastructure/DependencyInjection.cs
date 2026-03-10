using HoraDaBeleza.Application.Interfaces;
using HoraDaBeleza.Infrastructure.Data;
using HoraDaBeleza.Infrastructure.Repositories;
using HoraDaBeleza.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HoraDaBeleza.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();

        services.AddScoped<IUserRepository,         UserRepository>();
        services.AddScoped<ISalonRepository,        SalonRepository>();
        services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
        services.AddScoped<IServiceRepository,      ServiceRepository>();
        services.AddScoped<IAppointmentRepository,  AppointmentRepository>();
        services.AddScoped<IReviewRepository,       ReviewRepository>();
        services.AddScoped<IPlanRepository,         PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ICategoryRepository,     CategoryRepository>();
        services.AddScoped<ITokenService,           JwtTokenService>();

        return services;
    }
}
