namespace TaskForge.Application.Features.Projects;

public record CreateProjectRequest(
    string Name,
    string? Description
);