using Microsoft.AspNetCore.Identity;

namespace TaskForge.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = default!;
}