using MediatR;
using Payroll.Application.Common.Models;
using Payroll.Application.Features.PayrollRuns.Model;

namespace Payroll.Application.Features.PayrollRuns.Commands.GeneratePayrollRun
{
    public sealed record GeneratePayrollRunCommand(int Year, int Month) : IRequest<Result<int>>, IPayrollPeriod;
}
