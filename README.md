### Prerequisites
- .NET 10 SDK or later
- Docker Desktop (for PostgreSQL container).

### Issue:
When you publish an message in two different transaction in the same scope, the last message doesn't get consumed, and is now stuck. Until you either reboot or enable:

opts.Durability.OutboxStaleTime = x.Seconds();

opts.Durability.InboxStaleTime = x.Seconds();

You can find the code in WolverineIssue.Program on line 42-50.
