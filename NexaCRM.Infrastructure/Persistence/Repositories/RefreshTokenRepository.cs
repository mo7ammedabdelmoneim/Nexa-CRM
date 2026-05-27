using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Users;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
        => _context = context;

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
        => await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

    public async Task AddAsync(
        RefreshToken token,
        CancellationToken cancellationToken = default)
        => await _context.RefreshTokens.AddAsync(token, cancellationToken);

    public void Update(RefreshToken token)
        => _context.RefreshTokens.Update(token);
}