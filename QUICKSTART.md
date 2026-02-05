# Quick Start Guide

## Run the Application

```powershell
# Navigate to AppHost
cd D:\Projects\WolverineIssue\WolverineIssue.AppHost

# Run the application
dotnet run
```

## Test the Endpoint

After the Aspire dashboard opens and shows the API is running:

1. Note the API's HTTPS port from the dashboard (e.g., 7123)

2. Open a new PowerShell window and run:

```powershell
# Replace 7123 with your actual port
Invoke-RestMethod -Method POST -Uri "https://localhost:7123/publish?message=Hello%20from%20Wolverine" -SkipCertificateCheck
```

3. Check the Aspire dashboard logs to see:
   - API: Message published
   - Consumer: "Received message from Wolverine: Hello from Wolverine"

## What You'll See

**In Aspire Dashboard:**
- ✓ PostgreSQL running (green)
- ✓ PgAdmin running (green)  
- ✓ API running (green)
- ✓ Logs showing message flow
- ✓ Distributed traces

**Expected Response:**
```json
{
  "status": "Message published",
  "message": "Hello from Wolverine"
}
```

**In Logs:**
```
info: WolverineIssue.Consumers.MessageEventConsumer[0]
      Received message from Wolverine: Hello from Wolverine
```

That's it! Your Wolverine + Aspire application is working! 🎉
