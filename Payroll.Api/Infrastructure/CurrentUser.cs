using Payroll.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Payroll.Api.Infrastructure
{
    public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
    {
        private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

        public Guid? UserId =>
           Guid.TryParse(Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
               ? id
               : null;

        public string? UserName =>
            Principal?.FindFirstValue(JwtRegisteredClaimNames.UniqueName) ?? Principal?.Identity?.Name;
    }

}
