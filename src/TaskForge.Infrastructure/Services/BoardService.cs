using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Boards;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Persistence;

namespace TaskForge.Infrastructure.Services;

public class BoardService : IBoardService
{
    private readonly AppDbContext _db;
    private readonly ITenantProvider _tenantProvider;

    public BoardService(AppDbContext db, ITenantProvider tenantProvider)
    {
        _db = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<BoardResponse?> CreateAsync(Guid projectId, CreateBoardRequest request)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return null;

        var board = new Board
        {
            TenantId = _tenantProvider.TenantId,
            ProjectId = projectId,
            Name = request.Name
        };

        _db.Boards.Add(board);
        await _db.SaveChangesAsync();

        return new BoardResponse(board.Id, board.ProjectId, board.Name, board.CreatedAt);
    }

    public async Task<List<BoardResponse>> GetAllForProjectAsync(Guid projectId)
    {
        return await _db.Boards
            .AsNoTracking()
            .Where(b => b.ProjectId == projectId)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new BoardResponse(b.Id, b.ProjectId, b.Name, b.CreatedAt))
            .ToListAsync();
    }

    public async Task<BoardResponse?> GetByIdAsync(Guid projectId, Guid boardId)
    {
        return await _db.Boards
            .AsNoTracking()
            .Where(b => b.Id == boardId && b.ProjectId == projectId)
            .Select(b => new BoardResponse(b.Id, b.ProjectId, b.Name, b.CreatedAt))
            .FirstOrDefaultAsync();
    }

    public async Task<BoardResponse?> UpdateAsync(Guid projectId, Guid boardId, UpdateBoardRequest request)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == boardId && b.ProjectId == projectId);
        if (board is null) return null;

        board.Name = request.Name;
        board.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new BoardResponse(board.Id, board.ProjectId, board.Name, board.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid boardId)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == boardId && b.ProjectId == projectId);
        if (board is null) return false;

        _db.Boards.Remove(board);
        await _db.SaveChangesAsync();
        return true;
    }
}