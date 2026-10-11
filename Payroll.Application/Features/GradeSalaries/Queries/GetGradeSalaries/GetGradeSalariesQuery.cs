using MediatR;
using Payroll.Application.Features.GradeSalaries.Model;

namespace Payroll.Application.Features.GradeSalaries.Queries.GetGradeSalaries
{
    public sealed record GetGradeSalariesQuery : IRequest<IReadOnlyList<GradeSalaryDto>>;
}
