using WolverineIssue.Events;

namespace WolverineIssue.Consumers;

public class MessageEventConsumer
{
    private readonly ILogger<MessageEventConsumer> _logger;

    public MessageEventConsumer(ILogger<MessageEventConsumer> logger)
    {
        _logger = logger;
    }

    public void Handle(InitiateTaskEvent message)
    {
        _logger.LogError("Received message from Wolverine: {Message}", message.EntityTaskExecutionId.Value);
        _logger.LogError("Received message from Wolverine: {Message}", message.SegmentationId?.Value);
    }
}
