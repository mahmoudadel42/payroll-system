using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Security;
using Payroll.Application.Features.AbsenceRules.Commands.CreateAbsenceRule;
using Payroll.Application.Features.AbsenceRules.Commands.DeleteAbsenceRule;
using Payroll.Application.Features.AbsenceRules.Commands.UpdateAbsenceRule;
using Payroll.Application.Features.AbsenceRules.Model;
using Payroll.Application.Features.AbsenceRules.Queries.GetAbsenceRules;

namespace Payroll.Api.Controllers
{
    public sealed class AbsenceRulesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<AbsenceRuleDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct) =>
            Ok(await Sender.Send(new GetAbsenceRulesQuery(), ct));

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateAbsenceRuleCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, UpdateAbsenceRuleCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Id = id }, ct));

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new DeleteAbsenceRuleCommand(id), ct));
    }
}
