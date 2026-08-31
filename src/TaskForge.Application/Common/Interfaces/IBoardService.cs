using TaskForge.Application.Features.Boards;

namespace TaskForge.Application.Common.Interfaces;

public interface IBoardService
{
    Task<BoardResponse?> CreateAsync(Guid projectId, CreateBoardRequest request);
    Task<List<BoardResponse>> GetAllForProjectAsync(Guid projectId);
    Task<BoardResponse?> GetByIdAsync(Guid projectId, Guid boardId);
    Task<BoardResponse?> UpdateAsync(Guid projectId, Guid boardId, UpdateBoardRequest request);
    Task<bool> DeleteAsync(Guid projectId, Guid boardId);
}