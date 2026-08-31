using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Boards;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/boards")]
[Authorize]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IValidator<CreateBoardRequest> _createValidator;
    private readonly IValidator<UpdateBoardRequest> _updateValidator;

    public BoardsController(
        IBoardService boardService,
        IValidator<CreateBoardRequest> createValidator,
        IValidator<UpdateBoardRequest> updateValidator)
    {
        _boardService = boardService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<List<BoardResponse>>> GetAll(Guid projectId)
        => Ok(await _boardService.GetAllForProjectAsync(projectId));

    [HttpGet("{boardId:guid}")]
    public async Task<ActionResult<BoardResponse>> GetById(Guid projectId, Guid boardId)
    {
        var board = await _boardService.GetByIdAsync(projectId, boardId);
        return board is null ? NotFound(new { error = "Board not found." }) : Ok(board);
    }

    [HttpPost]
    public async Task<ActionResult<BoardResponse>> Create(Guid projectId, CreateBoardRequest request)
    {
        var validation = await _createValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var board = await _boardService.CreateAsync(projectId, request);
        if (board is null)
            return NotFound(new { error = "Project not found." });

        return CreatedAtAction(nameof(GetById), new { projectId, boardId = board.Id }, board);
    }

    [HttpPut("{boardId:guid}")]
    public async Task<ActionResult<BoardResponse>> Update(Guid projectId, Guid boardId, UpdateBoardRequest request)
    {
        var validation = await _updateValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var updated = await _boardService.UpdateAsync(projectId, boardId, request);
        return updated is null ? NotFound(new { error = "Board not found." }) : Ok(updated);
    }

    [HttpDelete("{boardId:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid boardId)
    {
        var deleted = await _boardService.DeleteAsync(projectId, boardId);
        return deleted ? NoContent() : NotFound(new { error = "Board not found." });
    }
}