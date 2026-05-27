namespace NexaCRM.Domain.Aggregates.Users;

public static class UserRole
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string SalesRep = "SalesRep";
    public const string Viewer = "Viewer";

    public static IReadOnlyList<string> All =>
        [Admin, Manager, SalesRep, Viewer];
}
