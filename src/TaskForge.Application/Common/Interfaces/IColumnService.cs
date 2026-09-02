using TaskForge.Application.Features.Columns;

namespace TaskForge.Application.Common.Interfaces;

public interface IColumnService
{
    Task<ColumnResponse?> CreateAsync(Guid boardId, CreateColumnRequest request);
    Task<List<ColumnResponse>> GetAllForBoardAsync(Guid boardId);
    Task<ColumnResponse?> GetByIdAsync(Guid boardId, Guid columnId);
    Task<ColumnResponse?> UpdateAsync(Guid boardId, Guid columnId, UpdateColumnRequest request);
    Task<bool> DeleteAsync(Guid boardId, Guid columnId);
}