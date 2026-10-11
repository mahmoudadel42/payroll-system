using MediatR;
using Payroll.Application.Features.AbsenceRules.Model;

namespace Payroll.Application.Features.AbsenceRules.Queries.GetAbsenceRules
{
    public sealed record GetAbsenceRulesQuery : IRequest<IReadOnlyList<AbsenceRuleDto>>;
}
