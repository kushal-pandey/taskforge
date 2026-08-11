namespace TaskForge.Application.Features.Projects;

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt
);