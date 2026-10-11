using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed record UpdateDepartmentCommand(int Id, string NameAr, string NameEn, decimal IncentivePercentage)
        : IRequest<Result>;
}
