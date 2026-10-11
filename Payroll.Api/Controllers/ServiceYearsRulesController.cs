using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Security;
using Payroll.Application.Features.ServiceYearsRules.Commands.CreateServiceYearsRule;
using Payroll.Application.Features.ServiceYearsRules.Commands.DeleteServiceYearsRule;
using Payroll.Application.Features.ServiceYearsRules.Commands.UpdateServiceYearsRule;
using Payroll.Application.Features.ServiceYearsRules.Model;
using Payroll.Application.Features.ServiceYearsRules.Queries.GetServiceYearsRules;

namespace Payroll.Api.Controllers
{
    public sealed class ServiceYearsRulesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<ServiceYearsRuleDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct) =>
            Ok(await Sender.Send(new GetServiceYearsRulesQuery(), ct));

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateServiceYearsRuleCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, UpdateServiceYearsRuleCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Id = id }, ct));

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new DeleteServiceYearsRuleCommand(id), ct));
    }
}
