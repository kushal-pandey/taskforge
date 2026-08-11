using TaskForge.Application.Features.Projects;

namespace TaskForge.Application.Common.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
}