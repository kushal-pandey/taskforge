using TaskForge.Application.Common.Interfaces;

namespace TaskForge.Infrastructure.Multitenancy;

public class TenantProvider : ITenantProvider
{
    public Guid TenantId { get; private set; } = Guid.Empty;
    public void SetTenant(Guid tenantId) => TenantId = tenantId;
}