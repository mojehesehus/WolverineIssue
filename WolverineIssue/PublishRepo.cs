using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;
using WolverineIssue.Infrastructure.Consumers;
using WolverineIssue.Infrastructure.Data;

namespace WolverineIssue;

public class PublishRepo(
    IDbContextOutbox bus,
    CoreDbContextFactory factory)
{
    public async Task Publish()
    {
        await using var context = await factory.CreateWithTrackingAsync();

        bus.Enroll(context);

        var executionStrategy = context.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                await bus.PublishAsync(new InitiateTaskEvent("test"));

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