namespace NexaCRM.Domain.Aggregates.Notifications;

public static class NotificationType
{
    public const string LeadStatusChanged = "LeadStatusChanged";
    public const string LeadAssigned = "LeadAssigned";
    public const string TaskAssigned = "TaskAssigned";
    public const string TaskOverdue = "TaskOverdue";
    public const string DealStageChanged = "DealStageChanged";
    public const string DealWon = "DealWon";
}