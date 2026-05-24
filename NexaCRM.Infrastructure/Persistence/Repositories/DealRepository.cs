using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Deals;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class DealRepository : IDealRepository
{
    private readonly AppDbContext _context;

    public DealRepository(AppDbContext context)
        => _context = context;

    public async Task<Deal?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Deals
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task AddAsync(
        Deal deal,
        CancellationToken cancellationToken = default)
        => await _context.Deals.AddAsync(deal, cancellationToken);

    public void Update(Deal deal)
        => _context.Deals.Update(deal);
}