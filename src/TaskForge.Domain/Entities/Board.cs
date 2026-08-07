using TaskForge.Domain.Common;

namespace TaskForge.Domain.Entities;

public class Board : TenantEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;
    public string Name { get; set; } = default!;
    public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
}

public class BoardColumn : TenantEntity
{
    public Guid BoardId { get; set; }
    public Board Board { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int Order { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}