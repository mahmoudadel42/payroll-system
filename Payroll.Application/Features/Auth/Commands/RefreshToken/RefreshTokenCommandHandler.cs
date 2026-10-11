using MediatR;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Auth.Model;

namespace Payroll.Application.Features.Auth.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler(IIdentityService identity)
        : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        public Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct) =>
            identity.RefreshAsync(request.RefreshToken, ct);
    }
}
