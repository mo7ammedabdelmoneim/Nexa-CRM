using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexaCRM.Application.Contracts;
using NexaCRM.Domain.Repositories;
using NexaCRM.Infrastructure.Jobs;
using NexaCRM.Infrastructure.Persistence;
using NexaCRM.Infrastructure.Persistence.Repositories;
using NexaCRM.Infrastructure.Services;

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
        services.AddScoped<IDealRepository, DealRepository>();
        services.AddScoped<IDealQueryRepository, DealQueryRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IActivityQueryRepository, ActivityQueryRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskQueryRepository, TaskQueryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationQueryRepository, NotificationQueryRepository>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<INotificationService, NotificationService>();

        // SignalR
        services.AddSignalR();

        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("Default")));

        services.AddHangfireServer();

        // Jobs
        services.AddScoped<TaskReminderJob>();

        return services;
    }
}