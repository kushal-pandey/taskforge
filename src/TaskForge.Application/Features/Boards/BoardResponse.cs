namespace TaskForge.Application.Features.Boards;

public record BoardResponse(Guid Id, Guid ProjectId, string Name, DateTime CreatedAt);