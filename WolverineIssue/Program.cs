using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using WolverineIssue.Data;
using WolverineIssue.Events;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add PostgreSQL with EF Core
builder.AddNpgsqlDbContext<ApplicationDbContext>("database");

builder.Services.AddDbContextFactory<ApplicationDbContext>(
    optionsAction: options => { options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); });

builder.Services.AddSingleton<CoreDbContextFactory>();

var connectionString = builder.Configuration.GetConnectionString("database");

// Add Wolverine with EF Core durable messaging
builder.UseWolverine(opts =>
{
    // Use EF Core for durable inbox/outbox
    opts.Durability.ScheduledJobPollingTime = TimeSpan.FromMilliseconds(100); // Default is 5 seconds
    opts.Durability.ScheduledJobFirstExecution = TimeSpan.FromMilliseconds(100);

    //For PostgreSQL, you can enable PostgreSQL backed partitioning for the inbox table as an optimization.
    //This is not enabled by default just to avoid causing database migrations in a minor point release.
    //Note that this will have some significant benefits for inbox/outbox metrics gathering in the future:
    opts.Durability.EnableInboxPartitioning = true;

    opts.PersistMessagesWithPostgresql(connectionString: connectionString!)
        .EnableMessageTransport();

    opts.Services.AddDbContextWithWolverineIntegration<ApplicationDbContext>(x => { x.UseNpgsql(connectionString); });

    // Persist outbox/inbox in EF Core
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.UseDurableLocalQueues();

    // Discover message handlers
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WolverineIssue API v1");
    options.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
});

app.UseHttpsRedirection();

// Redirect root to Swagger UI for Aspire
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

// Add endpoint to publish a message
app.MapPost("/publish", async (IDbContextOutbox bus,
        CoreDbContextFactory factory,
        string message) =>
    {
        await using var context = await factory.CreateWithTrackingAsync();

        bus.Enroll(context);

        var executionStrategy = context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await bus.PublishAsync(new MessageEvent(message));

                await context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                await bus.FlushOutgoingMessagesAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
        
        return Results.Ok(new { Status = "Message published", Message = message });
    })
    .WithName("PublishMessage");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}