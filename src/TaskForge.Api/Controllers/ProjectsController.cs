using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Projects;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project == null)
            return NotFound(new { error = "Project not found." });

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request)
    {
        var project = await _projectService.CreateAsync(request);

        return Ok(project);
    }
}