namespace TaskForge.Application.Features.Tasks;

public record CreateTaskRequest(
    string Title,
    string? Description,
    string? AssigneeUserId,
    DateTime? DueDate
);