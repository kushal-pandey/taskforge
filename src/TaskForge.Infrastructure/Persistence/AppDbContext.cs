using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Identity;

namespace TaskForge.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{
    private readonly ITenantProvider _tenantProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<BoardColumn> BoardColumns => Set<BoardColumn>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TenantUser>().HasKey(tu => new { tu.TenantId, tu.UserId });
        builder.Entity<RefreshToken>().HasIndex(rt => rt.Token).IsUnique();

        builder.Entity<Project>().HasQueryFilter(p => p.TenantId == _tenantProvider.TenantId);
        builder.Entity<Board>().HasQueryFilter(b => b.TenantId == _tenantProvider.TenantId);
        builder.Entity<BoardColumn>().HasQueryFilter(c => c.TenantId == _tenantProvider.TenantId);
        builder.Entity<TaskItem>().HasQueryFilter(t => t.TenantId == _tenantProvider.TenantId);
        builder.Entity<Comment>().HasQueryFilter(c => c.TenantId == _tenantProvider.TenantId);
    }
}