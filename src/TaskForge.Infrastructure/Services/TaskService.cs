using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Tasks;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Persistence;

namespace TaskForge.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;
    private readonly ITenantProvider _tenantProvider;

    public TaskService(AppDbContext db, ITenantProvider tenantProvider)
    {
        _db = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<TaskResponse?> CreateAsync(Guid columnId, CreateTaskRequest request)
    {
        var columnExists = await _db.BoardColumns.AnyAsync(c => c.Id == columnId);
        if (!columnExists) return null;

        var position = await _db.Tasks.CountAsync(t => t.ColumnId == columnId);

        var task = new TaskItem
        {
            TenantId = _tenantProvider.TenantId,
            ColumnId = columnId,
            Title = request.Title,
            Description = request.Description,
            AssigneeUserId = request.AssigneeUserId,
            DueDate = request.DueDate,
            Position = position
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task<List<TaskResponse>> GetAllForColumnAsync(Guid columnId)
    {
        return await _db.Tasks
            .AsNoTracking()
            .Where(t => t.ColumnId == columnId)
            .OrderBy(t => t.Position)
            .Select(t => new TaskResponse(t.Id, t.ColumnId, t.Title, t.Description, t.AssigneeUserId, t.DueDate, t.Position, t.CreatedAt))
            .ToListAsync();
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid columnId, Guid taskId)
    {
        return await _db.Tasks
            .AsNoTracking()
            .Where(t => t.Id == taskId && t.ColumnId == columnId)
            .Select(t => new TaskResponse(t.Id, t.ColumnId, t.Title, t.Description, t.AssigneeUserId, t.DueDate, t.Position, t.CreatedAt))
            .FirstOrDefaultAsync();
    }

    public async Task<TaskResponse?> UpdateAsync(Guid columnId, Guid taskId, UpdateTaskRequest request)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ColumnId == columnId);
        if (task is null) return null;

        task.Title = request.Title;
        task.Description = request.Description;
        task.AssigneeUserId = request.AssigneeUserId;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task<bool> DeleteAsync(Guid columnId, Guid taskId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ColumnId == columnId);
        if (task is null) return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    private static TaskResponse ToResponse(TaskItem task) =>
        new(task.Id, task.ColumnId, task.Title, task.Description, task.AssigneeUserId, task.DueDate, task.Position, task.CreatedAt);
}