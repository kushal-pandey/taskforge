using TaskForge.Application.Features.Projects;

namespace TaskForge.Application.Common.Interfaces;

public interface IProjectService
{
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);

    Task<List<ProjectResponse>> GetAllAsync();

    Task<ProjectResponse?> GetByIdAsync(Guid id);
}