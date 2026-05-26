using NexaCRM.Domain.Aggregates.Activities;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _context;

    public ActivityRepository(AppDbContext context)
        => _context = context;

    public async Task<Activity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Activities
            .FindAsync([id], cancellationToken);

    public async Task AddAsync(
        Activity activity,
        CancellationToken cancellationToken = default)
        => await _context.Activities.AddAsync(activity, cancellationToken);
}