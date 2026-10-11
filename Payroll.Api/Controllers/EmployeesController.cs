using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Employees.Commands.CreateEmployee;
using Payroll.Application.Features.Employees.Commands.DeleteEmployee;
using Payroll.Application.Features.Employees.Commands.UpdateEmployee;
using Payroll.Application.Features.Employees.Model;
using Payroll.Application.Features.Employees.Queries.GetEmployeeById;
using Payroll.Application.Features.Employees.Queries.GetEmployees;

namespace Payroll.Api.Controllers
{
    public sealed class EmployeesController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<PagedResult<EmployeeDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetEmployeesQuery query, CancellationToken ct) =>
            Ok(await Sender.Send(query, ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType<EmployeeDetailsDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new GetEmployeeByIdQuery(id), ct));

        [HttpPost]
        [ProducesResponseType<int>(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateEmployeeCommand command, CancellationToken ct) =>
            FromCreated(await Sender.Send(command, ct), nameof(GetById), id => new { id });

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, UpdateEmployeeCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Id = id }, ct));

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new DeleteEmployeeCommand(id), ct));
    }
}
