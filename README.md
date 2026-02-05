# Wolverine with .NET Aspire Project

This project demonstrates a .NET Aspire application with:
- ASP.NET Core Web API
- PostgreSQL database with EF Core
- Wolverine message bus with EF Core durable inbox/outbox pattern
- Message publishing endpoint and consumer

## Project Structure

- **WolverineIssue.AppHost** - Aspire orchestrator project that manages the infrastructure
- **WolverineIssue** - ASP.NET Core Web API with Wolverine integration
- **WolverineIssue.ServiceDefaults** - Shared service configuration for Aspire

## Features

### Aspire Infrastructure
- PostgreSQL database container orchestration
- PgAdmin for database management
- Service discovery and configuration
- Health checks

### Wolverine Integration
- **Durable Messaging**: Uses EF Core to store messages in PostgreSQL
- **Inbox/Outbox Pattern**: Ensures reliable message delivery
- **Local Queue**: Messages are processed using a local in-memory queue
- **Message Handler**: Automatically discovered consumer that logs received messages

## Running the Application

### Prerequisites
- .NET 10 SDK or later
- Docker Desktop (for PostgreSQL container)

### Start the Application

1. Run the Aspire AppHost:
```powershell
cd D:\Projects\WolverineIssue\WolverineIssue.AppHost
dotnet run
```

2. The Aspire dashboard will open automatically showing:
   - API service status
   - PostgreSQL database status
   - PgAdmin web interface
   - Logs and metrics

### Testing the Wolverine Endpoint

Once the application is running, you can test the message publishing:

**Publish a message:**
```powershell
# Using curl
curl -X POST "https://localhost:<port>/publish?message=Hello%20Wolverine" -k

# Using PowerShell
Invoke-RestMethod -Method POST -Uri "https://localhost:<port>/publish?message=Hello%20Wolverine" -SkipCertificateCheck
```

Replace `<port>` with the actual port shown in the Aspire dashboard.

## How It Works

1. **Message Publishing**: When you POST to `/publish`, the API publishes a `MessageEvent` to Wolverine
2. **Durable Storage**: Wolverine stores the message in the PostgreSQL outbox table via EF Core
3. **Message Processing**: Wolverine processes messages from the local queue
4. **Consumer Execution**: The `MessageEventConsumer` receives the message and logs it
5. **Durable Inbox**: Wolverine tracks processed messages in the inbox table

## Code Structure

### Event Definition
```csharp
public record MessageEvent(string Message);
```

### Consumer
```csharp
public class MessageEventConsumer
{
    public void Handle(MessageEvent message)
    {
        _logger.LogInformation("Received message from Wolverine: {Message}", message.Message);
    }
}
```

### Endpoint
```csharp
app.MapPost("/publish", async (IMessageBus bus, string message) =>
{
    await bus.PublishAsync(new MessageEvent(message));
    return Results.Ok(new { Status = "Message published", Message = message });
});
```

## Database

The PostgreSQL database includes:
- Standard application tables (if any)
- `wolverine_outgoing_envelopes` - Outbox for pending messages
- `wolverine_incoming_envelopes` - Inbox for processed messages
- `wolverine_dead_letters` - Failed messages

You can inspect these tables using PgAdmin (accessible via the Aspire dashboard).

## Observability

The Aspire dashboard provides:
- Structured logs from all services
- Distributed tracing
- Metrics and health checks
- Resource management

Check the logs to see messages being published and consumed!
