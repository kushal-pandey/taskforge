namespace TaskForge.Application.Features.Tasks;

public record UpdateTaskRequest(
    string Title,
    string? Description,
    string? AssigneeUserId,
    DateTime? DueDate
);