using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Queries.GetDepartmentsLookup
{
    public sealed record GetDepartmentsLookupQuery : IRequest<IReadOnlyList<LookupDto>>;
}
