using MediatR;
using NexaCRM.Domain.Common;

namespace NexaCRM.Application.Common;

public sealed record DomainEventNotification<TEvent>(TEvent Event)
    : INotification where TEvent : IDomainEvent;