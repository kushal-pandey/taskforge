using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Common.Interfaces;
using TaskForge.Application.Features.Auth;
using TaskForge.Domain.Entities;
using TaskForge.Infrastructure.Persistence;

namespace TaskForge.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(AppDbContext db, UserManager<AppUser> userManager, JwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var tenant = new Tenant
        {
            Name = request.TenantName,
            Slug = request.TenantName.ToLower().Replace(" ", "-")
        };
        _db.Tenants.Add(tenant);

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        _db.TenantUsers.Add(new TenantUser
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            Role = TenantRole.Owner
        });

        await _db.SaveChangesAsync();

        return await IssueTokensAsync(user, tenant.Id, TenantRole.Owner.ToString());
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var tenantUser = await _db.TenantUsers.FirstOrDefaultAsync(tu => tu.UserId == user.Id)
            ?? throw new UnauthorizedAccessException("User is not linked to any tenant.");

        return await IssueTokensAsync(user, tenantUser.TenantId, tenantUser.Role.ToString());
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (stored.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired.");

        stored.IsRevoked = true; // rotation: old token is single-use

        var user = await _userManager.FindByIdAsync(stored.UserId)
            ?? throw new UnauthorizedAccessException("User no longer exists.");

        var tenantUser = await _db.TenantUsers.FirstOrDefaultAsync(tu => tu.UserId == user.Id)
            ?? throw new UnauthorizedAccessException("User is not linked to any tenant.");

        var response = await IssueTokensAsync(user, tenantUser.TenantId, tenantUser.Role.ToString());
        await _db.SaveChangesAsync();
        return response;
    }

    private async Task<AuthResponse> IssueTokensAsync(AppUser user, Guid tenantId, string role)
    {
        var (accessToken, expiresAt) = _tokenGenerator.GenerateAccessToken(user, tenantId, role);
        var refreshTokenValue = _tokenGenerator.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(_tokenGenerator.RefreshTokenExpiryDays)
        });

        await _db.SaveChangesAsync();

        return new AuthResponse(accessToken, refreshTokenValue, expiresAt);
    }
}