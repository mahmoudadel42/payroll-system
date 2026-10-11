using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Features.Auth.Commands.Login;
using Payroll.Application.Features.Auth.Commands.Logout;
using Payroll.Application.Features.Auth.Commands.RefreshToken;
using Payroll.Application.Features.Auth.Model;

namespace Payroll.Api.Controllers
{
    [AllowAnonymous]
    public sealed class AuthController : ApiControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPost("refresh")]
        [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));
    }
}
