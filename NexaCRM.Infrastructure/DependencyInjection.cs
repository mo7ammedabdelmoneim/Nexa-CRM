using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Repositories;
using NexaCRM.Infrastructure.Persistence;
using NexaCRM.Infrastructure.Persistence.Repositories;

namespace NexaCRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Default"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<ILeadQueryRepository, LeadQueryRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}