using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WolverineIssue.Infrastructure.Data;

/// <summary>
/// Design-time factory for ApplicationDbContext.
/// This is used by EF Core tools (migrations, etc.) to create DbContext instances.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use a placeholder connection string for design-time operations
        // This allows migrations to be created without a running database
        optionsBuilder.UseNpgsql("Host=localhost;Database=wolverine_issue;Username=postgres;Password=postgres");
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
