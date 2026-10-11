using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Auth.Commands.Logout
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
}
