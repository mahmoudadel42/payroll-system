using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Security;
using Payroll.Application.Features.GradeSalaries.Commands.CreateGradeSalary;
using Payroll.Application.Features.GradeSalaries.Commands.UpdateGradeSalary;
using Payroll.Application.Features.GradeSalaries.Model;
using Payroll.Application.Features.GradeSalaries.Queries.GetGradeSalaries;
using Payroll.Domain.Enums;

namespace Payroll.Api.Controllers
{
    public sealed class GradeSalariesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<GradeSalaryDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct) =>
            Ok(await Sender.Send(new GetGradeSalariesQuery(), ct));

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateGradeSalaryCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPut("{grade}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(JobGrade grade, UpdateGradeSalaryCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Grade = grade }, ct));
    }
}
