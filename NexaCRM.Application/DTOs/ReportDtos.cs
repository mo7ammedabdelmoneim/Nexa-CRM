namespace NexaCRM.Application.DTOs;

public record ConversionRateDto(
    int TotalLeads,
    int WonLeads,
    double ConversionRate,
    DateTime From,
    DateTime To);

public record PipelineValueReportDto(
    IReadOnlyList<PipelineStageDto> Stages,
    decimal TotalValue,
    string Currency);

public record SalesRepPerformanceDto(
    Guid UserId,
    int LeadsHandled,
    int DealsWon,
    decimal TotalValue,
    double AvgCloseDays);

public record LeadSourceBreakdownDto(
    string Source,
    int Count,
    double Percentage);

public record ActivitySummaryDto(
    string Type,
    int Count);

public record OverdueTasksReportDto(
    Guid AssignedToUserId,
    int OverdueCount,
    IReadOnlyList<TaskSummaryDto> Tasks);