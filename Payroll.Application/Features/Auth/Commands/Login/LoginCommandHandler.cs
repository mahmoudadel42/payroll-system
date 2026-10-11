using MediatR;
using Payroll.Application.Common.Interfaces;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Auth.Model;

namespace Payroll.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler(IIdentityService identity)
        : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        public Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct) =>
            identity.LoginAsync(request.UserName, request.Password, ct);
    }
}
