using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Models;
using Payroll.Application.Common.Security;
using Payroll.Application.Features.PayrollRuns.Commands.ApprovePayrollRun;
using Payroll.Application.Features.PayrollRuns.Commands.DeletePayrollRun;
using Payroll.Application.Features.PayrollRuns.Commands.GeneratePayrollRun;
using Payroll.Application.Features.PayrollRuns.Commands.RecalculatePayrollRun;
using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Application.Features.PayrollRuns.Queries.GetPayrollRunById;
using Payroll.Application.Features.PayrollRuns.Queries.GetPayrollRuns;
using Payroll.Application.Features.PayrollRuns.Queries.GetPayslip;

namespace Payroll.Api.Controllers
{
    public sealed class PayrollRunsController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<PagedResult<PayrollRunDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetPayrollRunsQuery query, CancellationToken ct) =>
            Ok(await Sender.Send(query, ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType<PayrollRunDetailsDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new GetPayrollRunByIdQuery(id), ct));

        [HttpGet("payslips/{id:int}")]
        [ProducesResponseType<PayslipDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayslip(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new GetPayslipQuery(id), ct));

        [HttpPost]
        [ProducesResponseType<int>(StatusCodes.Status201Created)]
        public async Task<IActionResult> Generate(GeneratePayrollRunCommand command, CancellationToken ct) =>
            FromCreated(await Sender.Send(command, ct), nameof(GetById), id => new { id });

        [HttpPost("{id:int}/recalculate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Recalculate(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new RecalculatePayrollRunCommand(id), ct));

        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Approve(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new ApprovePayrollRunCommand(id), ct));

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new DeletePayrollRunCommand(id), ct));
    }
}
