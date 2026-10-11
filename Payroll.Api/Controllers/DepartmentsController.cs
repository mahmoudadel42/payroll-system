using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Models;
using Payroll.Application.Common.Security;
using Payroll.Application.Features.Departments.Commands.CreateDepartment;
using Payroll.Application.Features.Departments.Commands.DeleteDepartment;
using Payroll.Application.Features.Departments.Commands.UpdateDepartment;
using Payroll.Application.Features.Departments.Commands.UpdateDepartmentIncentive;
using Payroll.Application.Features.Departments.Model;
using Payroll.Application.Features.Departments.Queries.GetDepartmentById;
using Payroll.Application.Features.Departments.Queries.GetDepartments;

namespace Payroll.Api.Controllers
{
    public sealed class DepartmentsController : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType<PagedResult<DepartmentDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetDepartmentsQuery query, CancellationToken ct) =>
            Ok(await Sender.Send(query, ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType<DepartmentDto>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new GetDepartmentByIdQuery(id), ct));

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType<int>(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateDepartmentCommand command, CancellationToken ct) =>
            FromCreated(await Sender.Send(command, ct), nameof(GetById), id => new { id });

        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(int id, UpdateDepartmentCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Id = id }, ct));

        [HttpPatch("{id:int}/incentive")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateIncentive(int id, UpdateDepartmentIncentiveCommand command, CancellationToken ct) =>
            FromResult(await Sender.Send(command with { Id = id }, ct));

        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct) =>
            FromResult(await Sender.Send(new DeleteDepartmentCommand(id), ct));
    }
}
