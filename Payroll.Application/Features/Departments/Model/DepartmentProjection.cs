using System.Linq.Expressions;
using Payroll.Domain.Entities;

namespace Payroll.Application.Features.Departments.Model
{
    public static class DepartmentProjection
    {
        public static readonly Expression<Func<Department, DepartmentDto>> ToDto = d => new DepartmentDto
        {
            Id = d.Id,
            NameAr = d.NameAr,
            NameEn = d.NameEn,
            IncentivePercentage = d.IncentivePercentage,
            EmployeesCount = d.Employees.Count
        };
    }
}
