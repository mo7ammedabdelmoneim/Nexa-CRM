using NexaCRM.Application;
using NexaCRM.Infrastructure;
using NexaCRM.API.Middleware;
using Hangfire;
using NexaCRM.Infrastructure.Hubs;
using NexaCRM.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Services 
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NexaCRM API", Version = "v1" });
});

// App 
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseHangfireDashboard("/jobs");

app.MapHub<NotificationHub>("/hubs/notifications");

// Register recurring jobs
using (var scope = app.Services.CreateScope())
{
    RecurringJob.AddOrUpdate<TaskReminderJob>(
        "task-reminder",
        job => job.ExecuteAsync(),
        "0 * * * *"); //every hour
}

app.MapControllers();

app.Run();