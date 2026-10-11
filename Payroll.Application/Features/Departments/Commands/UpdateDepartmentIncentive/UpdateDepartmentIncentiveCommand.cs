using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.UpdateDepartmentIncentive
{
    public sealed record UpdateDepartmentIncentiveCommand(int Id, decimal IncentivePercentage) : IRequest<Result>;
}
