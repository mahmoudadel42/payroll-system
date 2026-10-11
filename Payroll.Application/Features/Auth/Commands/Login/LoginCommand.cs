using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Auth.Model;

namespace Payroll.Application.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(string UserName, string Password) : IRequest<Result<AuthResponse>>;
}
