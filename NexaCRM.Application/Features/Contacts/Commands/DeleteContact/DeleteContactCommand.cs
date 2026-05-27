using MediatR;
using NexaCRM.Application.Common;

namespace NexaCRM.Application.Features.Contacts.Commands.DeleteContact;

public record DeleteContactCommand(Guid ContactId) : IRequest<Result>;