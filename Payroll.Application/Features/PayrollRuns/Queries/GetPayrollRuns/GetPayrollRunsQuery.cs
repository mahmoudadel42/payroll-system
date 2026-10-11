using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;
using Payroll.Domain.Enums;

namespace Payroll.Application.Features.PayrollRuns.Queries.GetPayrollRuns
{
    public sealed record GetPayrollRunsQuery : PagedRequest, IRequest<PagedResult<PayrollRunDto>>
    {
        public int? Year { get; init; }
        public PayrollRunStatus? Status { get; init; }
    }
}
