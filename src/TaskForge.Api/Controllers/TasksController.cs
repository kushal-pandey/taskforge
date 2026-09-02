using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Tasks;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/columns/{columnId:guid}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<UpdateTaskRequest> _updateValidator;

    public TasksController(
        ITaskService taskService,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<UpdateTaskRequest> updateValidator)
    {
        _taskService = taskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> GetAll(Guid columnId)
        => Ok(await _taskService.GetAllForColumnAsync(columnId));

    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid columnId, Guid taskId)
    {
        var task = await _taskService.GetByIdAsync(columnId, taskId);
        return task is null ? NotFound(new { error = "Task not found." }) : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(Guid columnId, CreateTaskRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var task = await _taskService.CreateAsync(columnId, request);
        if (task is null)
            return NotFound(new { error = "Column not found." });

        return CreatedAtAction(nameof(GetById), new { columnId, taskId = task.Id }, task);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid columnId, Guid taskId, UpdateTaskRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _taskService.UpdateAsync(columnId, taskId, request);
        return updated is null ? NotFound(new { error = "Task not found." }) : Ok(updated);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> Delete(Guid columnId, Guid taskId)
    {
        var deleted = await _taskService.DeleteAsync(columnId, taskId);
        return deleted ? NoContent() : NotFound(new { error = "Task not found." });
    }
}