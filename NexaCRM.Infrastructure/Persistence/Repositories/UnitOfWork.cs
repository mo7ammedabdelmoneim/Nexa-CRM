using MediatR;
using NexaCRM.Application.Common;
using NexaCRM.Domain.Common;
using NexaCRM.Domain.Repositories;

namespace NexaCRM.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly IPublisher _publisher;

    public UnitOfWork(AppDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect all domain events before saving
        var domainEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // Save to DB
        var result = await _context.SaveChangesAsync(cancellationToken);

        // Publish domain events 
        foreach (var domainEvent in domainEvents)
        {
            var notification = CreateNotification(domainEvent);
            await _publisher.Publish(notification, cancellationToken);
        }

        //  Clear domain events
        _context.ChangeTracker
            .Entries<BaseEntity>()
            .ToList()
            .ForEach(e => e.Entity.ClearDomainEvents());

        return result;
    }

    private static INotification CreateNotification(IDomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>)
            .MakeGenericType(domainEvent.GetType());

        return (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
    }
}