using MediatR;
using Payroll.Application.Common.Models;

namespace Payroll.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed record CreateDepartmentCommand(string NameAr, string NameEn, decimal IncentivePercentage)
        : IRequest<Result<int>>;
}
