using Hangfire;
using NexaCRM.Application;
using NexaCRM.Infrastructure;
using NexaCRM.API.Middleware;
using NexaCRM.Infrastructure.Hubs;
using NexaCRM.Infrastructure.Jobs;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting NexaCRM API...");

    var builder = WebApplication.CreateBuilder(args);

    // Serilog 
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId());

    // Services 
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "NexaCRM API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new()
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token here."
        });
        c.AddSecurityRequirement(new()
        {
            {
                new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
                []
            }
        });
    });

    // App 
    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHangfireDashboard("/jobs");
    app.MapControllers();
    app.MapHub<NotificationHub>("/hubs/notifications");

    using (var scope = app.Services.CreateScope())
    {
        RecurringJob.AddOrUpdate<TaskReminderJob>(
            "task-reminder",
            job => job.ExecuteAsync(),
            "0 * * * *");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "NexaCRM API failed to start.");
}
finally
{
    Log.CloseAndFlush();
}