namespace NexaCRM.Application.DTOs;

public record PipelineStageDto(
    string Stage,
    int DealCount,
    decimal TotalValue,
    string Currency);