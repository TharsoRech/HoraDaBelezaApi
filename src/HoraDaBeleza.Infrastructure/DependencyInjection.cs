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

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISalaoRepository, SalaoRepository>();
        services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAvaliacaoRepository, AvaliacaoRepository>();
        services.AddScoped<IPlanoRepository, PlanoRepository>();
        services.AddScoped<IAssinaturaRepository, AssinaturaRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}
