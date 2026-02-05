using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using WolverineIssue.Infrastructure.Consumers;
using WolverineIssue.Infrastructure.Data;

namespace WolverineIssue.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    /// <summary>
    /// Adds EF Core and Wolverine infrastructure to the application
    /// </summary>
    public static IHostApplicationBuilder AddInfrastructure(
        this IHostApplicationBuilder builder, 
        string connectionStringName = "database")
    {
        // Add PostgreSQL with EF Core
        builder.AddNpgsqlDbContext<ApplicationDbContext>(connectionStringName);

        builder.Services.AddDbContextFactory<ApplicationDbContext>(
            optionsAction: options => { options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); });

        builder.Services.AddSingleton<CoreDbContextFactory>();

        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);

        // Add Wolverine with EF Core durable messaging
        builder.UseWolverine(opts =>
        {
            // Use EF Core for durable inbox/outbox
            opts.Durability.ScheduledJobPollingTime = TimeSpan.FromMilliseconds(100); // Default is 5 seconds
            opts.Durability.ScheduledJobFirstExecution = TimeSpan.FromMilliseconds(100);

            // For PostgreSQL, enable PostgreSQL backed partitioning for the inbox table as an optimization
            opts.Durability.EnableInboxPartitioning = true;

            opts.PersistMessagesWithPostgresql(connectionString: connectionString!)
                .EnableMessageTransport();

            opts.Services.AddDbContextWithWolverineIntegration<ApplicationDbContext>(
                x => { x.UseNpgsql(connectionString); });

            // Persist outbox/inbox in EF Core
            opts.UseEntityFrameworkCoreTransactions();
            opts.Policies.UseDurableLocalQueues();
            
            opts.Discovery.IncludeType<MessageEventConsumer>();
        });

        return builder;
    }

    /// <summary>
    /// Applies database migrations on application startup
    /// </summary>
    public static async Task<WebApplication> ApplyMigrationsAsync(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        return app;
    }
}
