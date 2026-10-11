using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.PayrollRuns.Commands.ApprovePayrollRun
{
    public sealed record ApprovePayrollRunCommand(int Id) : IRequest<Result>;
}
