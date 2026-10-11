using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Queries.GetPayrollRunById
{
    public sealed record GetPayrollRunByIdQuery(int Id) : IRequest<Result<PayrollRunDetailsDto>>;
}
