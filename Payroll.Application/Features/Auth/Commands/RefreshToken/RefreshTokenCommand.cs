using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Auth.Model;

namespace Payroll.Application.Features.Auth.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;
}
