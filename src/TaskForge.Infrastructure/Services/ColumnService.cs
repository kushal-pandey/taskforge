using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Columns;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Persistence;

namespace TaskForge.Infrastructure.Services;

public class ColumnService : IColumnService
{
    private readonly AppDbContext _db;
    private readonly ITenantProvider _tenantProvider;

    public ColumnService(AppDbContext db, ITenantProvider tenantProvider)
    {
        _db = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<ColumnResponse?> CreateAsync(Guid boardId, CreateColumnRequest request)
    {
        var boardExists = await _db.Boards.AnyAsync(b => b.Id == boardId);
        if (!boardExists) return null;

        var column = new BoardColumn
        {
            TenantId = _tenantProvider.TenantId,
            BoardId = boardId,
            Name = request.Name,
            Order = request.Order
        };

        _db.BoardColumns.Add(column);
        await _db.SaveChangesAsync();

        return new ColumnResponse(column.Id, column.BoardId, column.Name, column.Order, column.CreatedAt);
    }

    public async Task<List<ColumnResponse>> GetAllForBoardAsync(Guid boardId)
    {
        return await _db.BoardColumns
            .AsNoTracking()
            .Where(c => c.BoardId == boardId)
            .OrderBy(c => c.Order)
            .Select(c => new ColumnResponse(c.Id, c.BoardId, c.Name, c.Order, c.CreatedAt))
            .ToListAsync();
    }

    public async Task<ColumnResponse?> GetByIdAsync(Guid boardId, Guid columnId)
    {
        return await _db.BoardColumns
            .AsNoTracking()
            .Where(c => c.Id == columnId && c.BoardId == boardId)
            .Select(c => new ColumnResponse(c.Id, c.BoardId, c.Name, c.Order, c.CreatedAt))
            .FirstOrDefaultAsync();
    }

    public async Task<ColumnResponse?> UpdateAsync(Guid boardId, Guid columnId, UpdateColumnRequest request)
    {
        var column = await _db.BoardColumns.FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
        if (column is null) return null;

        column.Name = request.Name;
        column.Order = request.Order;
        column.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ColumnResponse(column.Id, column.BoardId, column.Name, column.Order, column.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid boardId, Guid columnId)
    {
        var column = await _db.BoardColumns.FirstOrDefaultAsync(c => c.Id == columnId && c.BoardId == boardId);
        if (column is null) return false;

        _db.BoardColumns.Remove(column);
        await _db.SaveChangesAsync();
        return true;
    }
}