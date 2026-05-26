namespace NexaCRM.Domain.Aggregates.Tasks;

public static class TaskPriority
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
    public const string Urgent = "Urgent";

    public static IReadOnlyList<string> All => [Low, Medium, High, Urgent];
}