using Microsoft.EntityFrameworkCore;
using NexaCRM.Domain.Aggregates.Users;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
        => _context = context;

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(
        string email,
        Guid tenantId,
        CancellationToken cancellationToken = default)
        => await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Email == email.ToLower() &&
                u.TenantId == tenantId,
                cancellationToken);

    public async Task<bool> ExistsByEmailAsync(
        string email,
        Guid tenantId,
        CancellationToken cancellationToken = default)
        => await _context.Users
            .AnyAsync(u =>
                u.Email == email.ToLower() &&
                u.TenantId == tenantId,
                cancellationToken);

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(user, cancellationToken);

    public void Update(User user)
        => _context.Users.Update(user);
}