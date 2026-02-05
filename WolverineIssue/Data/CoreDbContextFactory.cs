using Microsoft.EntityFrameworkCore;

namespace WolverineIssue.Data;

public class CoreDbContextFactory
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CoreDbContextFactory(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<ApplicationDbContext> CreateAsync()
    {
        var context = await _contextFactory.CreateDbContextAsync();

        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return context;
    }

    public async Task<ApplicationDbContext> CreateWithTrackingAsync()
    {
        var context = await _contextFactory.CreateDbContextAsync();

        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;

        return context;
    }
}