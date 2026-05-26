namespace NexaCRM.Domain.Aggregates.Activities;

public static class ActivityType
{
    public const string Call = "Call";
    public const string Email = "Email";
    public const string Meeting = "Meeting";
    public const string Note = "Note";

    public static IReadOnlyList<string> All => [Call, Email, Meeting, Note];
}