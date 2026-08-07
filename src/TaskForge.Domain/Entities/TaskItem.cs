using TaskForge.Domain.Common;

namespace TaskForge.Domain.Entities;

public class TaskItem : TenantEntity
{
    public Guid ColumnId { get; set; }
    public BoardColumn Column { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? AssigneeUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public int Position { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public class Comment : TenantEntity
{
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = default!;
    public string AuthorUserId { get; set; } = default!;
    public string Body { get; set; } = default!;
}