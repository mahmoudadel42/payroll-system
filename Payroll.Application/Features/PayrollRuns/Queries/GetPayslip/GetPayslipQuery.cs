using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Queries.GetPayslip
{
    public sealed record GetPayslipQuery(int Id) : IRequest<Result<PayslipDto>>;
}
