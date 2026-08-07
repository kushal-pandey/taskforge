using TaskForge.Domain.Common;

namespace TaskForge.Domain.Entities;

public class Project : TenantEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public ICollection<Board> Boards { get; set; } = new List<Board>();
}