using MediatR;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Auth.Commands.Logout
{
    public sealed class LogoutCommandHandler(IIdentityService identity) : IRequestHandler<LogoutCommand, Result>
    {
        public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
        {
            await identity.LogoutAsync(request.RefreshToken, ct);
            return Result.Success();
        }
    }
}
