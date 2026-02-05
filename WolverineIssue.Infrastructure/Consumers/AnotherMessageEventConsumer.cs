using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wolverine.EntityFrameworkCore;
using WolverineIssue.Infrastructure.Data;

namespace WolverineIssue.Infrastructure.Consumers;

public class AnotherMessageEventConsumer
{

    private readonly ILogger<AnotherMessageEventConsumer> _logger;

    public AnotherMessageEventConsumer(ILogger<AnotherMessageEventConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Handle(
        TaskCompleteEvent message,
        IDbContextOutbox bus,
        CoreDbContextFactory factory)
    {
        _logger.LogError("Received message from Wolverine: {Message}", message.TaskEventAwaiterInformation.ExecutionId.Value);
        
        await using var context = await factory.CreateWithTrackingAsync();

        bus.Enroll(context);

        var executionStrategy = context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await bus.PublishAsync(new InitiateTaskEvent
                {
                    EntityTaskExecutionId = new EntityTaskExecutionId(123),
                    SegmentationId = new SegmentationId(123)
                });

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
        
    }
}