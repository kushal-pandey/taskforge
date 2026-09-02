namespace TaskForge.Application.Features.Tasks;

public record TaskResponse(
    Guid Id,
    Guid ColumnId,
    string Title,
    string? Description,
    string? AssigneeUserId,
    DateTime? DueDate,
    int Position,
    DateTime CreatedAt
);