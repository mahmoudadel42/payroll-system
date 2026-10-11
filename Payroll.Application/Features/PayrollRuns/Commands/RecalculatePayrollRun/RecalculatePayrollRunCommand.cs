using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.PayrollRuns.Commands.RecalculatePayrollRun
{
    public sealed record RecalculatePayrollRunCommand(int Id) : IRequest<Result>;
}
