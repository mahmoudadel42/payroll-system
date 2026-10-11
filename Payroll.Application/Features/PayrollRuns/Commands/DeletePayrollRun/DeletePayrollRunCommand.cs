using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.PayrollRuns.Commands.DeletePayrollRun
{
    public sealed record DeletePayrollRunCommand(int Id) : IRequest<Result>;
}
