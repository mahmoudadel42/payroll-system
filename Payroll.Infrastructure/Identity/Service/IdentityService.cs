using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Auth.Model;
using Payroll.Infrastructure.Identity.Model;
using Payroll.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Payroll.Infrastructure.Identity.Service
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly ApplicationDbContext _context;
        private readonly JwtOptions _jwt;
        private static Error InvalidCredentials =>
        Error.Unauthorized("auth.invalidCredentials", "Invalid user name or password.");
        private static Error InvalidRefresh =>
        Error.Unauthorized("auth.invalidRefreshToken", "Your session has expired. Please sign in again.");
        public IdentityService(UserManager<ApplicationUser> users, ApplicationDbContext context, IOptions<JwtOptions> jwtOptions)
        {
            _users = users;
            _context = context;
            _jwt = jwtOptions.Value;
        }

        public async Task<Result<AuthResponse>> LoginAsync(string userName, string password, CancellationToken ct)
        {
            var user = await _users.FindByNameAsync(userName);
            if (user is null)
                return InvalidCredentials;

            if (await _users.IsLockedOutAsync(user))
                return Error.Unauthorized("auth.lockedOut", "Your account is temporarily locked. Please try again later.");

            if (!await _users.CheckPasswordAsync(user, password))
            {
                await _users.AccessFailedAsync(user);
                return InvalidCredentials;
            }

            await _users.ResetAccessFailedCountAsync(user);
            return await IssueAsync(user, ct);
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken ct)
        {
            var hash = Hash(refreshToken);
            var stored = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
            if (stored is null || stored.RevokedAt is not null)
                return;
            stored.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var hash = Hash(refreshToken);
            var stored = await _context.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
            if (stored is null)
                return InvalidRefresh;

            if (stored.RevokedAt is not null)
            {
                // A revoked token being replayed indicates theft: revoke every active token of this user.
                await _context.RefreshTokens
                    .Where(t => t.UserId == stored.UserId && t.RevokedAt == null)
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now), ct);
                return InvalidRefresh;
            }

            if (!stored.IsActive(now)) return InvalidRefresh;

            var response = await IssueAsync(stored.User, ct, stored);
            return response;
        }


        private async Task<AuthResponse> IssueAsync(ApplicationUser user, CancellationToken ct, RefreshToken? replacing = null)
        {
            var now = DateTime.UtcNow;
            var roles = (await _users.GetRolesAsync(user)).ToList();

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new("display_name", user.DisplayName)
        };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var accessExpires = now.AddMinutes(_jwt.AccessTokenMinutes);
            var token = new JwtSecurityToken(
                _jwt.Issuer, _jwt.Audience, claims, now, accessExpires,
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)), SecurityAlgorithms.HmacSha256));

            var rawRefresh = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
            var refreshExpires = now.AddDays(_jwt.RefreshTokenDays);
            var refreshHash = Hash(rawRefresh);

            if (replacing is not null)
            {
                replacing.RevokedAt = now;
                replacing.ReplacedByHash = refreshHash;
            }

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshHash,
                CreatedAt = now,
                ExpiresAt = refreshExpires
            });
            await _context.SaveChangesAsync(ct);
            return new AuthResponse
            {
                DisplayName = user.DisplayName,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = rawRefresh,
                AccessTokenExpiresAt = accessExpires,
                Roles = roles,
                UserName = user.UserName!,
                RefreshTokenExpiresAt = refreshExpires
            };
        }
        private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
