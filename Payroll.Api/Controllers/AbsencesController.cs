using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Absences.Commands.CreateAbsence;
using Payroll.Application.Features.Absences.Commands.CreateAbsences;
using Payroll.Application.Features.Absences.Commands.DeleteAbsence;
using Payroll.Application.Features.Absences.Commands.UpdateAbsence;
using Payroll.Application.Features.Absences.Model;
using Payroll.Application.Features.Absences.Queries.GetAbsences;

namespace Payroll.Api.Controllers
{
    public sealed class AbsencesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<PagedResult<AbsenceDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAbsencesQuery query, CancellationToken ct) =>
            Ok(await Sender.Send(query, ct));

        [HttpPost]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CreateAbsenceCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPost("bulk")]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMany(CreateAbsencesCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command, ct));

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, UpdateAbsenceCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Id = id }, ct));

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new DeleteAbsenceCommand(id), ct));
    }
}
