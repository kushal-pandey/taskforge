namespace TaskForge.Application.Features.Columns;

public record ColumnResponse(Guid Id, Guid BoardId, string Name, int Order, DateTime CreatedAt);