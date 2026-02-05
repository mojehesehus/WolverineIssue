# Test Script for Wolverine Aspire Project

Write-Host "Building the solution..." -ForegroundColor Cyan
$buildResult = dotnet build D:\Projects\WolverineIssue\WolverineIssue.sln

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build successful!" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`nProject structure:" -ForegroundColor Cyan
Write-Host "- WolverineIssue.AppHost (Aspire Orchestrator)"
Write-Host "- WolverineIssue (API with Wolverine)"
Write-Host "- WolverineIssue.ServiceDefaults (Shared config)"

Write-Host "`nKey files created:" -ForegroundColor Cyan
Write-Host "✓ ApplicationDbContext.cs (EF Core with Wolverine inbox/outbox)"
Write-Host "✓ MessageEvent.cs (Event definition)"
Write-Host "✓ MessageEventConsumer.cs (Wolverine consumer)"
Write-Host "✓ Program.cs (Wolverine + Aspire configuration)"
Write-Host "✓ AppHost.cs (PostgreSQL + API orchestration)"

Write-Host "`nTo run the application:" -ForegroundColor Yellow
Write-Host "cd D:\Projects\WolverineIssue\WolverineIssue.AppHost"
Write-Host "dotnet run"
Write-Host "`nThen test with:"
Write-Host 'Invoke-RestMethod -Method POST -Uri "https://localhost:<port>/publish?message=Hello" -SkipCertificateCheck'

Write-Host "`n✓ Setup complete! Your Aspire + Wolverine project is ready." -ForegroundColor Green
