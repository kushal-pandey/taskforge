using TaskForge.Application.Features.Tasks;

namespace TaskForge.Application.Common.Interfaces;

public interface ITaskService
{
    Task<TaskResponse?> CreateAsync(Guid columnId, CreateTaskRequest request);
    Task<List<TaskResponse>> GetAllForColumnAsync(Guid columnId);
    Task<TaskResponse?> GetByIdAsync(Guid columnId, Guid taskId);
    Task<TaskResponse?> UpdateAsync(Guid columnId, Guid taskId, UpdateTaskRequest request);
    Task<bool> DeleteAsync(Guid columnId, Guid taskId);
}