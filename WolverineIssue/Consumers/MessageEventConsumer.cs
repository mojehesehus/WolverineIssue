using WolverineIssue.Events;

namespace WolverineIssue.Consumers;

public class MessageEventConsumer
{
    private readonly ILogger<MessageEventConsumer> _logger;

    public MessageEventConsumer(ILogger<MessageEventConsumer> logger)
    {
        _logger = logger;
    }

    public void Handle(MessageEvent message)
    {
        _logger.LogInformation("Received message from Wolverine: {Message}", message.Message);
    }
}
