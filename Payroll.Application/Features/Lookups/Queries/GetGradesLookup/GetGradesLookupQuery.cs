using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Lookups.Queries.GetGradesLookup
{
    public sealed record GetGradesLookupQuery : IRequest<IReadOnlyList<LookupDto>>;
}
