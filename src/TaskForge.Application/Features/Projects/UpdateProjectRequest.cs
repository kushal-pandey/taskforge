namespace TaskForge.Application.Features.Projects;

public record UpdateProjectRequest(
    string Name,
    string? Description
);