using Microsoft.AspNetCore.Mvc;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.Departments.Queries.GetDepartmentsLookup;
using Payroll.Application.Features.Lookups.Queries.GetGradesLookup;

namespace Payroll.Api.Controllers
{
    public sealed class LookupsController : ApiControllerBase
    {
        [HttpGet("grades")]
        [ProducesResponseType<IReadOnlyList<LookupDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Grades(CancellationToken ct) =>
            Ok(await Sender.Send(new GetGradesLookupQuery(), ct));

        [HttpGet("departments")]
        [ProducesResponseType<IReadOnlyList<LookupDto>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> Departments(CancellationToken ct) =>
            Ok(await Sender.Send(new GetDepartmentsLookupQuery(), ct));
    }
}
