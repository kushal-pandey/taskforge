using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Columns;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/boards/{boardId:guid}/columns")]
[Authorize]
public class ColumnsController : ControllerBase
{
    private readonly IColumnService _columnService;
    private readonly IValidator<CreateColumnRequest> _createValidator;
    private readonly IValidator<UpdateColumnRequest> _updateValidator;

    public ColumnsController(
        IColumnService columnService,
        IValidator<CreateColumnRequest> createValidator,
        IValidator<UpdateColumnRequest> updateValidator)
    {
        _columnService = columnService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ColumnResponse>>> GetAll(Guid boardId)
        => Ok(await _columnService.GetAllForBoardAsync(boardId));

    [HttpGet("{columnId:guid}")]
    public async Task<ActionResult<ColumnResponse>> GetById(Guid boardId, Guid columnId)
    {
        var column = await _columnService.GetByIdAsync(boardId, columnId);
        return column is null ? NotFound(new { error = "Column not found." }) : Ok(column);
    }

    [HttpPost]
    public async Task<ActionResult<ColumnResponse>> Create(Guid boardId, CreateColumnRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var column = await _columnService.CreateAsync(boardId, request);
        if (column is null)
            return NotFound(new { error = "Board not found." });

        return CreatedAtAction(nameof(GetById), new { boardId, columnId = column.Id }, column);
    }

    [HttpPut("{columnId:guid}")]
    public async Task<ActionResult<ColumnResponse>> Update(Guid boardId, Guid columnId, UpdateColumnRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _columnService.UpdateAsync(boardId, columnId, request);
        return updated is null ? NotFound(new { error = "Column not found." }) : Ok(updated);
    }

    [HttpDelete("{columnId:guid}")]
    public async Task<IActionResult> Delete(Guid boardId, Guid columnId)
    {
        var deleted = await _columnService.DeleteAsync(boardId, columnId);
        return deleted ? NoContent() : NotFound(new { error = "Column not found." });
    }
}