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
        if (_tenantProvider.TenantId == Guid.Empty)
            throw new UnauthorizedAccessException("Tenant not found.");

        var exists = await _db.Projects.AnyAsync(x =>
            x.TenantId == _tenantProvider.TenantId &&
            x.Name == request.Name);

        if (exists)
            throw new InvalidOperationException("A project with this name already exists.");

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
}