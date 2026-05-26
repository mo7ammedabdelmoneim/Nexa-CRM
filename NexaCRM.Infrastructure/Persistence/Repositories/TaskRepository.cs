using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Tasks;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
        => _context = context;

    public async Task<CrmTask?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(
        CrmTask task,
        CancellationToken cancellationToken = default)
        => await _context.Tasks.AddAsync(task, cancellationToken);

    public void Update(CrmTask task)
        => _context.Tasks.Update(task);

    public void Delete(CrmTask task)
        => _context.Tasks.Update(task);
}