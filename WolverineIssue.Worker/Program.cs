using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;
using WolverineIssue.Infrastructure.Consumers;
using WolverineIssue.Infrastructure.Data;
using WolverineIssue.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure (EF Core + Wolverine)
builder.AddInfrastructure();

var app = builder.Build();


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
app.MapPost("/publish", async (
        IDbContextOutbox bus,
        CoreDbContextFactory factory,
        long entityTaskExecutionId,
        byte segmentationId) =>
    {
       
        
        return Results.Ok(new { Status = "Message published" });
    })
    .WithName("PublishMessage");

app.MapPost("/publish2", async (IDbContextOutbox bus,
        CoreDbContextFactory factory,
        long entityTaskExecutionId,
        string targetWorkflowId) =>
    {
        await using var context = await factory.CreateWithTrackingAsync();

        bus.Enroll(context);

        var executionStrategy = context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await bus.PublishAsync(new TaskCompleteEvent(
                    new TaskEventAwaiterInformation(
                        new EntityTaskExecutionId(entityTaskExecutionId),
                        targetWorkflowId
                    )
                ));

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
        
        return Results.Ok(new { Status = "Message published" });
    })
    .WithName("PublishMessage2");


app.Run();
