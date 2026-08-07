using TaskForge.Domain.Common;

namespace TaskForge.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}

public enum TenantRole { Owner, Admin, Member }

public class TenantUser
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public TenantRole Role { get; set; }
}