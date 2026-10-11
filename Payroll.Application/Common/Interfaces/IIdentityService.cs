using Payroll.Application.Common.Models;
using Payroll.Application.Features.Auth.Model;

namespace Payroll.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<AuthResponse>> LoginAsync(string userName, string password, CancellationToken ct);
        Task<Result<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct);
        Task LogoutAsync(string refreshToken, CancellationToken ct);
    }
}
