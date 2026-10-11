using MediatR;
using Payroll.Application.Features.ServiceYearsRules.Model;

namespace Payroll.Application.Features.ServiceYearsRules.Queries.GetServiceYearsRules
{
    public sealed record GetServiceYearsRulesQuery : IRequest<IReadOnlyList<ServiceYearsRuleDto>>;
}
