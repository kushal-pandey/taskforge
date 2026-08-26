using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Projects;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Persistence;

namespace TaskForge.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;
    private readonly ITenantProvider _tenantProvider;

    public ProjectService(
        AppDbContext db,
        ITenantProvider tenantProvider)
    {
        _db = db;
        _tenantProvider = tenantProvider;
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var project = new Project
        {
            TenantId = _tenantProvider.TenantId,
            Name = request.Name,
            Description = request.Description
        };

        _db.Projects.Add(project);

        await _db.SaveChangesAsync();

        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt
        );
    }

    public async Task<List<ProjectResponse>> GetAllAsync()
    {
        return await _db.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectResponse(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt
            ))
            .ToListAsync();
    }

    public async Task<ProjectResponse?> GetByIdAsync(Guid id)
    {
        return await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectResponse(
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt
            ))
            .FirstOrDefaultAsync();
    }

    public async Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return null;

        project.Name = request.Name;
        project.Description = request.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new ProjectResponse(project.Id, project.Name, project.Description, project.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return false;

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        return true;
    }
}