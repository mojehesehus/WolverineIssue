using WolverineIssue;
using WolverineIssue.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<PublishRepo>();

// Add Infrastructure (EF Core + Wolverine)
builder.AddInfrastructure();

var app = builder.Build();

// Apply migrations on startup
await app.ApplyMigrationsAsync();

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
app.MapPost("/publish", async (PublishRepo repo) =>
    {
        //First transaction
        await repo.Publish();
        //Second transaction
        await repo.Publish();
        
        return Results.Ok(new { Status = "Message published" });
    })
    .WithName("PublishMessage");

app.Run();
