namespace NexaCRM.Domain.Exceptions;
public class InvalidStatusTransitionException : DomainException
{
    public InvalidStatusTransitionException(string from, string to)
        : base($"Invalid status transition from '{from}' to '{to}'.") { }
}